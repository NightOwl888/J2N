using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Collections.Generic
{
    public class TestEqualityComparer : TestCase
    {
        [Test]
        public void TestLoadNullableValueTypes()
        {
            assertEquals(typeof(NullableDoubleComparer), EqualityComparer<double?>.Default.GetType());
            assertEquals(typeof(NullableSingleComparer), EqualityComparer<float?>.Default.GetType());

            assertEquals(typeof(DoubleComparer), EqualityComparer<double>.Default.GetType());
            assertEquals(typeof(SingleComparer), EqualityComparer<float>.Default.GetType());
        }

        [Test]
        public void Double_Equals_Basic()
        {
            var eq = DoubleComparer.Default;

            Assert.IsTrue(eq.Equals(1.0, 1.0));
            Assert.IsFalse(eq.Equals(1.0, 2.0));
        }

        [Test]
        public void Double_Equals_NaN()
        {
            var eq = DoubleComparer.Default;

            Assert.IsTrue(eq.Equals(double.NaN, double.NaN));

            // Different NaN payloads
            double nan1 = BitConversion.Int64BitsToDouble(0x7ff8000000000001L);
            double nan2 = BitConversion.Int64BitsToDouble(0x7ff8000000000002L);

            Assert.IsTrue(eq.Equals(nan1, nan2));
        }

        [Test]
        public void Double_Equals_Zero()
        {
            var eq = DoubleComparer.Default;

            Assert.IsFalse(eq.Equals(+0.0, -0.0));
            Assert.IsTrue(eq.Equals(+0.0, +0.0));
            Assert.IsTrue(eq.Equals(-0.0, -0.0));
        }

        [Test]
        public void Double_Equals_Cross()
        {
            var eq = DoubleComparer.Default;

            Assert.IsFalse(eq.Equals(double.NaN, 0.0));
            Assert.IsFalse(eq.Equals(double.NaN, double.PositiveInfinity));
        }

        [Test]
        public void Double_GetHashCode()
        {
            var eq = DoubleComparer.Default;

            // NaN canonicalization
            double nan1 = BitConversion.Int64BitsToDouble(0x7ff8000000000001L);
            double nan2 = BitConversion.Int64BitsToDouble(0x7ff8000000000002L);

            Assert.AreEqual(eq.GetHashCode(nan1), eq.GetHashCode(nan2));

            // Zero distinction
            Assert.AreNotEqual(eq.GetHashCode(+0.0), eq.GetHashCode(-0.0));
        }

        [Test]
        public void NullableDouble_Equals()
        {
            var eq = DoubleComparer.Default;

            double? a = 1.0;
            double? b = 1.0;
            double? c = null;

            Assert.IsTrue(eq.Equals(a.Value, b.Value));
            Assert.IsFalse(c.HasValue && eq.Equals(c.Value, 0.0));
        }

        [Test]
        public void Single_Equals_NaN()
        {
            var eq = SingleComparer.Default;

            Assert.IsTrue(eq.Equals(float.NaN, float.NaN));

            int nan1 = 0x7fc00001;
            int nan2 = 0x7fc00002;

            float f1 = BitConversion.Int32BitsToSingle(nan1);
            float f2 = BitConversion.Int32BitsToSingle(nan2);

            Assert.IsTrue(eq.Equals(f1, f2));
        }

        [Test]
        public void Float_GetHashCode()
        {
            var eq = SingleComparer.Default;

            float nan1 = BitConversion.Int32BitsToSingle(0x7fc00001);
            float nan2 = BitConversion.Int32BitsToSingle(0x7fc00002);

            Assert.AreEqual(eq.GetHashCode(nan1), eq.GetHashCode(nan2));

            Assert.AreNotEqual(eq.GetHashCode(+0.0f), eq.GetHashCode(-0.0f));
        }

        [Test]
        public void Double_Equals_Matches_JDK_Semantics()
        {
            var eq = DoubleComparer.Default;

            foreach (var bitsX in TestBitPatterns)
                foreach (var bitsY in TestBitPatterns)
                {
                    double x = BitConversion.Int64BitsToDouble(bitsX);
                    double y = BitConversion.Int64BitsToDouble(bitsY);

                    bool expected =
                        BitConversion.DoubleToInt64Bits(x) ==
                        BitConversion.DoubleToInt64Bits(y);

                    Assert.AreEqual(expected, eq.Equals(x, y));
                }
        }

        private static readonly long[] TestBitPatterns = new long[]
        {
            // Zeros
            0x0000000000000000L, // +0.0
            unchecked((long)0x8000000000000000UL), // -0.0

            // Infinities
            0x7ff0000000000000L, // +inf
            unchecked((long)0xfff0000000000000UL), // -inf

            // Canonical NaN
            0x7ff8000000000000L,

            // Non-canonical NaNs (payload variations)
            0x7ff8000000000001L,
            0x7ff8000000000002L,
            0x7fffffffffffffffL,

            unchecked((long)0xfff8000000000000UL),
            unchecked((long)0xfff8000000000001UL),

            // Smallest/largest subnormals
            0x0000000000000001L,
            0x000fffffffffffffL,
            unchecked((long)0x8000000000000001UL),
            unchecked((long)0x800fffffffffffffUL),

            // Smallest/largest normals
            0x0010000000000000L,
            0x7fefffffffffffffL,
            unchecked((long)0x8010000000000000UL),
            unchecked((long)0xffefffffffffffffUL),

            // Some arbitrary normals
            BitConverter.DoubleToInt64Bits(1.0),
            BitConverter.DoubleToInt64Bits(-1.0),
            BitConverter.DoubleToInt64Bits(123.456),
            BitConverter.DoubleToInt64Bits(-9876.54321),
        };
    }
}
