﻿#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
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

#if FEATURE_RANDOMIZER

using System;

namespace J2N
{
    /// <summary>
    /// This class provides methods that generates pseudo-random numbers of different
    /// types, such as <see cref="int"/>, <see cref="long"/>, <see cref="double"/>, and <see cref="float"/>.
    /// <para/>
    /// Usage Note:
    /// <para/>
    /// This class differs from <see cref="System.Random"/> in the following ways:
    /// <list type="bullet">
    ///     <item><description>It uses the same pseudo-random algorithm that is used in Java, so setting
    ///         the seed to the same value on both platforms produces identical results.</description></item>
    ///     <item><description>The random seed is provided as <see cref="long"/> rather than <see cref="int"/>.</description></item>
    ///     <item><description>The random seed can be set again after the instance is created using <see cref="Seed"/>.
    ///         This sets the instance back to the same state as if it were newly created with that seed value.</description></item>
    ///     <item><description>It provides random values for <see cref="bool"/>, <see cref="int"/>,
    ///         <see cref="long"/>, <see cref="double"/>, and <see cref="float"/> as well as a 
    ///         <see cref="NextGaussian()"/> method.</description></item>
    ///     <item><description>Random number generation is thread-safe.</description></item>
    /// </list>
    /// <para/>
    /// This class differs from <c>java.util.Random</c> in the following ways:
    /// <list type="bullet">
    ///     <item><description>It subclasses <see cref="System.Random"/> (so it can be used interchangably in .NET).</description></item>
    ///     <item><description>It provides the <see cref="Next(int, int)"/> overload.</description></item>
    ///     <item><description>It doesn't use the <see cref="Randomizer"/> instance for thread synchronization, instead it
    ///         exposes its lock object through its <see cref="SyncRoot"/> property.</description></item>
    ///     <item><description>Its initial seed can be read (as well as set) through the <see cref="Seed"/> property.</description></item>
    ///     <item><description>The <c>nextInt()</c> methods were renamed <see cref="Next()"/> and <see cref="Next(int)"/> to
    ///         override the <see cref="System.Random"/> methods.</description></item>
    ///     <item><description>The <c>next(int)</c> protected method was renamed <see cref="NextInt(int)"/>. Keep this in mind when subclassing.</description></item>
    /// </list>
    /// </summary>
#if FEATURE_SERIALIZABLE_RANDOM
    [Serializable]
#endif
    public class Randomizer : System.Random
    {
        private const long multiplier = 0x5deece66dL;

#if FEATURE_SERIALIZABLE_RANDOM
        [NonSerialized]
#else
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Not readonly for serialization")]
#endif
        private object syncRoot = new object(); // Not readonly for serializer

        /// <summary>
        /// The backing field for the user. This is the value the user sets, and
        /// is the value returned from the <see cref="Seed"/> property, however <see cref="internalSeed"/>
        /// is the actual value used to generate random numbers.
        /// </summary>
        internal long seed; // internal for testing

        /// <summary>
        /// The boolean value indicating if the second Gaussian number is available.
        /// </summary>
        internal bool haveNextNextGaussian; // internal for testing

        /// <summary>
        /// It is associated with the internal state of this generator.
        /// </summary>
        internal long internalSeed; // internal for testing

        /// <summary>
        /// The second Gaussian generated number.
        /// </summary>
        internal double nextNextGaussian; // internal for testing

        /// <summary>
        /// Construct a random generator with the current time of day in milliseconds
        /// as the initial state.
        /// </summary>
        /// <seealso cref="Seed"/>
        public Randomizer()
        {
            Seed = Time.CurrentTimeMilliseconds() + GetHashCode();
        }

        /// <summary>
        /// Construct a random generator with the given <paramref name="seed"/> as the
        /// initial state.
        /// </summary>
        /// <param name="seed">The seed that will determine the initial state of this random
        /// number generator.</param>
        /// <seealso cref="Seed"/>
        public Randomizer(long seed)
        {
            Seed = seed;
        }

        /// <summary>
        /// Returns a pseudo-random uniformly distributed <see cref="int"/> value of
        /// the number of bits specified by the argument <paramref name="bits"/> as
        /// described by Donald E. Knuth in <i>The Art of Computer Programming,
        /// Volume 2: Seminumerical Algorithms</i>, section 3.2.1.
        /// <para/>
        /// NOTE: This was next() in Java.
        /// </summary>
        /// <param name="bits">Number of bits of the returned value.</param>
        /// <returns>A pseudo-random generated int number.</returns>
        /// <seealso cref="NextBytes(byte[])"/>
        /// <seealso cref="NextDouble()"/>
        /// <seealso cref="NextSingle()"/>
        /// <seealso cref="Next()"/>
        /// <seealso cref="Next(int)"/>
        /// <seealso cref="Next(int, int)"/>
        /// <seealso cref="NextGaussian()"/>
        /// <seealso cref="NextInt64()"/>
        protected virtual int NextInt(int bits)
        {
            long localSeed = 0;
            lock (syncRoot)
            {
                localSeed = internalSeed = (internalSeed * multiplier + 0xbL) & ((1L << 48) - 1);
            }
            return (int)(localSeed >>> (48 - bits));
        }

