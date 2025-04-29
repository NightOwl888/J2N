using NUnit.Framework;
using System;
using System.Threading;

namespace J2N
{
    public class TestRandomizer : TestCase
    {
        Randomizer r;

        /**
         * @tests java.util.Random#Random()
         */
        [Test]
        public void Test_Constructor()
        {
            // Test for method java.util.Random()
            assertTrue("Used to test", true);
        }

        /**
         * @tests java.util.Random#Random(long)
         */
        [Test]
        public void Test_ConstructorJ()
        {
            Randomizer r = new Randomizer(8409238L);
            Randomizer r2 = new Randomizer(8409238L);
            for (int i = 0; i < 100; i++)
                assertTrue("Values from randoms with same seed don't match", r
                        .Next() == r2.Next());
        }

        /**
         * @tests java.util.Random#nextBoolean()
         */
        [Test]
        public void Test_nextBoolean()
        {
            // Test for method boolean java.util.Random.nextBoolean()
            bool falseAppeared = false, trueAppeared = false;
            for (int counter = 0; counter < 100; counter++)
                if (r.NextBoolean())
                    trueAppeared = true;
                else
                    falseAppeared = true;
            assertTrue("Calling nextBoolean() 100 times resulted in all trues",
                    falseAppeared);
            assertTrue("Calling nextBoolean() 100 times resulted in all falses",
                    trueAppeared);
        }

        /**
         * @tests java.util.Random#nextBytes(byte[])
         */
        [Test]
        public void Test_NextBytes_ByteArray()
        {
            // Test for method void java.util.Random.nextBytes(byte [])
            bool someDifferent = false;
            byte[] randomBytes = new byte[100];
            r.NextBytes(randomBytes);
            byte firstByte = randomBytes[0];
            for (int counter = 1; counter < randomBytes.Length; counter++)
                if (randomBytes[counter] != firstByte)
                    someDifferent = true;
            assertTrue(
                    "nextBytes() returned an array of length 100 of the same byte",
                    someDifferent);
        }

        /**
         * @tests java.util.Random#nextBytes(Span<byte>)
         */
        [Test]
        public void Test_NextBytes_Span() // J2N specific
        {
            // Test for method void java.util.Random.nextBytes(byte [])
            bool someDifferent = false;
            Span<byte> randomBytes = stackalloc byte[100];
            r.NextBytes(randomBytes);
            byte firstByte = randomBytes[0];
            for (int counter = 1; counter < randomBytes.Length; counter++)
                if (randomBytes[counter] != firstByte)
                    someDifferent = true;
            assertTrue(
                    "nextBytes() returned an array of length 100 of the same byte",
                    someDifferent);
        }

        [Test]
        public void Test_NextBytes_Repeatability() // J2N specific
        {
            var r1 = new Randomizer(42);
            var r2 = new Randomizer(42);

            for (int i = 0; i < 2; i++)
            {
                byte[] b1 = new byte[1000];
                byte[] b2 = new byte[1000];
                if (i == 0)
                {
                    r1.NextBytes(b1);
                    r2.NextBytes(b2);
                }
                else
                {
                    r1.NextBytes((Span<byte>)b1);
                    r2.NextBytes((Span<byte>)b2);
                }
                Assert.IsTrue(b1.AsSpan().SequenceEqual(b2));
            }
        }

        [Test]
        public void Test_NextBytes_Against_JDK22() // J2N specific
        {
            ReadOnlySpan<byte> expected = stackalloc byte[]
            {
                (byte)0x35, (byte)0x9D, (byte)0x41, (byte)0xBA, (byte)0xF7, (byte)0x8A, (byte)0xFE, (byte)0x0D,
                (byte)0xE1, (byte)0xBB, (byte)0xE7, (byte)0xAE, (byte)0x28, (byte)0xC0, (byte)0x45, (byte)0x0C,
                (byte)0xE4, (byte)0x3C, (byte)0x08, (byte)0x4F, (byte)0x4B, (byte)0xBB, (byte)0x2B, (byte)0xF1,
                (byte)0x83, (byte)0x9D, (byte)0xEE, (byte)0x46, (byte)0x6D, (byte)0x85, (byte)0x2C, (byte)0xB5,
                (byte)0xBE, (byte)0x6A, (byte)0x61, (byte)0xAA, (byte)0x9A, (byte)0x0C, (byte)0x61, (byte)0x17,
                (byte)0xBD, (byte)0x67, (byte)0x43, (byte)0xE7, (byte)0xDC, (byte)0x97, (byte)0x85, (byte)0x73,
                (byte)0x99, (byte)0x8E, (byte)0x68, (byte)0x5E, (byte)0x88, (byte)0x5C, (byte)0xB3, (byte)0x61,
                (byte)0xF8, (byte)0x6C, (byte)0x97, (byte)0x46, (byte)0x20, (byte)0xBE, (byte)0xBF, (byte)0xB0
            };

            var target = new Randomizer(42L);
            Span<byte> actual = stackalloc byte[64];
            target.NextBytes(actual);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.That(expected[i], Is.EqualTo(actual[i]), $"Loop {i}");
            }
        }

