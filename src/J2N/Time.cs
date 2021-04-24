using System.Diagnostics;
using System.Runtime.CompilerServices;


namespace J2N
{
    /// <summary>
    /// Utilities for timing.
    /// </summary>
    public static class Time
    {
        /// <summary>
        /// The number of milliseconds in one nanosecond.
        /// </summary>
        public const long MillisecondsPerNanosecond = 1000000;

        /// <summary>
        /// Returns the current value of the running framework's high-resolution time source, in nanoseconds.
        /// <para/>
        /// This method can only be used to measure elapsed time and is not related to any other notion of system
        /// or wall-clock time.The value returned represents nanoseconds since some fixed but arbitrary origin
        /// time (perhaps in the future, so values may be negative).
        /// <para/>
        /// This method provides nanosecond precision, but not necessarily nanosecond resolution (that is, how
        /// frequently the value changes) - no guarantees are made except that the resolution is at least as
        /// good as that of <see cref="CurrentTimeMilliseconds()"/>.
        /// <para/>
        /// This method relies on <see cref="System.Diagnostics.Stopwatch"/>, which is the most accurate
        /// timing mechanism in the .NET framework.
        /// </summary>
        /// <returns>The current value of the current framework's high resolution time source, in nanoseconds.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static long NanoTime()
        {
            return (Stopwatch.GetTimestamp() / Stopwatch.Frequency) * 1000000000;
        }

        /// <summary>
        /// Returns the current time in milliseconds. Note that while the unit of time of the
        /// return value is a millisecond, the granularity of the value depends on the underlying
        /// operating system and may be larger. For example, many operating systems measure time in
        /// units of tens of milliseconds.
        /// <para/>
        /// This method relies on <see cref="System.Diagnostics.Stopwatch"/>, which is the most accurate
        /// timing mechanism in the .NET framework.
        /// </summary>
        /// <returns>The current value of the current framework's high resolution time source, in milliseconds.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static long CurrentTimeMilliseconds()
        {
            return (Stopwatch.GetTimestamp() / Stopwatch.Frequency) * 1000;
        }
    }
}