        /// <summary>
        /// Returns the next pseudo-random, uniformly distributed <see cref="bool"/> value
        /// generated by this generator.
        /// </summary>
        /// <returns>A pseudo-random, uniformly distributed <see cref="bool"/> value.</returns>
        public virtual bool NextBoolean()
        {
            return NextInt(1) != 0;
        }

        /// <summary>
        /// Modifies the <see cref="byte"/> array by a random sequence of <see cref="byte"/>s generated by this
        /// random number generator.
        /// </summary>
        /// <param name="buffer">Array to contain the new random <see cref="byte"/>s.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="buffer"/> is <c>null</c>.</exception>
        /// <seealso cref="NextInt(int)"/>
        public override void NextBytes(byte[] buffer)
        {
            if (buffer is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.buffer);

            NextBytes(buffer.AsSpan());
        }

        /// <summary>
        /// Modifies the <see cref="Span{Byte}"/>> by a random sequence of <see cref="byte"/>s generated by this
        /// random number generator.
        /// </summary>
        /// <param name="buffer">Array to contain the new random <see cref="byte"/>s.</param>
        /// <seealso cref="NextInt(int)"/>
#if FEATURE_RANDOM_NEXTBYTES_SPAN
        public override void NextBytes(Span<byte> buffer)
#else
        public virtual void NextBytes(Span<byte> buffer)
#endif
        {
            int rand = 0, count = 0, loop = 0;
            while (count < buffer.Length)
            {
                if (loop == 0)
                {
                    rand = Next();
                    loop = 3;
                }
                else
                {
                    loop--;
                }
                buffer[count++] = (byte)rand;
                rand >>= 8;
            }
        }

        /// <summary>
        /// Generates a normally distributed random <see cref="double"/> number between 0.0
        /// inclusively and 1.0 exclusively.
        /// </summary>
        /// <returns>A random <see cref="double"/> in the range [0.0 - 1.0).</returns>
        /// <seealso cref="NextSingle()"/>
        public override double NextDouble()
        {
            return ((((long)NextInt(26) << 27) + NextInt(27)) / (double)(1L << 53));
        }

        /// <summary>
        /// Generates a normally distributed random <see cref="float"/> number between 0.0
        /// inclusively and 1.0 exclusively.
        /// </summary>
        /// <returns>A random <see cref="float"/> number between [0.0 and 1.0).</returns>
        /// <seealso cref="NextDouble()"/>
#if FEATURE_RANDOM_NEXTSINGLE
        public override float NextSingle()
#else
        public virtual float NextSingle()
#endif
        {
            return (NextInt(24) / 16777216f);
        }

        /// <summary>
        /// Pseudo-randomly generates (approximately) a normally distributed
        /// <see cref="double"/> value with mean 0.0 and a standard deviation value
        /// of <c>1.0</c> using the <i>polar method</i> of G. E. P. Box, M.
        /// E. Muller, and G. Marsaglia, as described by Donald E. Knuth in <i>The
        /// Art of Computer Programming, Volume 2: Seminumerical Algorithms</i>,
        /// section 3.4.1, subsection C, algorithm P.
        /// </summary>
        /// <returns>A random <see cref="double"/>.</returns>
        /// <seealso cref="NextDouble()"/>
        public virtual double NextGaussian()
        {
            lock (syncRoot)
            {
                if (haveNextNextGaussian)
                { // if X1 has been returned, return the
                  // second Gaussian
                    haveNextNextGaussian = false;
                    return nextNextGaussian;
                }

                double v1, v2, s;
                do
                {
                    v1 = 2 * NextDouble() - 1; // Generates two independent random
                                               // variables U1, U2
                    v2 = 2 * NextDouble() - 1;
                    s = v1 * v1 + v2 * v2;
                } while (s >= 1);
                double norm = Math.Sqrt(-2 * Math.Log(s) / s);
                nextNextGaussian = v2 * norm; // should that not be norm instead
                                              // of multiplier ?
                haveNextNextGaussian = true;
                return v1 * norm; // should that not be norm instead of multiplier
                                  // ?
            }
        }