        /**
         * @tests java.util.Random#nextDouble()
         */
        [Test]
        public void Test_nextDouble()
        {
            // Test for method double java.util.Random.nextDouble()
            double lastNum = r.NextDouble();
            double nextNum;
            bool someDifferent = false;
            bool inRange = true;
            for (int counter = 0; counter < 100; counter++)
            {
                nextNum = r.NextDouble();
                if (nextNum != lastNum)
                    someDifferent = true;
                if (!(0 <= nextNum && nextNum < 1.0))
                    inRange = false;
                lastNum = nextNum;
            }
            assertTrue("Calling nextDouble 100 times resulted in same number",
                    someDifferent);
            assertTrue(
                    "Calling nextDouble resulted in a number out of range [0,1)",
                    inRange);
        }

        [Test]
        public void Test_NextDouble_Against_JDK22() // J2N specific
        {
            ReadOnlySpan<long> expected = stackalloc long[]
            {
                (long)0x3FE74833A06FF457L, (long)0x3FE5DCF778622E01L, (long)0x3FD3C20F3F12BBB4L, (long)0x3FD1BBA76B52C856L,
                (long)0x3FE54C2D50BB0864L, (long)0x3FECE86CF39C2CBEL, (long)0x3FD79A23A61B35C8L, (long)0x3FD1A5DB3B0BFBE2L,
                (long)0x3FDDAC800C318574L, (long)0x3FE90D8807FC450FL, (long)0x3FED6B22193735E3L, (long)0x3FDBEF77D7096B88L,
                (long)0x3FE7FF3B3F71EEC2L, (long)0x3FD8BD82FCC5C830L, (long)0x3FC6B45684D148F0L, (long)0x3FE304EA1AB4DACAL,
                (long)0x3FCAD9A9E8056AE0L, (long)0x3FEA6E4FFAEBD9A3L, (long)0x3FC60B3CC513B2BCL, (long)0x3FE2CC3482318166L,
                (long)0x3FE80A7D35262360L, (long)0x3FE245F668A46CF4L, (long)0x3FE28F9058B9AE4FL, (long)0x3FE8148FBE63914EL,
                (long)0x3FA0160D28842350L, (long)0x3FD6E828F32F702AL, (long)0x3FEA2B647816A9F1L, (long)0x3FDABB648C500436L,
                (long)0x3FEF2B4CE0A52E25L, (long)0x3FE6D4395C4399B5L, (long)0x3FDEC1BB9DF9A848L, (long)0x3FD2AA8003832CC4L,
                (long)0x3FEE654114C45EBEL, (long)0x3FEA417811828D73L, (long)0x3FE45F64630705A3L, (long)0x3FD79FAFC22ECA88L,
                (long)0x3FD70E6A76FB03CAL, (long)0x3FDBD17CBA617570L, (long)0x3FDD44AEEB22B096L, (long)0x3FDE4086EF37050AL,
                (long)0x3FDE047708C86816L, (long)0x3FEA796CE04C8A07L, (long)0x3FC3550081AEBA34L, (long)0x3FEAAF083E4F151EL,
                (long)0x3FDD75A8DC2E15B8L, (long)0x3FD1F4E43A1A21A8L, (long)0x3FC9155AEB08D580L, (long)0x3FC6F26E9ECCB144L,
                (long)0x3FEBB3B4CBD9C27CL, (long)0x3FDF244D28C547AAL, (long)0x3FDAF1335021617AL, (long)0x3FE44078260FA33EL,
                (long)0x3FE6653DF50C5D31L, (long)0x3FD42E57611FBFB0L, (long)0x3FE24AB6AABF6288L, (long)0x3FD7BEAA54A6C5F0L,
                (long)0x3FEBE5E7B93B059EL, (long)0x3FE9C88C42037919L, (long)0x3FE3B805C3125C0EL, (long)0x3FD7F580F1D6A62AL,
                (long)0x3FE64FDC912C8373L, (long)0x3FED135EE2552111L, (long)0x3FC91B58E57827C4L, (long)0x3FE9E459BA4748C8L
            };

            var target = new Randomizer(42L);
            for (int i = 0; i < expected.Length; i++)
            {
                double d = target.NextDouble();
                long value = BitConversion.DoubleToRawInt64Bits(d);
                Assert.That(expected[i], Is.EqualTo(value), $"Loop {i}");
            }
        }

        /**
         * @tests java.util.Random#nextFloat()
         */
        [Test]
        public void Test_nextFloat()
        {
            // Test for method float java.util.Random.nextFloat()
            float lastNum = r.NextSingle();
            float nextNum;
            bool someDifferent = false;
            bool inRange = true;
            for (int counter = 0; counter < 100; counter++)
            {
                nextNum = r.NextSingle();
                if (nextNum != lastNum)
                    someDifferent = true;
                if (!(0 <= nextNum && nextNum < 1.0))
                    inRange = false;
                lastNum = nextNum;
            }
            assertTrue("Calling nextFloat 100 times resulted in same number",
                    someDifferent);
            assertTrue("Calling nextFloat resulted in a number out of range [0,1)",
                    inRange);
        }

