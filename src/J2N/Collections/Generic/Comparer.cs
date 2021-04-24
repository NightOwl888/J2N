using System;
using System.Collections.Generic;


namespace J2N.Collections.Generic
{
    /// <summary>
    /// Provides comparers that use natural equality rules similar to those in Java.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public static class Comparer<T>
    {
        /// <summary>
        /// Provides natural comparison rules similar to those in Java.
        /// <list type="bullet">
        ///     <item><description>
        ///         <see cref="double"/> and <see cref="float"/> are compared for positive zero and negative zero. These are considered
        ///         two separate numbers, as opposed to the default .NET behavior that considers them equal.
        ///     </description></item>
        ///     <item><description>
        ///         <see cref="double"/> and <see cref="float"/> are compared for NaN (not a number). All NaN values are considered equal,
        ///         which differs from the default .NET behavior, where NaN is never equal to NaN.
        ///     </description></item>
        ///     <item><description>
        ///         <see cref="string"/> uses culture-insensitive comparison using <see cref="StringComparer.Ordinal"/>.
        ///     </description></item>
        /// </list>
        /// </summary>
        public static IComparer<T> Default { get; } = LoadDefault();

        private static IComparer<T> LoadDefault()
        {
            Type genericClosingType = typeof(T);

            // Special cases to match Java equality behavior
            if (typeof(double).Equals(genericClosingType))
                return (IComparer<T>)new EqualityComparer<T>.DoubleComparer();
            else if (typeof(float).Equals(genericClosingType))
                return (IComparer<T>)new EqualityComparer<T>.SingleComparer();
            else if (typeof(string).Equals(genericClosingType))
                return (IComparer<T>)StringComparer.Ordinal;

            return System.Collections.Generic.Comparer<T>.Default;
        }
    }
}