        /// <summary>
        /// Generates a non-negative uniformly distributed 32-bit <see cref="int"/> value from
        /// the random number sequence.
        /// <para/>
        /// NOTE: This was nextInt() in Java.
        /// </summary>
        /// <returns>A uniformly distributed <see cref="int"/> value.</returns>
        /// <seealso cref="int.MaxValue"/>
        /// <seealso cref="int.MinValue"/>
        /// <seealso cref="NextInt(int)"/>
        /// <seealso cref="NextInt64()"/>;
        public override int Next()
        {
            return NextInt(32);
        }

        /// <summary>
        /// Returns a new non-negative pseudo-random <see cref="int"/> value which is uniformly distributed
        /// between 0 (inclusively) and the value of <paramref name="maxValue"/> (exclusively).
        /// <para/>
        /// NOTE: This was nextInt(int) in Java.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the range.</param>
        /// <returns>A random <see cref="int"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxValue"/> is less than 0.</exception>
        public override int Next(int maxValue)
        {
            if (maxValue <= 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegativeNonZero(maxValue, ExceptionArgument.maxValue);

            if ((maxValue & -maxValue) == maxValue)
            {
                // Power of two
                return (int)((maxValue * (long)NextInt(31)) >> 31);
            }
            else
            {
                // General case: use rejection sampling to avoid modulo bias
                int bits, candidate;
                int mask = maxValue - 1;
                do
                {
                    bits = NextInt(31);
                    candidate = bits % maxValue;
                } while ((uint)(bits - candidate) + (uint)mask > (uint)int.MaxValue);
                return candidate;
            }
        }

        /// <summary>
        /// Returns a new non-negative pseudo-random <see cref="int"/> value which is uniformly distributed
        /// between <paramref name="minValue"/> (inclusively) and the value of <paramref name="maxValue"/> (exclusively).
        /// <para/>
        /// NOTE: This was nextInt(int, int) in Java. The behavior is binary compatibile with JDK 21+, which is
        /// different than earlier implementations in JDK 17+.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the range.</param>
        /// <param name="maxValue">The exclusive upper bound of the range. <paramref name="maxValue"/>
        /// must be greater than or equal to <paramref name="minValue"/>.</param>
        /// <returns>A random <see cref="int"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
        public override int Next(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
                ThrowHelper.ThrowArgumentOutOfRangeException_Argument_MinMaxValue(ExceptionArgument.minValue, ExceptionArgument.maxValue);

            int candidate = Next();

            int range = maxValue - minValue;
            if (range > 0)
            {
                int mask = range - 1;

                if ((range & mask) == 0)
                {
                    // Range size is a power of two: use bitmask
                    candidate = (candidate & mask) + minValue;
                }
                else
                {
                    // General case: use rejection sampling to avoid modulo bias
                    int bits = candidate >>> 1; // ignore sign bit
                    while (true)
                    {
                        candidate = bits % range;
                        if (bits + mask - candidate >= 0)
                            break;
                        bits = Next() >>> 1; // ignore sign bit
                    }

                    candidate += minValue;
                }
            }
            else
            {
                // This handles very rare edge cases where range overflows (maxValue - minValue < 0)
                while ((uint)(candidate - minValue) >= (uint)(maxValue - minValue))
                {
                    candidate = Next();
                }
            }

            return candidate;
        }

        /// <summary>
        /// Generates a uniformly distributed 64-bit integer value from
        /// the random number sequence.
        /// <para/>
        /// NOTE: This was nextLong() in Java.
        /// </summary>
        /// <returns>64-bit random integer.</returns>
        /// <seealso cref="long.MaxValue"/>
        /// <seealso cref="long.MinValue"/>
        /// <seealso cref="NextInt(int)"/>
        /// <seealso cref="Next()"/>
        /// <seealso cref="Next(int)"/>
        /// <seealso cref="Next(int, int)"/>
#if FEATURE_RANDOM_NEXTINT64
        public override long NextInt64()
#else
        public virtual long NextInt64()
#endif
        {
            return ((long)NextInt(32) << 32) + NextInt(32);
        }

        /// <summary>
        /// Returns a new non-negative pseudo-random <see cref="long"/> value which is uniformly distributed
        /// between 0 (inclusively) and the value of <paramref name="maxValue"/> (exclusively).
        /// <para/>
        /// NOTE: This was nextLong(long) in Java.
        /// </summary>
        /// <param name="maxValue">The exclusive upper bound of the range.</param>
        /// <returns>A random <see cref="long"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxValue"/> is less than 0.</exception>
#if FEATURE_RANDOM_NEXTINT64
        public override long NextInt64(long maxValue)
#else
        public virtual long NextInt64(long maxValue)
#endif
        {
            if (maxValue <= 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegativeNonZero(maxValue, ExceptionArgument.maxValue);

            long mask = maxValue - 1;

            if ((maxValue & mask) == 0L)
            {
                // Power of two: use bitmask
                return NextInt64() & mask;
            }
            else
            {
                // General case: use rejection sampling to avoid modulo bias
                long bits, candidate;
                do
                {
                    bits = NextInt64() >>> 1; // Convert to non-negative value by dropping the sign bit.
                    candidate = bits % maxValue;
                } while ((ulong)(bits - candidate) + (ulong)mask > (ulong)long.MaxValue);

                return candidate;
            }
        }

        /// <summary>
        /// Returns a new non-negative pseudo-random <see cref="long"/> value which is uniformly distributed
        /// between <paramref name="minValue"/> (inclusively) and the value of <paramref name="maxValue"/> (exclusively).
        /// <para/>
        /// NOTE: This was nextLong(long, long) in Java. The behavior is binary compatibile with JDK 21+, which is
        /// different than earlier implementations in JDK 17+.
        /// </summary>
        /// <param name="minValue">The inclusive lower bound of the range.</param>
        /// <param name="maxValue">The exclusive upper bound of the range.</param>
        /// <returns>A random <see cref="long"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
#if FEATURE_RANDOM_NEXTINT64
        public override long NextInt64(long minValue, long maxValue)
#else
        public virtual long NextInt64(long minValue, long maxValue)
#endif
        {
            if (minValue >= maxValue)
                ThrowHelper.ThrowArgumentOutOfRangeException_Argument_MinMaxValue(ExceptionArgument.minValue, ExceptionArgument.maxValue);

            long candidate = NextInt64();

            long range = maxValue - minValue;
            if (range > 0)
            {
                long mask = range - 1;

                if ((range & mask) == 0L)
                {
                    // Range size is a power of two: use bitmask
                    candidate = (candidate & mask) + minValue;
                }
                else
                {
                    // General case: use rejection sampling to avoid modulo bias
                    long bits = candidate >>> 1; // ignore sign bit
                    while (true)
                    {
                        candidate = bits % range;
                        if (bits + mask - candidate >= 0L)
                            break;
                        bits = NextInt64() >>> 1; // ignore sign bit
                    }

                    candidate += minValue;
                }
            }
            else
            {
                // This handles very rare edge cases where range overflows (maxValue - minValue < 0)
                while ((ulong)(candidate - minValue) >= (ulong)(maxValue - minValue))
                {
                    candidate = NextInt64();
                }
            }

            return candidate;
        }

        /// <summary>
        /// Gets or sets the seed using linear congruential formula presented in <i>The
        /// Art of Computer Programming, Volume 2</i>, Section 3.2.1.
        /// <para/>
        /// Although the seed is exposed here as an instance member that doesn't change,
        /// its value reflects the original seed that was set, not the current state of the seed
        /// that is used for the next operation. This allows the initial seed to be read regardless
        /// of the current state of <see cref="Randomizer"/>, so it can be set again to produce repeatable
        /// results.
        /// <para/>
        /// Setting the <see cref="Seed"/> resets the state of <see cref="Randomizer"/> to the same
        /// state as creating a new <see cref="Randomizer"/> instance with the same seed value passed to
        /// its constructor.
        /// <para/>
        /// Note that while <see cref="Randomizer"/> is thread safe, getting and setting
        /// (or using operators) on <see cref="Seed"/> are not atomic. To synchronize,
        /// use the <see cref="SyncRoot"/> property. Example:
        /// <code>
        /// Random random = new Random(123);
        /// lock (random.SyncRoot)
        ///     random.Seed++;
        /// </code>
        /// which is equivalent to
        /// <code>
        /// Random random = new Random(123);
        /// lock (random.SyncRoot)
        ///     random.Seed = random.Seed + 1;
        /// </code>
        /// that is, both operations require a get, an addition, and then a set. So,
        /// the lock is necessary to ensure the 3 operations happen atomically.
        /// </summary>
        /// <seealso cref="Randomizer.Randomizer()"/>
        /// <seealso cref="Randomizer.Randomizer(long)"/>
        /// <seealso cref="Randomizer.SyncRoot"/>
        // J2N: This property setter has a side-effect, but its usage expectation is clear
        // that seed will reset the state of the entire random number generator.
        public long Seed
        {
            get
            {
                lock (syncRoot)
                    return seed;
            }
            set
            {
                lock (syncRoot)
                {
                    this.seed = value;
                    this.internalSeed = (this.seed ^ multiplier) & ((1L << 48) - 1);
                    haveNextNextGaussian = false;
                }
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="Randomizer"/>.
        /// </summary>
        /// <seealso cref="Seed"/>
        public object SyncRoot => syncRoot;


#if FEATURE_SERIALIZABLE_RANDOM
        [System.Runtime.Serialization.OnDeserialized]
        internal void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
        {
            syncRoot = new object();
        }
#endif
    }
}
#endif