        [Test]
        public void Test_NextFloat_Against_JDK22()
        {
            ReadOnlySpan<int> expected = stackalloc int[]
            {
                (int)0x3F3A419D, (int)0x3D5FE8A0, (int)0x3F2EE7BB, (int)0x3D445C00,
                (int)0x3E9E1078, (int)0x3F712BBB, (int)0x3E8DDD3A, (int)0x3F352C85,
                (int)0x3F2A616A, (int)0x3DBB0860, (int)0x3F674367, (int)0x3EE70B2E,
                (int)0x3EBCD11C, (int)0x3EC366B8, (int)0x3E8D2ED8, (int)0x3F30BFBE,
                (int)0x3EED6400, (int)0x3F431857, (int)0x3F486C40, (int)0x3F7F88A1,
                (int)0x3F6B5910, (int)0x3E1B9AF0, (int)0x3EDF7BBE, (int)0x3EE12D70,
                (int)0x3F3FF9D9, (int)0x3F6E3DD8, (int)0x3EC5EC16, (int)0x3F4C5C83,
                (int)0x3E35A2B4, (int)0x3E1A291C, (int)0x3F182750, (int)0x3EAD36B2,
                (int)0x3E56CD4C, (int)0x3E8056AE, (int)0x3F53727F, (int)0x3EBAF668,
                (int)0x3E3059E4, (int)0x3E227654, (int)0x3F1661A4, (int)0x3E8C6058,
                (int)0x3F4053E9, (int)0x3F24C46C, (int)0x3F122FB3, (int)0x3DA46CF0,
                (int)0x3F147C82, (int)0x3DB9AE48, (int)0x3F40A47D, (int)0x3F4C7229,
                (int)0x3D00B060, (int)0x3EA2108C, (int)0x3EB74146, (int)0x3E4BDC08,
                (int)0x3F515B23, (int)0x3C354F80, (int)0x3ED5DB24, (int)0x3F450043,
                (int)0x3F795A67, (int)0x3DA52E20, (int)0x3F36A1CA, (int)0x3F087336,
                (int)0x3EF60DDC, (int)0x3F5F9A84, (int)0x3E955400, (int)0x3E60CB30
            };

            var target = new Randomizer(42L);
            for (int i = 0; i < expected.Length; i++)
            {
                float f = target.NextSingle();
                int value = BitConversion.SingleToRawInt32Bits(f);
                Assert.That(expected[i], Is.EqualTo(value), $"Loop {i}");
            }
        }

        /**
         * @tests java.util.Random#nextGaussian()
         */
        [Test]
        public void Test_nextGaussian()
        {
            // Test for method double java.util.Random.nextGaussian()
            double lastNum = r.NextGaussian();
            double nextNum;
            bool someDifferent = false;
            bool someInsideStd = false;
            for (int counter = 0; counter < 100; counter++)
            {
                nextNum = r.NextGaussian();
                if (nextNum != lastNum)
                    someDifferent = true;
                if (-1.0 <= nextNum && nextNum <= 1.0)
                    someInsideStd = true;
                lastNum = nextNum;
            }
            assertTrue("Calling nextGaussian 100 times resulted in same number",
                    someDifferent);
            assertTrue(
                    "Calling nextGaussian 100 times resulted in no number within 1 std. deviation of mean",
                    someInsideStd);
        }

