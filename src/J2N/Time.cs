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
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace J2N
{
    /// <summary>
    /// Utilities for timing.
    /// </summary>
#if FEATURE_TIME_PUBLIC
    public
#else
    internal
#endif
        static class Time
    {
        /// <summary>
        /// The number of milliseconds in one nanosecond.
        /// </summary>
        public const long MillisecondsPerNanosecond = 1000000L;

        /// <summary>
        /// The number or seconds in one nanosecond.
        /// </summary>
        public const long SecondsPerNanosecond = 1000000000L;

        /// <summary>
        /// The .NET ticks representing January 1, 1970 0:00:00, also known as the "epoch".
        /// </summary>
        private const long UnixEpochTicks = 621355968000000000L;

        /// <summary>
        /// The <see cref="DateTime"/> January 1, 1970 0:00:00 UTC, also known as the "epoch".
        /// </summary>
        public readonly static DateTime UnixEpoch = new DateTime(UnixEpochTicks, DateTimeKind.Utc);

        /// <summary>
        /// Returns the current value of the running framework's high-resolution time source, in nanoseconds.
        /// </summary>
        /// <returns>The current value of the current framework's high resolution time source, in nanoseconds.</returns>
        /// <remarks>
        /// This method can only be used to measure elapsed time and is not related to any other notion of system
        /// or wall-clock time. The value returned represents nanoseconds since some fixed but arbitrary origin
        /// time (perhaps in the future, so values may be negative).
        /// <para/>
        /// This method provides nanosecond precision, but not necessarily nanosecond resolution (that is, how
        /// frequently the value changes) - no guarantees are made except that the resolution is at least as
        /// good as that of <see cref="CurrentTimeMilliseconds()"/>.
        /// <para/>
        /// This method relies on <see cref="System.Diagnostics.Stopwatch"/>, which is the most accurate
        /// timing mechanism in the .NET framework.
        /// <para/>
        /// For example, to measure how long some code takes to execute:
        /// <code>
        /// long startTime = Time.NanoTime();<br/>
        /// // ... the code being measured ...<br/>
        /// long estimatedTime = Time.NanoTime() - startTime;
        /// </code>
        /// To compare two nanoTime values
        /// <code>
        /// long t0 = Time.NanoTime();<br/>
        /// ...<br/>
        /// long t1 = Time.NanoTime();
        /// </code>
        /// one should use <c>t1 - t0 &lt; 0</c>, not <c>t1 &lt; t0</c>, because of the possibility
        /// of numerical overflow.
        /// <para/>
        /// The elapsed time can be converted to milliseconds or seconds by using the <see cref="MillisecondsPerNanosecond"/>
        /// and <see cref="SecondsPerNanosecond"/> constants.
        /// <code>
        /// long t0 = Time.NanoTime();<br/>
        /// ...<br/>
        /// long t1 = Time.NanoTime();<br/>
        /// <br/>
        /// double milliseconds = ((double)t1 - t0) / Time.MillisecondsPerNanosecond;<br/>
        /// double seconds = ((dobule)t1 - t0) / Time.SecondsPerNanosecond;
        /// </code>
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long NanoTime()
        {
            return unchecked((long)(1_000_000_000.0d * Stopwatch.GetTimestamp() / Stopwatch.Frequency));
        }

        /// <summary>
        /// Returns the current time in milliseconds. Note that while the unit of time of the
        /// return value is a millisecond, the granularity of the value depends on the underlying
        /// operating system and may be larger. For example, many operating systems measure time in
        /// units of tens of milliseconds.
        /// <para/>
        /// This value returned is based on <see cref="DateTime.UtcNow"/>, which is not the highest
        /// resolution timer available. For a higher resolution measurment of elapsed time, use <see cref="NanoTime()"/>.
        /// </summary>
        /// <returns>The difference, measured in milliseconds, between the current time and midnight, January 1, 1970 UTC
        /// also known as the "epoch".</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CurrentTimeMilliseconds()
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// Returns the number of milliseconds since January 1, 1970, 00:00:00 UTC represented by <paramref name="dateTime"/>.
        /// <para/>
        /// Usage Note: This method corresponds to Date.getTime() in the JDK.
        /// </summary>
        /// <param name="dateTime">This <see cref="DateTime"/>.</param>
        /// <returns>The number of milliseconds since January 1, 1970, 00:00:00 UTC represented by <paramref name="dateTime"/>.</returns>
        /// <seealso cref="GetTimeSpanSinceUnixEpoch(DateTime)"/>
        /// <seealso cref="UnixEpoch"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long GetMillisecondsSinceUnixEpoch(this DateTime dateTime)
        {
            return (long)(dateTime - UnixEpoch).TotalMilliseconds;
        }

        /// <summary>
        /// Returns the <see cref="TimeSpan"/> since January 1, 1970, 00:00:00 UTC represented by <paramref name="dateTime"/>.
        /// <para/>
        /// Usage Note: This method provides similar functionality to Date.getTime() in the JDK, however,
        /// <see cref="GetMillisecondsSinceUnixEpoch(DateTime)"/> is an exact behavioral match.
        /// </summary>
        /// <param name="dateTime">This <see cref="DateTime"/>.</param>
        /// <returns>The <see cref="TimeSpan"/> since January 1, 1970, 00:00:00 UTC represented by <paramref name="dateTime"/>.</returns>
        /// <seealso cref="GetMillisecondsSinceUnixEpoch(DateTime)"/>
        /// <seealso cref="UnixEpoch"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan GetTimeSpanSinceUnixEpoch(this DateTime dateTime)
        {
            return dateTime - UnixEpoch;
        }
    }
}
