using System;
using System.Collections.Generic;

namespace J2N
{
    /// <summary>
    /// Provides a <see cref="Default"/> property that uses natural equality rules similar to those in Java.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public static class EqualityComparer<T> //: IEqualityComparer<T>, IEqualityComparer //IComparer<T>
    {
        /// <summary>
        /// Provides natural comparison rules similar to those in Java.
        /// <list type="bullet">
        ///     <item><description>
        ///         <see cref="double"/> and <see cref="float"/> are compared for positive zero and negative zero equality. These are considered
        ///         two separate numbers, as opposed to the default .NET behavior that considers them equal.
        ///     </description></item>
        ///     <item><description>
        ///         <see cref="string"/> uses culture-insesitive equality comparison using <see cref="StringComparer.Ordinal"/>.
        ///     </description></item>
        /// </list>
        /// </summary>
        public static IEqualityComparer<T> Default { get; } = LoadDefaultEqualityComparer();

        private static IEqualityComparer<T> LoadDefaultEqualityComparer()
        {
            Type genericClosingType = typeof(T);

            // Special cases to match Java equality behavior
            if (typeof(double).Equals(genericClosingType))
                return (IEqualityComparer<T>)new DoubleComparer();
            else if (typeof(float).Equals(genericClosingType))
                return (IEqualityComparer<T>)new SingleComparer();
            //else if (typeof(string).Equals(genericClosingType))
            //    return (IEqualityComparer<T>)StringComparer.Ordinal;

            return System.Collections.Generic.EqualityComparer<T>.Default;
        }

        private class DoubleComparer : System.Collections.Generic.EqualityComparer<double>, IComparer<double>
        {
            /// <summary>
            /// Accounts for signed zero (.NET does not, but Java does)
            /// </summary>
            private int CompareZero(double x, double y)
            {
                long a = BitConversion.DoubleToRawInt64Bits(x);
                long b = BitConversion.DoubleToRawInt64Bits(y);
                if (a > b)
                    return 1;
                else if (a < b)
                    return -1;

                return 0;
            }

            public int Compare(double x, double y)
            {
                if (x != 0 || y != 0)
                    return x.CompareTo(y);

                return CompareZero(x, y);
            }

            public override bool Equals(double x, double y)
            {
                if (x != 0 || y != 0)
                    return x.Equals(y);

                return CompareZero(x, y) == 0;
            }

            public override int GetHashCode(double obj)
            {
                if (obj != 0)
                    return obj.GetHashCode();

                // Make positive zero and negative zero have differnt hash codes
                return BitConversion.DoubleToRawInt64Bits(obj).GetHashCode();
            }
        }

        private class SingleComparer : System.Collections.Generic.EqualityComparer<float>, IComparer<float>
        {
            /// <summary>
            /// Accounts for signed zero (.NET does not, but Java does)
            /// </summary>
            private int CompareZero(float x, float y)
            {
                long a = BitConversion.SingleToRawInt32Bits(x);
                long b = BitConversion.SingleToRawInt32Bits(y);
                if (a > b)
                    return 1;
                else if (a < b)
                    return -1;

                return 0;
            }

            public int Compare(float x, float y)
            {
                if (x != 0 || y != 0)
                    return x.CompareTo(y);

                return CompareZero(x, y);
            }

            public override bool Equals(float x, float y)
            {
                if (x != 0 || y != 0)
                    return x.Equals(y);

                return CompareZero(x, y) == 0;
            }

            public override int GetHashCode(float obj)
            {
                if (obj != 0)
                    return obj.GetHashCode();

                // Make positive zero and negative zero have differnt hash codes
                return BitConversion.SingleToRawInt32Bits(obj).GetHashCode();
            }
        }
    }
}