        [Test]
        public void Test_NextGaussian_Against_JDK22() // J2N specific
        {
            ReadOnlySpan<long> expected = PopulateExpected(stackalloc long[64]);

            var target = new Randomizer(42L);

            for (int i = 0; i < expected.Length; i++)
            {
                double d = target.NextGaussian();
                long actualValue = BitConversion.DoubleToRawInt64Bits(d);
                long expectedValue = expected[i];

                // J2N: .NET is more precise than Java at calculating the values. So,
                // we may be off by 1 ULP. This checks to ensure the calculation
                // behavior is within one ULP, so at least we know we have a valid algorithm.
                long ulpDifference = Math.Abs(actualValue - expectedValue);
                Assert.That(ulpDifference, Is.LessThanOrEqualTo(1L),
                    $"Loop {i} - actual: 0x{actualValue:X}, expected: 0x{expectedValue:X}, ULP diff: {ulpDifference}");
            }


            static ReadOnlySpan<long> PopulateExpected(Span<long> buffer)
            {
                unchecked
                {
                    ReadOnlySpan<long> tempValues = stackalloc long[64]
                    {
                        (long)0x3FF2453E82115D86L, (long)0x3FED6BCA38120847L, (long)0xBFEE654EB7A040C2L, (long)0xBFF1B63B72513280L,
                        (long)0x3FD1FB89A19B83AFL, (long)0x3FE5E86E10AAD3BCL, (long)0xBFEA26AD824BCBC5L, (long)0xBFF658A6C0AAD25AL,
                        (long)0xBFC870DEAB7EBA10L, (long)0x3FF7C787B1B31EF5L, (long)0x3FE9AC800B282266L, (long)0xBFBF1B78957AC777L,
                        (long)0x3FF6916EF96A4B20L, (long)0xBFE47CC975ADE686L, (long)0xBFF35AB426047CE4L, (long)0x3FD6A3F753C46425L,
                        (long)0xBFDF61E37EBA8C40L, (long)0x3FE19F82C682EB1EL, (long)0xBFF341BEB2082109L, (long)0x3FD48B8707F62409L,
                        (long)0x3FF8D1802FC33C18L, (long)0x3FDC10E1556F284DL, (long)0x3FDED280AC43BD0BL, (long)0x3FF85068ADA272E1L,
                        (long)0xBFD1B79C6DD6B33AL, (long)0xBFB57D065F7EBDECL, (long)0x3FF417E459919AA8L, (long)0xBFD4D144FAFBDF35L,
                        (long)0xBFC62E60AC9AEB2BL, (long)0xBFFDBCC3C6CD48B0L, (long)0x3FF6C7E9CAB4F8AEL, (long)0xBFF5D1D26121B38AL,
                        (long)0xBFFF6F45B81B5333L, (long)0xBFED6521039F1713L, (long)0xC004641F12DE3208L, (long)0xBFFA186A33E7E659L,
                        (long)0xBFBF3BB474F2C2D4L, (long)0x3FF4A065434BABFDL, (long)0xBFD139922DC805D6L, (long)0x3FD07ABD3EA30A74L,
                        (long)0xBFD4797A259D6BF9L, (long)0xBFFC4BC68227707EL, (long)0xBFDEF0D994BE2BE4L, (long)0xBFE051D78643D030L,
                        (long)0x3FF1DDE4E72623A9L, (long)0xBFA4F70816C24779L, (long)0xBFF1B905C4BE1E86L, (long)0x3FFDCC118AF6F5FFL,
                        (long)0x3FF254F4A8C24843L, (long)0xBFF0F05782409598L, (long)0x3FF1295DB59FD924L, (long)0xBFFEE86C8E534E82L,
                        (long)0x3FD343FF449F2714L, (long)0x3FCFAED6BEE89589L, (long)0x3FF67F9E530923C3L, (long)0xBFF852D2F64A2725L,
                        (long)0x3FD156E851C25E6AL, (long)0x3FE1F5C10F65B6C7L, (long)0xBFE0F45D28C3E5C2L, (long)0x3FE13FAB7789C867L,
                        (long)0x4001B2DF6D1C2FA1L, (long)0xBFE440FDE64D7017L, (long)0xBFFE217D10B8CE19L, (long)0x3FD8BD7F423BFDDFL
                    };

                    tempValues.CopyTo(buffer);
                    return buffer;
                }
            }
        }

        /**
         * @tests java.util.Random#nextInt()
         */
        [Test]
        public void Test_nextInt()
        {
            // Test for method int java.util.Random.nextInt()
            int lastNum = r.Next();
            int nextNum;
            bool someDifferent = false;
            for (int counter = 0; counter < 100; counter++)
            {
                nextNum = r.Next();
                if (nextNum != lastNum)
                    someDifferent = true;
                lastNum = nextNum;
            }
            assertTrue("Calling nextInt 100 times resulted in same number",
                    someDifferent);
        }

        [Test]
        public void Test_NextInt_Against_JDK8() // J2N specific
        {
            ReadOnlySpan<int> expected = PopulateExpected(stackalloc int[64]);

            var target = new Randomizer(42L);
            for (int i = 0; i < expected.Length; i++)
            {
                int value = target.Next();
                Assert.That(expected[i], Is.EqualTo(value), $"Loop {i}");
            }

            static ReadOnlySpan<int> PopulateExpected(Span<int> buffer)
            {
                unchecked
                {
                    int[] tempValues = new int[]
                    {
                        (int)0xBA419D35, (int)0x0DFE8AF7, (int)0xAEE7BBE1, (int)0x0C45C028,
                        (int)0x4F083CE4, (int)0xF12BBB4B, (int)0x46EE9D83, (int)0xB52C856D,
                        (int)0xAA616ABE, (int)0x17610C9A, (int)0xE74367BD, (int)0x738597DC,
                        (int)0x5E688E99, (int)0x61B35C88, (int)0x46976CF8, (int)0xB0BFBE20,
                        (int)0x76B20011, (int)0xC3185754, (int)0xC86C400F, (int)0xFF88A1E3,
                        (int)0xEB5910F1, (int)0x26E6BC6F, (int)0x6FBDDF55, (int)0x7096B883,
                        (int)0xBFF9D9D6, (int)0xEE3DD852, (int)0x62F60BDC, (int)0xCC5C8305,
                        (int)0x2D68AD16, (int)0x268A478C, (int)0x982750F4, (int)0x569B5949,
                        (int)0x35B353DD, (int)0x402B571A, (int)0xD3727FF2, (int)0x5D7B347F,
                        (int)0x2C167999, (int)0x289D95FA, (int)0x9661A432, (int)0x46302CD9,
                        (int)0xC053E997, (int)0xA4C46C02, (int)0x922FB35E, (int)0x148D9E82,
                        (int)0x947C82EE, (int)0x1735C9ED, (int)0xC0A47DEF, (int)0xCC7229CE,
                        (int)0x080B069B, (int)0x510846B6, (int)0x5BA0A3F3, (int)0x32F702B1,
                        (int)0xD15B23ED, (int)0x02D53E28, (int)0x6AED9211, (int)0xC5004375,
                        (int)0xF95A6723, (int)0x14A5C4B6, (int)0xB6A1CACA, (int)0x887336A7,
                        (int)0x7B06EE53, (int)0xDF9A8487, (int)0x4AAA0001, (int)0x3832CC52,
                    };

                    for (int i = 0; i < tempValues.Length; i++)
                    {
                        buffer[i] = tempValues[i];
                    }

                    return buffer;
                }
            }
        }

