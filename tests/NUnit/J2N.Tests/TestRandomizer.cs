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
        public void Test_nextBytes_B()
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
