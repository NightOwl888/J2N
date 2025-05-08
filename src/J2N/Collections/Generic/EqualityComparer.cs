#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static J2N.ThrowHelper;

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
        ///         <see cref="double"/>? and <see cref="float"/>? are first compared for <c>null</c> prior to applying the above rules.
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

            if (genericClosingType.IsGenericType && genericClosingType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Special cases to match Java equality behavior
                if (typeof(double?).Equals(genericClosingType))
                    return (IEqualityComparer<T>)NullableDoubleComparer.Default;
                else if (typeof(float?).Equals(genericClosingType))
                    return (IEqualityComparer<T>)NullableSingleComparer.Default;

                return System.Collections.Generic.EqualityComparer<T>.Default;
            }

            // Special cases to match Java equality behavior
            if (typeof(double).Equals(genericClosingType))
                return (IEqualityComparer<T>)DoubleComparer.Default;
            else if (typeof(float).Equals(genericClosingType))
                return (IEqualityComparer<T>)SingleComparer.Default;
            else if (typeof(string).Equals(genericClosingType))
                return (IEqualityComparer<T>)StringComparer.Ordinal;

            return System.Collections.Generic.EqualityComparer<T>.Default;
        }
    }

    internal class NullableDoubleComparer : System.Collections.Generic.EqualityComparer<double?>, IComparer<double?>
    {
        private NullableDoubleComparer() { } // Singleton only

        new public static readonly NullableDoubleComparer Default = new NullableDoubleComparer();

        public int Compare(double? x, double? y)
        {
            if (x.HasValue)
            {
                if (y.HasValue) return DoubleComparer.Default.Compare(x.Value, y.Value);
                return 1;
            }
            if (y.HasValue) return -1;
            return 0;
        }

        public override bool Equals(double? x, double? y)
        {
            if (x.HasValue)
            {
                if (y.HasValue) return DoubleComparer.Default.Equals(x.Value, y.Value);
                return false;
            }
            if (y.HasValue) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode(double? obj) => obj.HasValue ? DoubleComparer.Default.GetHashCode(obj.Value) : 0;

        // Equals method for the comparer itself.
        public override bool Equals(object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    internal class NullableSingleComparer : System.Collections.Generic.EqualityComparer<float?>, IComparer<float?>
    {
        private NullableSingleComparer() { } // Singleton only

        new public static readonly NullableSingleComparer Default = new NullableSingleComparer();

        public int Compare(float? x, float? y)
        {
            if (x.HasValue)
            {
                if (y.HasValue) return SingleComparer.Default.Compare(x.Value, y.Value);
                return 1;
            }
            if (y.HasValue) return -1;
            return 0;
        }

        public override bool Equals(float? x, float? y)
        {
            if (x.HasValue)
            {
                if (y.HasValue) return SingleComparer.Default.Equals(x.Value, y.Value);
                return false;
            }
            if (y.HasValue) return false;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode(float? obj) => obj.HasValue ? SingleComparer.Default.GetHashCode(obj.Value) : 0;

        // Equals method for the comparer itself.
        public override bool Equals(object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    internal class DoubleComparer : System.Collections.Generic.EqualityComparer<double>, IComparer<double>
    {
        private DoubleComparer() { } // Singleton only

        new public static readonly DoubleComparer Default = new DoubleComparer();

        public int Compare(double x, double y)
        {
            // Non-zero, non-NaN checking.
            if (x > y)
            {
                return 1;
            }
            if (y > x)
            {
                return -1;
            }
            if (x == y && 0.0d != x)
            {
                return 0;
            }

            // NaNs are equal to other NaNs and larger than any other double
            if (double.IsNaN(x))
            {
                if (double.IsNaN(y)) return 0;
                return 1;
            }
            else if (double.IsNaN(y))
            {
                return -1;
            }

            // Deal with +0.0 and -0.0
            long d1 = BitConversion.DoubleToRawInt64Bits(x); // NaN already dealt with, so we can use "raw" here
            long d2 = BitConversion.DoubleToRawInt64Bits(y);
            // The below expression is equivalent to:
            // (d1 == d2) ? 0 : (d1 < d2) ? -1 : 1
            return (int)((d1 >> 63) - (d2 >> 63));
        }

        public override bool Equals(double x, double y)
        {
            if (x < y || x > y)
                return false;

            if (0.0d != x && x == y)
                return true;

            if (double.IsNaN(x) && double.IsNaN(y))
                return true;

            // Deal with +0.0 and -0.0
            long d1 = BitConversion.DoubleToRawInt64Bits(x); // NaN already dealt with, so we can use "raw" here
            long d2 = BitConversion.DoubleToRawInt64Bits(y);
            // The below expression is equivalent to:
            // (d1 == d2) ? 0 : (d1 < d2) ? -1 : 1
            return (int)((d1 >> 63) - (d2 >> 63)) == 0;
        }

        public override int GetHashCode(double obj)
        {
            if (obj != 0 && !double.IsNaN(obj))
                return obj.GetHashCode();

            // Make positive zero and negative zero have different hash codes,
            // and NaN always have the same hash code.
            // We intentionlly call the non "raw" overload here to get that behavior.
            return BitConversion.DoubleToInt64Bits(obj).GetHashCode();
        }

        // Equals method for the comparer itself.
        public override bool Equals(object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }

    internal class SingleComparer : System.Collections.Generic.EqualityComparer<float>, IComparer<float>
    {
        private SingleComparer() { } // Singleton only

        new public static readonly SingleComparer Default = new SingleComparer();

        public int Compare(float x, float y)
        {
            // Non-zero, non-NaN checking.
            if (x > y)
            {
                return 1;
            }
            if (y > x)
            {
                return -1;
            }
            if (x == y && 0.0f != x)
            {
                return 0;
            }

            // NaNs are equal to other NaNs and larger than any other float
            if (float.IsNaN(x))
            {
                if (float.IsNaN(y)) return 0;
                return 1;
            }
            else if (float.IsNaN(y))
            {
                return -1;
            }

            // Deal with +0.0 and -0.0
            int f1 = BitConversion.SingleToRawInt32Bits(x); // NaN already dealt with, so we can use "raw" here
            int f2 = BitConversion.SingleToRawInt32Bits(y);
            // The below expression is equivalent to:
            // (f1 == f2) ? 0 : (f1 < f2) ? -1 : 1
            return (f1 >> 31) - (f2 >> 31);
        }

        public override bool Equals(float x, float y)
        {
            if (x < y || x > y)
                return false;

            if (0.0f != x && x == y)
                return true;

            if (float.IsNaN(x) && float.IsNaN(y))
                return true;

            // Deal with +0.0 and -0.0
            int f1 = BitConversion.SingleToRawInt32Bits(x); // NaN already dealt with, so we can use "raw" here
            int f2 = BitConversion.SingleToRawInt32Bits(y);
            // The below expression is equivalent to:
            // (f1 == f2) ? 0 : (f1 < f2) ? -1 : 1
            return (f1 >> 31) - (f2 >> 31) == 0;
        }

        public override int GetHashCode(float obj)
        {
            if (obj != 0 && !float.IsNaN(obj))
                return obj.GetHashCode();

            // Make positive zero and negative zero have different hash codes,
            // and NaN always have the same hash code.
            // We intentionlly call the non "raw" overload here to get that behavior.
            return BitConversion.SingleToInt32Bits(obj).GetHashCode();
        }

        // Equals method for the comparer itself.
        public override bool Equals(object? obj) =>
            obj != null && GetType() == obj.GetType();

        public override int GetHashCode() =>
            GetType().GetHashCode();
    }
}