        /**
         * @tests java.util.Random#nextInt(int)
         */
        [Test]
        public void Test_nextIntI()
        {
            // Test for method int java.util.Random.nextInt(int)
            int range = 10;
            int lastNum = r.Next(range);
            int nextNum;
            bool someDifferent = false;
            bool inRange = true;
            for (int counter = 0; counter < 100; counter++)
            {
                nextNum = r.Next(range);
                if (nextNum != lastNum)
                    someDifferent = true;
                if (!(0 <= nextNum && nextNum < range))
                    inRange = false;
                lastNum = nextNum;
            }
            assertTrue("Calling nextInt (range) 100 times resulted in same number",
                    someDifferent);
            assertTrue(
                    "Calling nextInt (range) resulted in a number outside of [0, range)",
                    inRange);

        }

        [Test]
        public void Test_NextIntI_Against_JDK8() // J2N specific
        {
            ReadOnlySpan<int> expected = stackalloc int[64] {
                130, 13, 248, 134, 220, 25, 5, 168,
                19, 93, 182, 2, 26, 42, 226, 32,
                206, 170, 243, 209, 150, 113, 226, 163,
                243, 241, 30, 208, 137, 246, 230, 6,
                180, 85, 217, 227, 212, 93, 213, 164,
                129, 65, 73, 237, 225, 34, 57, 185,
                157, 193, 193, 190, 58, 6, 160, 160,
                213, 59, 11, 3, 147, 69, 52, 101
            };

            var target = new Randomizer(42L);
            for (int i = 0; i < expected.Length; i++)
            {
                int value = target.Next(250);
                Assert.That(expected[i], Is.EqualTo(value), $"Loop {i}");
            }
        }


        /**
         * @tests java.util.Random#nextInt(int)
         */
        [Test]
        public void Test_nextIntI_I() // J2N specific method to test overload inherited from System.Random
        {
            // Test for method int Random.Next(int, int)
            int rangeBegin = 50;
            int rangeEnd = 100;
            int lastNum = r.Next(rangeBegin, rangeEnd);
            int nextNum;
            bool someDifferent = false;
            bool inRange = true;
            for (int counter = 0; counter < 100; counter++)
            {
                nextNum = r.Next(rangeBegin, rangeEnd);
                if (nextNum != lastNum)
                    someDifferent = true;
                if (!(rangeBegin <= nextNum && nextNum < rangeEnd))
                    inRange = false;
                lastNum = nextNum;
            }
            assertTrue("Calling Next(rangeBegin, rangeEnd) 100 times resulted in same number",
                    someDifferent);
            assertTrue(
                    "Calling Next(rangeBegin, rangeEnd) resulted in a number outside of [rangeBegin, rangeEnd)",
                    inRange);

        }

        [Test]
        public void Test_NextIntI_I_Against_JDK22() // J2N specific
        {
            ReadOnlySpan<int> expected = PopulateExpected(stackalloc int[64]);

            var target = new Randomizer(42L);
            for (int i = 0; i < expected.Length; i++)
            {
                int value = target.Next(short.MinValue, int.MaxValue);
                Assert.That(expected[i], Is.EqualTo(value), $"Loop {i}");
            }

            static ReadOnlySpan<int> PopulateExpected(Span<int> buffer)
            {
                unchecked
                {
                    int[] tempValues = new int[]
                    {
                        (int)0x0DFE8AF7, (int)0x0C45C028, (int)0x4F083CE4, (int)0x46EE9D83,
                        (int)0x17610C9A, (int)0x738597DC, (int)0x5E688E99, (int)0x61B35C88,
                        (int)0x46976CF8, (int)0x76B20011, (int)0x26E6BC6F, (int)0x6FBDDF55,
                        (int)0x7096B883, (int)0x62F60BDC, (int)0x2D68AD16, (int)0x268A478C,
                        (int)0x569B5949, (int)0x35B353DD, (int)0x402B571A, (int)0x5D7B347F,
                        (int)0x2C167999, (int)0x289D95FA, (int)0x46302CD9, (int)0x148D9E82,
                        (int)0x1735C9ED, (int)0x080B069B, (int)0x510846B6, (int)0x5BA0A3F3,
                        (int)0x32F702B1, (int)0x02D53E28, (int)0x6AED9211, (int)0x14A5C4B6,
                        (int)0x7B06EE53, (int)0x4AAA0001, (int)0x3832CC52, (int)0x3051AE6F,
                        (int)0x60E0B477, (int)0x5E7EBF2C, (int)0x22ECA89B, (int)0x5C39A9DA,
                        (int)0x6FB03CBB, (int)0x6F45F2EA, (int)0x7512BBB5, (int)0x79021B83,
                        (int)0x7811DC31, (int)0x099140ED, (int)0x26AA011E, (int)0x0D75D1A3,
                        (int)0x75D6A35D, (int)0x47D390C7, (int)0x322AB5C6, (int)0x5846AC1E,
                        (int)0x2DE4DD0E, (int)0x7B384F91, (int)0x7C913489, (int)0x6BC4CD5D,
                        (int)0x021617A9, (int)0x50B95DAA, (int)0x11FBFB1D, (int)0x57EC5114,
                        (int)0x5EFAA97C, (int)0x4A6C5F10, (int)0x2760B3C7, (int)0x406F233D
                    };

                    for (int i = 0; i < tempValues.Length; i++)
                    {
                        buffer[i] = tempValues[i];
                    }

                    return buffer;
                }
            }
        }

