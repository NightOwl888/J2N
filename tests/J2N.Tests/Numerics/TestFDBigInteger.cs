using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    public class TestFDBigInteger : TestCase
    {
        private const int MAX_P5 = 413;
        private const int MAX_P2 = 65;
        private const long LONG_SIGN_MASK = (1L << 63);
        private static readonly BigInteger FIVE = new BigInteger(5);
        private static readonly FDBigInteger MUTABLE_ZERO = FDBigInteger.ValueOfPow52(0, 0).LeftInplaceSub(FDBigInteger.ValueOfPow52(0, 0));
        private static readonly FDBigInteger IMMUTABLE_ZERO = FDBigInteger.ValueOfPow52(0, 0).LeftInplaceSub(FDBigInteger.ValueOfPow52(0, 0)).MakeImmutable();
        private static readonly FDBigInteger IMMUTABLE_MILLION = genMillion1().MakeImmutable();
        private static readonly FDBigInteger IMMUTABLE_BILLION = genBillion1().MakeImmutable();
        private static readonly FDBigInteger IMMUTABLE_TEN18 = genTen18().MakeImmutable();


        private static FDBigInteger mutable(String hex, int offset)
        {

            char[] chars = BigInteger.Parse(hex, NumberStyles.HexNumber).ToString().ToCharArray();
            return new FDBigInteger(0, chars, 0, chars.Length).MultByPow52(0, offset * 32);
        }

        private static FDBigInteger immutable(String hex, int offset)
        {
            FDBigInteger fd = mutable(hex, offset);
            fd.MakeImmutable();
            return fd;
        }

        private static BigInteger biPow52(int p5, int p2)
        {
            //return FIVE.Pow(p5).ShiftLeft(p2);
            //return (FIVE * p5) << p2;
            return BigInteger.Pow(FIVE, p5) << p2;
        }

        // data.length == 1, nWords == 1, offset == 0
        private static FDBigInteger genMillion1()
        {
            return FDBigInteger.ValueOfPow52(6, 0).LeftShift(6);
        }

        // data.length == 2, nWords == 1, offset == 0
        private static FDBigInteger genMillion2()
        {
            return FDBigInteger.ValueOfMulPow52(1000000L, 0, 0);
        }

        // data.length == 1, nWords == 1, offset == 0
        private static FDBigInteger genBillion1()
        {
            return FDBigInteger.ValueOfPow52(9, 0).LeftShift(9);
        }

        // data.length == 2, nWords == 2, offset == 0
        private static FDBigInteger genTen18()
        {
            return FDBigInteger.ValueOfPow52(18, 0).LeftShift(18);
        }

        private static void check(BigInteger expected, FDBigInteger actual, String message)
        {
            if (!expected.Equals(actual.ToBigInteger()))
            //if (!actual.ToHexString().Equals(expected.ToString("x")))
            {
                throw new Exception(message + " result " + actual.ToHexString() + " expected " + expected.ToString("x"));
            }
        }

        private static void TestValueOfPow52(int p5, int p2)
        {
            check(biPow52(p5, p2), FDBigInteger.ValueOfPow52(p5, p2),
                    "valueOfPow52(" + p5 + "," + p2 + ")");
        }

        [Test]
        public void TestValueOfPow52()
        {
            for (int p5 = 0; p5 <= MAX_P5; p5++)
            {
                for (int p2 = 0; p2 <= MAX_P2; p2++)
                {
                    if (p5 == 1 && p2 == 29)
                    {

                    }

                    TestValueOfPow52(p5, p2);
                }
            }
        }

        private static void TestValueOfMulPow52(long value, int p5, int p2)
        {
            //BigInteger bi = BigInteger.valueOf(value & ~LONG_SIGN_MASK);
            BigInteger bi = value & ~LONG_SIGN_MASK;
            if (value < 0)
            {
                //bi = bi.SetBit(63);
                bi |= 1 << 63;
            }
            check(biPow52(p5, p2) * bi, FDBigInteger.ValueOfMulPow52(value, p5, p2),
                    "valueOfMulPow52(" + value.ToHexString() + "." + p5 + "," + p2 + ")");
        }

        private static void TestValueOfMulPow52(long value, int p5)
        {
            TestValueOfMulPow52(value, p5, 0);
            TestValueOfMulPow52(value, p5, 1);
            TestValueOfMulPow52(value, p5, 30);
            TestValueOfMulPow52(value, p5, 31);
            TestValueOfMulPow52(value, p5, 33);
            TestValueOfMulPow52(value, p5, 63);
        }

        [Test]
        public void TestValueOfMulPow52()
        {
            for (int p5 = 0; p5 <= MAX_P5; p5++)
            {
                TestValueOfMulPow52(0xFFFFFFFFL, p5);
                TestValueOfMulPow52(0x123456789AL, p5);
                TestValueOfMulPow52(0x7FFFFFFFFFFFFFFFL, p5);
                TestValueOfMulPow52(unchecked((long)0xFFFFFFFFFFF54321L), p5);
            }
        }

        private static void TestLeftShift(FDBigInteger t, int shift, bool isImmutable)
        {
            BigInteger bt = t.ToBigInteger();
            FDBigInteger r = t.LeftShift(shift);
            if ((bt.Sign == 0 || shift == 0 || !isImmutable) && r != t)
            {
                throw new Exception("leftShift doesn't reuse its argument");
            }
            if (isImmutable)
            {
                check(bt, t, "leftShift corrupts its argument");
            }
            check(bt << shift, r, "leftShift returns wrong result");
        }

        [Test]
        public void TestLeftShift()
        {
            TestLeftShift(IMMUTABLE_ZERO, 0, true);
            TestLeftShift(IMMUTABLE_ZERO, 10, true);
            TestLeftShift(MUTABLE_ZERO, 0, false);
            TestLeftShift(MUTABLE_ZERO, 10, false);

            TestLeftShift(IMMUTABLE_MILLION, 0, true);
            TestLeftShift(IMMUTABLE_MILLION, 1, true);
            TestLeftShift(IMMUTABLE_MILLION, 12, true);
            TestLeftShift(IMMUTABLE_MILLION, 13, true);
            TestLeftShift(IMMUTABLE_MILLION, 32, true);
            TestLeftShift(IMMUTABLE_MILLION, 33, true);
            TestLeftShift(IMMUTABLE_MILLION, 44, true);
            TestLeftShift(IMMUTABLE_MILLION, 45, true);

            TestLeftShift(genMillion1(), 0, false);
            TestLeftShift(genMillion1(), 1, false);
            TestLeftShift(genMillion1(), 12, false);
            TestLeftShift(genMillion1(), 13, false);
            TestLeftShift(genMillion1(), 25, false);
            TestLeftShift(genMillion1(), 26, false);
            TestLeftShift(genMillion1(), 32, false);
            TestLeftShift(genMillion1(), 33, false);
            TestLeftShift(genMillion1(), 44, false);
            TestLeftShift(genMillion1(), 45, false);

            TestLeftShift(genMillion2(), 0, false);
            TestLeftShift(genMillion2(), 1, false);
            TestLeftShift(genMillion2(), 12, false);
            TestLeftShift(genMillion2(), 13, false);
            TestLeftShift(genMillion2(), 25, false);
            TestLeftShift(genMillion2(), 26, false);
            TestLeftShift(genMillion2(), 32, false);
            TestLeftShift(genMillion2(), 33, false);
            TestLeftShift(genMillion2(), 44, false);
            TestLeftShift(genMillion2(), 45, false);
        }

        private static void TestQuoRemIteration(FDBigInteger t, FDBigInteger s)
        {
            BigInteger bt = t.ToBigInteger();
            BigInteger bs = s.ToBigInteger();
            int q = t.QuoRemIteration(s);
            //BigInteger []
            //qr = bt.DivideAndRemainder(bs);
            //qr = BigInteger.DivRem()
            BigInteger result = BigInteger.DivRem(bt, bs, out BigInteger remainder);

            //if (!BigInteger.valueOf(q).equals(qr [0]))
            if (q != result)
            {
                throw new Exception("quoRemIteration returns incorrect quo");
            }
            //check(qr [1].multiply(BigInteger.TEN), t, "quoRemIteration returns incorrect rem");
            check(remainder * 10, t, "quoRemIteration returns incorrect rem");
        }

        [Test]
        public void TestQuoRemIteration()
        {
            // IMMUTABLE_TEN18 == 0de0b6b3a7640000
            // q = 0
            TestQuoRemIteration(mutable("00000001", 0), IMMUTABLE_TEN18);
            TestQuoRemIteration(mutable("00000001", 1), IMMUTABLE_TEN18);
            TestQuoRemIteration(mutable("0de0b6b2", 1), IMMUTABLE_TEN18);
            // q = 1 -> q = 0
            TestQuoRemIteration(mutable("0de0b6b3", 1), IMMUTABLE_TEN18);
            TestQuoRemIteration(mutable("0de0b6b3a763FFFF", 0), IMMUTABLE_TEN18);
            // q = 1
            TestQuoRemIteration(mutable("0de0b6b3a7640000", 0), IMMUTABLE_TEN18);
            TestQuoRemIteration(mutable("0de0b6b3FFFFFFFF", 0), IMMUTABLE_TEN18);
            TestQuoRemIteration(mutable("8ac72304", 1), IMMUTABLE_TEN18);
            TestQuoRemIteration(mutable("0de0b6b400000000", 0), IMMUTABLE_TEN18);
            TestQuoRemIteration(mutable("8ac72305", 1), IMMUTABLE_TEN18);
            // q = 18
            TestQuoRemIteration(mutable("FFFFFFFF", 1), IMMUTABLE_TEN18);
        }

        private static void TestCmp(FDBigInteger t, FDBigInteger o)
        {
            BigInteger bt = t.ToBigInteger();
            BigInteger bo = o.ToBigInteger();
            int cmp = t.Cmp(o);
            int bcmp = bt.CompareTo(bo);
            if (bcmp != cmp)
            {
                throw new Exception("cmp returns " + cmp + " expected " + bcmp);
            }
            check(bt, t, "cmp corrupts this");
            check(bo, o, "cmp corrupts other");
            if (o.Cmp(t) != -cmp)
            {
                throw new Exception("asymmetrical cmp");
            }
            check(bt, t, "cmp corrupts this");
            check(bo, o, "cmp corrupts other");
        }

        [Test]
        public void TestCmp()
        {
            TestCmp(mutable("FFFFFFFF", 0), mutable("100000000", 0));
            TestCmp(mutable("FFFFFFFF", 0), mutable("1", 1));
            TestCmp(mutable("5", 0), mutable("6", 0));
            TestCmp(mutable("5", 0), mutable("5", 0));
            TestCmp(mutable("5000000001", 0), mutable("500000001", 0));
            TestCmp(mutable("5000000001", 0), mutable("6", 1));
            TestCmp(mutable("5000000001", 0), mutable("5", 1));
            TestCmp(mutable("5000000000", 0), mutable("5", 1));
        }

        private static void TestCmpPow52(FDBigInteger t, int p5, int p2)
        {
            FDBigInteger o = FDBigInteger.ValueOfPow52(p5, p2);
            BigInteger bt = t.ToBigInteger();
            BigInteger bo = biPow52(p5, p2);
            int cmp = t.Cmp(o);
            int bcmp = bt.CompareTo(bo);
            if (bcmp != cmp)
            {
                throw new Exception("cmpPow52 returns " + cmp + " expected " + bcmp);
            }
            check(bt, t, "cmpPow52 corrupts this");
            check(bo, o, "cmpPow5 corrupts other");
        }

        [Test]
        public void TestCmpPow52()
        {
            TestCmpPow52(mutable("00000002", 1), 0, 31);
            TestCmpPow52(mutable("00000002", 1), 0, 32);
            TestCmpPow52(mutable("00000002", 1), 0, 33);
            TestCmpPow52(mutable("00000002", 1), 0, 34);
            TestCmpPow52(mutable("00000002", 1), 0, 64);
            TestCmpPow52(mutable("00000003", 1), 0, 32);
            TestCmpPow52(mutable("00000003", 1), 0, 33);
            TestCmpPow52(mutable("00000003", 1), 0, 34);
        }

        private static void TestAddAndCmp(FDBigInteger t, FDBigInteger x, FDBigInteger y)
        {
            BigInteger bt = t.ToBigInteger();
            BigInteger bx = x.ToBigInteger();
            BigInteger by = y.ToBigInteger();
            int cmp = t.AddAndCmp(x, y);
            int bcmp = bt.CompareTo(bx + by);
            if (bcmp != cmp)
            {
                throw new Exception("addAndCmp returns " + cmp + " expected " + bcmp);
            }
            check(bt, t, "addAndCmp corrupts this");
            check(bx, x, "addAndCmp corrupts x");
            check(by, y, "addAndCmp corrupts y");
        }

        [Test]
        public void TestAddAndCmp()
        {
            TestAddAndCmp(MUTABLE_ZERO, MUTABLE_ZERO, MUTABLE_ZERO);
            TestAddAndCmp(mutable("00000001", 0), MUTABLE_ZERO, MUTABLE_ZERO);
            TestAddAndCmp(mutable("00000001", 0), mutable("00000001", 0), MUTABLE_ZERO);
            TestAddAndCmp(mutable("00000001", 0), MUTABLE_ZERO, mutable("00000001", 0));
            TestAddAndCmp(mutable("00000001", 0), mutable("00000002", 0), MUTABLE_ZERO);
            TestAddAndCmp(mutable("00000001", 0), MUTABLE_ZERO, mutable("00000002", 0));
            TestAddAndCmp(mutable("00000001", 2), mutable("FFFFFFFF", 0), mutable("FFFFFFFF", 0));
            TestAddAndCmp(mutable("00000001", 0), mutable("00000001", 1), mutable("00000001", 0));

            TestAddAndCmp(mutable("00000001", 2), mutable("0F0F0F0F80000000", 1), mutable("F0F0F0F080000000", 1));
            TestAddAndCmp(mutable("00000001", 2), mutable("0F0F0F0E80000000", 1), mutable("F0F0F0F080000000", 1));

            TestAddAndCmp(mutable("00000002", 1), mutable("0000000180000000", 1), mutable("0000000280000000", 1));
            TestAddAndCmp(mutable("00000003", 1), mutable("0000000180000000", 1), mutable("0000000280000000", 1));
            TestAddAndCmp(mutable("00000004", 1), mutable("0000000180000000", 1), mutable("0000000280000000", 1));
            TestAddAndCmp(mutable("00000005", 1), mutable("0000000180000000", 1), mutable("0000000280000000", 1));

            TestAddAndCmp(mutable("00000001", 2), mutable("8000000000000000", 0), mutable("8000000000000000", 0));
            TestAddAndCmp(mutable("00000001", 2), mutable("8000000000000000", 0), mutable("8000000000000001", 0));
            TestAddAndCmp(mutable("00000002", 2), mutable("8000000000000000", 0), mutable("8000000000000000", 0));
            TestAddAndCmp(mutable("00000003", 2), mutable("8000000000000000", 0), mutable("8000000000000000", 0));
        }

        private static void TestMultBy10(FDBigInteger t, bool isImmutable)
        {
            BigInteger bt = t.ToBigInteger();
            FDBigInteger r = t.MultBy10();
            if ((bt.Sign == 0 || !isImmutable) && r != t)
            {
                throw new Exception("multBy10 of doesn't reuse its argument");
            }
            if (isImmutable)
            {
                check(bt, t, "multBy10 corrupts its argument");
            }
            check(bt * 10, r, "multBy10 returns wrong result");
        }

        [Test]
        public void TestMultBy10()
        {
            for (int p5 = 0; p5 <= MAX_P5; p5++)
            {
                for (int p2 = 0; p2 <= MAX_P2; p2++)
                {
                    // This strange way of creating a value ensures that it is mutable.
                    FDBigInteger value = FDBigInteger.ValueOfPow52(0, 0).MultByPow52(p5, p2);
                    TestMultBy10(value, false);
                    value.MakeImmutable();
                    TestMultBy10(value, true);
                }
            }
        }

        private static void TestMultByPow52(FDBigInteger t, int p5, int p2)
        {
            BigInteger bt = t.ToBigInteger();
            FDBigInteger r = t.MultByPow52(p5, p2);
            if (bt.Sign == 0 && r != t)
            {
                throw new Exception("multByPow52 of doesn't reuse its argument");
            }
            check(bt * biPow52(p5, p2), r, "multByPow52 returns wrong result");
        }
        [Test]
        public void TestMultByPow52()
        {
            for (int p5 = 0; p5 <= MAX_P5; p5++)
            {
                for (int p2 = 0; p2 <= MAX_P2; p2++)
                {
                    // This strange way of creating a value ensures that it is mutable.
                    FDBigInteger value = FDBigInteger.ValueOfPow52(0, 0).MultByPow52(p5, p2);
                    TestMultByPow52(value, p5, p2);
                }
            }
        }

        private static void TestLeftInplaceSub(FDBigInteger left, FDBigInteger right, bool isImmutable)
        {
            BigInteger biLeft = left.ToBigInteger();
            BigInteger biRight = right.ToBigInteger();
            FDBigInteger diff = left.LeftInplaceSub(right);
            if (!isImmutable && diff != left)
            {
                throw new Exception("leftInplaceSub of doesn't reuse its argument");
            }
            if (isImmutable)
            {
                check(biLeft, left, "leftInplaceSub corrupts its left immutable argument");
            }
            check(biRight, right, "leftInplaceSub corrupts its right argument");
            check(biLeft - biRight, diff, "leftInplaceSub returns wrong result");
        }
        [Test]
        public void TestLeftInplaceSub()
        {
            for (int p5 = 0; p5 <= MAX_P5; p5++)
            {
                for (int p2 = 0; p2 <= MAX_P2; p2++)
                {
                    //                for (int p5r = 0; p5r <= p5; p5r += 10) {
                    //                    for (int p2r = 0; p2r <= p2; p2r += 10) {
                    for (int p5r = 0; p5r <= p5; p5r++)
                    {
                        for (int p2r = 0; p2r <= p2; p2r++)
                        {
                            // This strange way of creating a value ensures that it is mutable.
                            FDBigInteger left = FDBigInteger.ValueOfPow52(0, 0).MultByPow52(p5, p2);
                            FDBigInteger right = FDBigInteger.ValueOfPow52(0, 0).MultByPow52(p5r, p2r);
                            TestLeftInplaceSub(left, right, false);
                            left = FDBigInteger.ValueOfPow52(0, 0).MultByPow52(p5, p2);
                            left.MakeImmutable();
                            TestLeftInplaceSub(left, right, true);
                        }
                    }
                }
            }
        }

        private static void TestRightInplaceSub(FDBigInteger left, FDBigInteger right, bool isImmutable)
        {
            BigInteger biLeft = left.ToBigInteger();
            BigInteger biRight = right.ToBigInteger();
            FDBigInteger diff = left.RightInplaceSub(right);
            if (!isImmutable && diff != right)
            {
                throw new Exception("rightInplaceSub of doesn't reuse its argument");
            }
            check(biLeft, left, "leftInplaceSub corrupts its left argument");
            if (isImmutable)
            {
                check(biRight, right, "leftInplaceSub corrupts its right immutable argument");
            }
            try
            {
                check(biLeft - biRight, diff, "rightInplaceSub returns wrong result");
            }
            catch (Exception)
            {
                Console.WriteLine(biLeft + " - " + biRight + " = " + (biLeft - biRight).ToString());
                throw;
            }
        }

        [Test]
        public void TestRightInplaceSub()
        {
            for (int p5 = 0; p5 <= MAX_P5; p5++)
            {
                for (int p2 = 0; p2 <= MAX_P2; p2++)
                {
                    //                for (int p5r = 0; p5r <= p5; p5r += 10) {
                    //                    for (int p2r = 0; p2r <= p2; p2r += 10) {
                    for (int p5r = 0; p5r <= p5; p5r++)
                    {
                        for (int p2r = 0; p2r <= p2; p2r++)
                        {
                            // This strange way of creating a value ensures that it is mutable.
                            FDBigInteger left = FDBigInteger.ValueOfPow52(0, 0).MultByPow52(p5, p2);
                            FDBigInteger right = FDBigInteger.ValueOfPow52(0, 0).MultByPow52(p5r, p2r);
                            TestRightInplaceSub(left, right, false);
                            right = FDBigInteger.ValueOfPow52(0, 0).MultByPow52(p5r, p2r);
                            right.MakeImmutable();
                            TestRightInplaceSub(left, right, true);
                        }
                    }
                }
            }
        }

        //public static void main(String[] args) throws Exception
        //{
        //    testValueOfPow52();
        //    testValueOfMulPow52();
        //    testLeftShift();
        //    testQuoRemIteration();
        //    testCmp();
        //    testCmpPow52();
        //    testAddAndCmp();
        //    // Uncomment the following for more comprehensize but slow testing.
        //    // testLeftInplaceSub();
        //    // testMultBy10();
        //    // testMultByPow52();
        //    // testRightInplaceSub();
        //}
    }
}
