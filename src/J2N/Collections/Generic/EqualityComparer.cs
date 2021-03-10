using System;
using System.Collections.Generic;
#nullable enable

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Provides comparers that use natural equality rules similar to those in Java.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
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
        ///         <see cref="double"/> and <see cref="float"/> are compared for NaN (not a number) equality. All NaN values are considered equal,
        ///         which differs from the default .NET behavior, where NaN is never equal to NaN.
        ///     </description></item>
        ///     <item><description>
        ///         <see cref="string"/> uses culture-insensitive equality comparison using <see cref="StringComparer.Ordinal"/>.
        ///     </description></item>
        /// </list>
        /// </summary>
        public static IEqualityComparer<T> Default { get; } = LoadDefault();

        private static IEqualityComparer<T> LoadDefault()
        {
            Type genericClosingType = typeof(T);

            // Special cases to match Java equality behavior
            if (typeof(double).Equals(genericClosingType))
                return (IEqualityComparer<T>)new DoubleComparer();
            else if (typeof(float).Equals(genericClosingType))
                return (IEqualityComparer<T>)new SingleComparer();
            else if (typeof(string).Equals(genericClosingType))
                return (IEqualityComparer<T>)StringComparer.Ordinal;

            return System.Collections.Generic.EqualityComparer<T>.Default;
        }

        internal class DoubleComparer : System.Collections.Generic.EqualityComparer<double>, IComparer<double>
        {
            /// <summary>
            /// Accounts for signed zero (.NET does not, but Java does)
            /// </summary>
            private int CompareZero(double x, double y)
            {
                long a = BitConversion.DoubleToInt64Bits(x);
                long b = BitConversion.DoubleToInt64Bits(y);
                if (a > b)
                    return 1;
                else if (a < b)
                    return -1;

                return 0;
            }

            public int Compare(double x, double y)
            {
                if (double.IsNaN(x) && double.IsNaN(y))
                    return 0;

                if (x != 0 && y != 0)
                    return x.CompareTo(y);

                return CompareZero(x, y);
            }

            public override bool Equals(double x, double y)
            {
                if (double.IsNaN(x) && double.IsNaN(y))
                    return true;

                if (x != 0 && y != 0)
                    return x.Equals(y);

                return CompareZero(x, y) == 0;
            }

            public override int GetHashCode(double obj)
            {
                if (obj != 0 && !double.IsNaN(obj))
                    return obj.GetHashCode();

                // Make positive zero and negative zero have differnt hash codes,
                // and NaN always have the same hash code
                return BitConversion.DoubleToInt64Bits(obj).GetHashCode();
            }
        }

        internal class SingleComparer : System.Collections.Generic.EqualityComparer<float>, IComparer<float>
        {
            /// <summary>
            /// Accounts for signed zero (.NET does not, but Java does)
            /// </summary>
            private int CompareZero(float x, float y)
            {
                int a = BitConversion.SingleToInt32Bits(x);
                int b = BitConversion.SingleToInt32Bits(y);
                if (a > b)
                    return 1;
                else if (a < b)
                    return -1;

                return 0;
            }

            public int Compare(float x, float y)
            {
                if (float.IsNaN(x) && float.IsNaN(y))
                    return 0;

                if (x != 0 && y != 0)
                    return x.CompareTo(y);

                return CompareZero(x, y);
            }

            public override bool Equals(float x, float y)
            {
                if (float.IsNaN(x) && float.IsNaN(y))
                    return true;

                if (x != 0 && y != 0)
                    return x.Equals(y);

                return CompareZero(x, y) == 0;
            }

            public override int GetHashCode(float obj)
            {
                if (obj != 0 && !float.IsNaN(obj))
                    return obj.GetHashCode();

                // Make positive zero and negative zero have differnt hash codes,
                // and NaN always have the same hash code
                return BitConversion.SingleToInt32Bits(obj).GetHashCode();
            }
        }
    }
}