        /**
         * @tests java.util.Random#nextLong()
         */
        [Test]
        public void Test_nextLong()
        {
            // Test for method long java.util.Random.nextLong()
            long lastNum = r.NextInt64();
            long nextNum;
            bool someDifferent = false;
            for (int counter = 0; counter < 100; counter++)
            {
                nextNum = r.NextInt64();
                if (nextNum != lastNum)
                    someDifferent = true;
                lastNum = nextNum;
            }
            assertTrue("Calling nextLong 100 times resulted in same number",
                    someDifferent);
        }

        [Test]
        public void Test_NextLong_Against_JDK8() // J2N specific
        {
            ReadOnlySpan<long> expected = PopulateExpected(stackalloc long[64]);

            var target = new Randomizer(42L);
            for (int i = 0; i < expected.Length; i++)
            {
                long value = target.NextInt64();
                Assert.That(expected[i], Is.EqualTo(value), $"Loop {i}");
            }

            static ReadOnlySpan<long> PopulateExpected(Span<long> buffer)
            {
                unchecked
                {
                    long[] tempValues = new long[]
                    {
                        (long)0xBA419D350DFE8AF7, (long)0xAEE7BBE10C45C028, (long)0x4F083CE3F12BBB4B, (long)0x46EE9D82B52C856D,
                        (long)0xAA616ABE17610C9A, (long)0xE74367BD738597DC, (long)0x5E688E9961B35C88, (long)0x46976CF7B0BFBE20,
                        (long)0x76B20010C3185754, (long)0xC86C400EFF88A1E3, (long)0xEB5910F126E6BC6F, (long)0x6FBDDF557096B883,
                        (long)0xBFF9D9D5EE3DD852, (long)0x62F60BDBCC5C8305, (long)0x2D68AD16268A478C, (long)0x982750F4569B5949,
                        (long)0x35B353DD402B571A, (long)0xD3727FF25D7B347F, (long)0x2C167999289D95FA, (long)0x9661A43246302CD9,
                        (long)0xC053E996A4C46C02, (long)0x922FB35E148D9E82, (long)0x947C82EE1735C9ED, (long)0xC0A47DEECC7229CE,
                        (long)0x080B069B510846B6, (long)0x5BA0A3F332F702B1, (long)0xD15B23ED02D53E28, (long)0x6AED9210C5004375,
                        (long)0xF95A672314A5C4B6, (long)0xB6A1CAC9887336A7, (long)0x7B06EE52DF9A8487, (long)0x4AAA00013832CC52,
                        (long)0xF32A088B988BD7D5, (long)0xD20BC0BC3051AE6F, (long)0xA2FB233F60E0B477, (long)0x5E7EBF2C22ECA89B,
                        (long)0x5C39A9DA6FB03CBB, (long)0x6F45F2E9A617570A, (long)0x7512BBB4B22B0979, (long)0x79021B82F37050BB,
                        (long)0x7811DC308C868164, (long)0xD3CB6736099140ED, (long)0x26AA011E0D75D1A3, (long)0xD57841FAC9E2A3D1,
                        (long)0x75D6A35CC2E15B9F, (long)0x47D390C6A1A21A80, (long)0x322AB5C65846AC1E, (long)0x2DE4DD0DF6658A2E,
                        (long)0xDD9DA64B7B384F91, (long)0x7C9134888C547AA0, (long)0x6BC4CD5D021617A9, (long)0xA203C138C1F467DB,
                        (long)0xB329EF8DA18BA62F, (long)0x50B95DAA11FBFB1D, (long)0x9255B57757EC5114, (long)0x5EFAA97C4A6C5F10,
                        (long)0xDF2F3DDB2760B3C7, (long)0xCE44621F406F233D, (long)0x9DC02E0C624B81C6, (long)0x5FD603DD1D6A62BE,
                        (long)0xB27EE48D25906E78, (long)0xE89AF71D4AA42237, (long)0x3236B1F82BC13E38, (long)0xCF22CDE548E91907
                    };

                    for (int i = 0; i < tempValues.Length; i++)
                    {
                        buffer[i] = tempValues[i];
                    }

                    return buffer;
                }
            }
        }

