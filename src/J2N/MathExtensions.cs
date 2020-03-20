using J2N.Numerics;
using System;
using System.Runtime.CompilerServices;
#if NET40
    using MethodImplOptions = J2N.Compatibility.MethodImplOptions;
    using MethodImplAttribute = J2N.Compatibility.MethodImplAttribute;
#endif

namespace J2N
{
    /// <summary>
    /// Additions to <see cref="System.Math"/>, implemented as extension methods.
    /// </summary>
    public static class MathExtensions
    {
        /// <summary>
        /// Returns the signum function of the specified <see cref="int"/> value. (The
        /// return value is <c>-1</c> if the specified value is negative; <c>0</c> if the
        /// specified value is zero; and <c>1</c> if the specified value is positive.)
        /// <para/>
        /// This can be useful for testing the results of two <see cref="IComparable{T}.CompareTo(T)"/>
        /// methods against each other, since only the sign is guaranteed to be the same between implementations.
        /// </summary>
        /// <param name="value">The value whose signum has to be computed.</param>
        /// <returns>The signum function of the specified <see cref="int"/> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Signum(this int value)
        {
            // HD, Section 2-7
            return (value >> 31) | ((-value).TripleShift(31));
        }

        /// <summary>
        /// Returns the signum function of the specified <see cref="long"/> value. (The
        /// return value is <c>-1</c> if the specified value is negative; <c>0</c> if the
        /// specified value is zero; and <c>1</c> if the specified value is positive.)
        /// </summary>
        /// <param name="value">The value whose signum has to be computed.</param>
        /// <returns>The signum function of the specified <see cref="long"/> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Signum(this long value)
        {
            // HD, Section 2-7
            return (int)((value >> 63) | ((-value).TripleShift(63)));
        }

        /// <summary>
        /// Converts an angle measured in degrees to an approximately equivalent angle 
        /// measured in radians. The conversion from degrees to radians is generally inexact.
        /// </summary>
        /// <param name="degrees">An angle in degrees to convert to radians</param>
        /// <returns>The value in radians</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadians(this double degrees)
        {
            return degrees / 180 * Math.PI;
        }

        /// <summary>
        /// Converts an angle measured in degrees to an approximately equivalent angle 
        /// measured in radians. The conversion from degrees to radians is generally inexact.
        /// </summary>
        /// <param name="degrees">An angle in degrees to convert to radians</param>
        /// <returns>The value in radians</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ToRadians(this decimal degrees)
        {
            return degrees / 180 * (decimal)Math.PI;
        }

        /// <summary>
        /// Converts an angle measured in degrees to an approximately equivalent angle 
        /// measured in radians. The conversion from degrees to radians is generally inexact.
        /// </summary>
        /// <param name="degrees">An angle in degrees to convert to radians</param>
        /// <returns>The value in radians</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToRadians(this int degrees)
        {
            return ((double)degrees) / 180 * Math.PI;
        }

        /// <summary>
        /// Converts an angle measured in radians to an approximately equivalent angle 
        /// measured in degrees. The conversion from radians to degrees is generally 
        /// inexact; users should not expect Cos((90.0).ToRadians()) to exactly equal 0.0.
        /// </summary>
        /// <param name="radians">An angle in radians to convert to radians</param>
        /// <returns>The value in radians</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDegrees(this double radians)
        {
            return radians * 180 / Math.PI;
        }

        /// <summary>
        /// Converts an angle measured in radians to an approximately equivalent angle 
        /// measured in degrees. The conversion from radians to degrees is generally 
        /// inexact; users should not expect Cos((90.0).ToRadians()) to exactly equal 0.0.
        /// </summary>
        /// <param name="radians">An angle in radians to convert to radians</param>
        /// <returns>The value in radians</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static decimal ToDegrees(this decimal radians)
        {
            return radians * 180 / (decimal)Math.PI;
        }

        /// <summary>
        /// Converts an angle measured in radians to an approximately equivalent angle 
        /// measured in degrees. The conversion from radians to degrees is generally 
        /// inexact; users should not expect Cos((90.0).ToRadians()) to exactly equal 0.0.
        /// </summary>
        /// <param name="radians">An angle in radians to convert to radians</param>
        /// <returns>The value in radians</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ToDegrees(this int radians)
        {
            return ((double)radians) * 180 / Math.PI;
        }
    }
}