        [Test]
        public void Test_NextLongI_Against_JDK22() // J2N specific
        {
            ReadOnlySpan<long> expected = new long[]
            {
                (long)0x000000000000036AL, (long)0x00000000000002C8L, (long)0x0000000000000245L, (long)0x0000000000000364L,
                (long)0x00000000000002DCL, (long)0x000000000000027FL, (long)0x00000000000001D3L, (long)0x00000000000003DDL,
                (long)0x00000000000002C1L, (long)0x0000000000000159L, (long)0x0000000000000383L, (long)0x00000000000002B9L,
                (long)0x0000000000000075L, (long)0x000000000000008AL, (long)0x00000000000000DFL, (long)0x00000000000002FEL,
                (long)0x000000000000030CL, (long)0x00000000000003D9L, (long)0x000000000000000CL, (long)0x00000000000002B0L,
                (long)0x0000000000000378L, (long)0x00000000000000DBL, (long)0x0000000000000395L, (long)0x0000000000000372L,
                (long)0x000000000000023BL, (long)0x0000000000000017L, (long)0x0000000000000398L, (long)0x000000000000013EL,
                (long)0x0000000000000001L, (long)0x0000000000000397L, (long)0x000000000000029AL, (long)0x00000000000001B5L,
                (long)0x00000000000001F7L, (long)0x0000000000000072L, (long)0x00000000000000DDL, (long)0x000000000000017AL,
                (long)0x0000000000000245L, (long)0x0000000000000229L, (long)0x000000000000029AL, (long)0x00000000000001E2L,
                (long)0x00000000000000EEL, (long)0x000000000000009DL, (long)0x00000000000001CCL, (long)0x00000000000003B7L,
                (long)0x0000000000000372L, (long)0x0000000000000030L, (long)0x00000000000001B3L, (long)0x00000000000000E7L,
                (long)0x0000000000000239L, (long)0x000000000000018CL, (long)0x00000000000000F0L, (long)0x0000000000000316L,
                (long)0x0000000000000211L, (long)0x000000000000006BL, (long)0x0000000000000293L, (long)0x00000000000000BAL,
                (long)0x0000000000000324L, (long)0x0000000000000047L, (long)0x000000000000008FL, (long)0x000000000000031CL,
                (long)0x0000000000000080L, (long)0x00000000000001F4L, (long)0x0000000000000104L, (long)0x00000000000000A2L
            };

            var target = new Randomizer(42L);
            for (int i = 0; i < expected.Length; i++)
            {
                long value = target.NextInt64(999L);
                Assert.That(expected[i], Is.EqualTo(value), $"Loop {i}");
            }
        }


        [Test]
        public void Test_NextLongI_I_Against_JDK22() // J2N specific
        {
            ReadOnlySpan<long> expected = new long[]
            {
                (long)0x4F083CE3F12BBB4BL, (long)0x46EE9D82B52C856DL, (long)0x5E688E9961B35C88L, (long)0x46976CF7B0BFBE20L,
                (long)0x76B20010C3185754L, (long)0x6FBDDF557096B883L, (long)0x62F60BDBCC5C8305L, (long)0x2D68AD16268A478CL,
                (long)0x35B353DD402B571AL, (long)0x2C167999289D95FAL, (long)0x080B069B510846B6L, (long)0x5BA0A3F332F702B1L,
                (long)0x6AED9210C5004375L, (long)0x7B06EE52DF9A8487L, (long)0x4AAA00013832CC52L, (long)0x5E7EBF2C22ECA89BL,
                (long)0x5C39A9DA6FB03CBBL, (long)0x6F45F2E9A617570AL, (long)0x7512BBB4B22B0979L, (long)0x79021B82F37050BBL,
                (long)0x7811DC308C868164L, (long)0x26AA011E0D75D1A3L, (long)0x75D6A35CC2E15B9FL, (long)0x47D390C6A1A21A80L,
                (long)0x322AB5C65846AC1EL, (long)0x2DE4DD0DF6658A2EL, (long)0x7C9134888C547AA0L, (long)0x6BC4CD5D021617A9L,
                (long)0x50B95DAA11FBFB1DL, (long)0x5EFAA97C4A6C5F10L, (long)0x5FD603DD1D6A62BEL, (long)0x3236B1F82BC13E38L,
                (long)0x76A1559FA83A59B9L, (long)0x4E3A6F7BFFB95478L, (long)0x20526EE717BAB863L, (long)0x7F50D0BACA6BEAA7L,
                (long)0x06FE0FA1ABA9B4F6L, (long)0x09570CA077FADE46L, (long)0x7BDD30F57598E44AL, (long)0x6550F07D78C7BD66L,
                (long)0x6275DB11D13F3576L, (long)0x13BBD9AC5C42F173L, (long)0x4760E5E7FA82B084L, (long)0x5FB3B9BCEBA6E693L,
                (long)0x6E6A4D8947095549L, (long)0x55133401CD5132BCL, (long)0x50AD813CCBC247AAL, (long)0x715724A52018A4EFL,
                (long)0x18B101ED65827193L, (long)0x2BBD76B7E9A40597L, (long)0x23C94034F05004D5L, (long)0x17CB5CCC4C1F9747L,
                (long)0x06EB6DDAA918928FL, (long)0x483E25A367AC6CF9L, (long)0x580C9794BAC7BC0FL, (long)0x0399299D024AC139L,
                (long)0x1DA1CCEF4F88F506L, (long)0x61F0D3CAB4C952E3L, (long)0x429C0CE701F1B23CL, (long)0x65C4EDE4D4421E7DL,
                (long)0x421719BFF1BD7712L, (long)0x7CFA80D2352469E2L, (long)0x5C8956BA3359155EL, (long)0x56F9AA9C814AA5FAL
            };

            var target = new Randomizer(42L);
            for (int i = 0; i < expected.Length; i++)
            {
                long value = target.NextInt64(int.MinValue, long.MaxValue);
                Assert.That(expected[i], Is.EqualTo(value), $"Loop {i}");
            }
        }



        /**
         * @tests java.util.Random#setSeed(long)
         */
        [Test]
        public void Test_setSeedJ()
        {
            // Test for method void java.util.Random.setSeed(long)
            long[] randomArray = new long[100];
            bool someDifferent = false;
            long firstSeed = 1000;
            long aLong, anotherLong, yetAnotherLong;
            Randomizer aRandom = new Randomizer();
            Randomizer anotherRandom = new Randomizer();
            Randomizer yetAnotherRandom = new Randomizer();
            aRandom.Seed = (firstSeed);
            anotherRandom.Seed = (firstSeed);
            for (int counter = 0; counter < randomArray.Length; counter++)
            {
                aLong = aRandom.NextInt64();
                anotherLong = anotherRandom.NextInt64();
                assertTrue(
                        "Two randoms with same seeds gave differing nextLong values",
                        aLong == anotherLong);
                yetAnotherLong = yetAnotherRandom.NextInt64();
                randomArray[counter] = aLong;
                if (aLong != yetAnotherLong)
                    someDifferent = true;
            }
            assertTrue(
                    "Two randoms with the different seeds gave the same chain of values",
                    someDifferent);
            aRandom.Seed = (firstSeed);
            for (int counter = 0; counter < randomArray.Length; counter++)
                assertTrue(
                        "Reseting a random to its old seed did not result in the same chain of values as it gave before",
                        aRandom.NextInt64() == randomArray[counter]);
        }

        // two random create at a time should also generated different results
        // regression test for Harmony 4616
        [Test]
        public void Test_random_generate()
        {
            for (int i = 0; i < 100; i++)
            {
                Randomizer random1 = new Randomizer();
                Randomizer random2 = new Randomizer();
                assertFalse(random1.NextInt64() == random2.NextInt64());
            }
        }

        [Test]
        public void TestSyncRoot()
        {
            Randomizer random = new Randomizer(0);

            int threadCount = 10;
            int incrementCount = 300;
            var threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; i++)
                threads[i] = new Thread(() => {
                    for (int j = 0; j < incrementCount; j++)
                        lock (random.SyncRoot)
                            random.Seed++;
                });

            for (int i = 0; i < threadCount; i++)
                threads[i].Start();

            for (int i = 0; i < threadCount; i++)
                threads[i].Join();

            assertEquals(threadCount * incrementCount, random.Seed);
        }

#if FEATURE_SERIALIZABLE_RANDOM
        [Test]
        public void TestSerialization()
        {
            var target = new Randomizer(123);
            target.Next();
            target.NextGaussian();

            var clone = Clone(target);

            assertNotSame(target, clone);
            assertEquals(target.seed, clone.seed);
            assertEquals(target.internalSeed, clone.internalSeed);
            assertEquals(target.haveNextNextGaussian, clone.haveNextNextGaussian);
            assertEquals(target.nextNextGaussian, clone.nextNextGaussian);

            assertEquals(target.Next(), clone.Next());
            assertEquals(target.NextBoolean(), clone.NextBoolean());
        }
#endif

        [Test]
        [Description("Tests to ensure we aren't calling the non-repeatable BCL implementation of NextSingle().")]
        public void TestSingleRepeatability()
        {
            long seed = new Randomizer().NextInt64();

            var left = new Randomizer(seed);
            float leftFloat = left.NextSingle();

            var right = new Randomizer(seed);
            float rightFloat = right.NextSingle();

            Assert.IsTrue(BitConversion.SingleToRawInt32Bits(leftFloat) == BitConversion.SingleToRawInt32Bits(rightFloat));
        }

        [Test]
        [Description("Tests to ensure we aren't calling the non-repeatable BCL implementation of NextInt64().")]
        public void TestInt64Repeatability()
        {
            long seed = new Randomizer().NextInt64();

            var left = new Randomizer(seed);
            long leftLong = left.NextInt64();

            var right = new Randomizer(seed);
            long rightLong = right.NextInt64();

            Assert.IsTrue(leftLong == rightLong);
        }

        [Test]
        public void TestNextBytes_NullValue_ThrowsArgumentNullException()
        {
            var target = new Randomizer();
            Assert.Throws<ArgumentNullException>(() => target.NextBytes(null));
        }

        [Test]
        public void TestNext_NegativeValueOrZeroValue_ThrowsArgumentOutOfRangeException()
        {
            var target = new Randomizer();
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Next(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Next(-1));
        }

        [Test]
        public void TestNextII_MinValueGreaterThanMaxValue_ThrowsArgumentOutOfRangeException()
        {
            var target = new Randomizer();
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Next(2, 1));
        }


        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        public override void SetUp()
        {
            r = new Randomizer();
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
