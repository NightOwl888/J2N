#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
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

using J2N.Numerics;
using System;
using System.Diagnostics;
using System.Threading;


namespace J2N.Threading.Atomic
{
    /// <summary>
    /// A <see cref="long"/> value that may be updated atomically.
    /// An <see cref="AtomicInt64"/> is used in applications such as atomically
    /// incremented sequence numbers, and cannot be used as a replacement
    /// for a <see cref="System.Int64"/>. However, this class does
    /// implement implicit conversion to <see cref="long"/>, so it can
    /// be utilized with language features, tools and utilities that deal
    /// with numerical operations.
    /// <para/>
    /// NOTE: This was AtomicLong in the JDK
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerDisplay("{Value}")]
    public class AtomicInt64 : Number, IEquatable<AtomicInt64>, IEquatable<long>, IFormattable, IConvertible
    {
        private long value;

        /// <summary>
        /// Creates a new <see cref="AtomicInt64"/> with the default inital value, <c>0</c>.
        /// </summary>
        public AtomicInt64()
            : this(0)
        { }

        /// <summary>
        /// Creates a new <see cref="AtomicInt64"/> with the given initial <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The inital value.</param>
        public AtomicInt64(long value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets or sets the current value. Note that these operations can be done
        /// implicitly by setting the <see cref="AtomicInt64"/> to a <see cref="long"/>.
        /// <code>
        /// AtomicInt64 along = new AtomicInt64(4);
        /// long x = along;
        /// </code>
        /// </summary>
        /// <remarks>
        /// Properties are inherently not atomic. Operators such as ++ and -- should not
        /// be used on <see cref="Value"/> because they perform both a separate get and a set operation. Instead,
        /// use the atomic methods such as <see cref="IncrementAndGet()"/>, <see cref="GetAndIncrement()"/>
        /// <see cref="DecrementAndGet()"/> and <see cref="GetAndDecrement()"/> to ensure atomicity.
        /// </remarks>
        public long Value // Port Note: This is a replacement for Get() and Set()
        {
            get => Interlocked.Read(ref this.value);
            set => Interlocked.Exchange(ref this.value, value);
        }

        /// <summary>
        /// Atomically sets to the given value and returns the old value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>The previous value.</returns>
        public long GetAndSet(int newValue)
        {
            return Interlocked.Exchange(ref value, newValue);
        }

        /// <summary>
        /// Atomically sets the value to the given updated value
        /// if the current value equals the expected value.
        /// </summary>
        /// <param name="expect">The expected value (the comparand).</param>
        /// <param name="update">The new value that will be set if the current value equals the expected value.</param>
        /// <returns><c>true</c> if successful. A <c>false</c> return value indicates that the actual value
        /// was not equal to the expected value.</returns>
        public bool CompareAndSet(long expect, long update)
        {
            long rc = Interlocked.CompareExchange(ref value, update, expect);
            return rc == expect;
        }

        /// <summary>
        /// Atomically increments by one the current value.
        /// </summary>
        /// <returns>The previous value, before the increment.</returns>
        public long GetAndIncrement()
        {
            return Interlocked.Increment(ref value) - 1;
        }

        /// <summary>
        /// Atomically decrements by one the current value.
        /// </summary>
        /// <returns>The previous value, before the decrement.</returns>
        public long GetAndDecrement()
        {
            return Interlocked.Decrement(ref value) + 1;
        }

        /// <summary>
        /// Atomically adds the given value to the current value.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>The previous value, before the addition.</returns>
        public long GetAndAdd(long value)
        {
            return Interlocked.Add(ref this.value, value) - value;
        }

        /// <summary>
        /// Atomically increments by one the current value.
        /// </summary>
        /// <returns>The updated value.</returns>
        public long IncrementAndGet()
        {
            return Interlocked.Increment(ref value);
        }

        /// <summary>
        /// Atomically decrements by one the current value.
        /// </summary>
        /// <returns>The updated value.</returns>
        public long DecrementAndGet()
        {
            return Interlocked.Decrement(ref value);
        }

        /// <summary>
        /// Atomically adds the given value to the current value.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>The updated value.</returns>
        public long AddAndGet(long value)
        {
            return Interlocked.Add(ref this.value, value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="AtomicInt64"/> is equal to the current <see cref="AtomicInt64"/>.
        /// </summary>
        /// <param name="other">The <see cref="AtomicInt64"/> to compare with the current <see cref="AtomicInt64"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="AtomicInt64"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(AtomicInt64? other)
        {
            if (other is null)
                return false;

            return Value == other.Value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="long"/> is equal to the current <see cref="AtomicInt64"/>.
        /// </summary>
        /// <param name="other">The <see cref="long"/> to compare with the current <see cref="AtomicInt64"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="AtomicInt64"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(long other)
        {
            return Value == other;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="AtomicInt64"/>.
        /// <para/>
        /// If <paramref name="other"/> is a <see cref="AtomicInt64"/>, the comparison is not done atomically.
        /// </summary>
        /// <param name="other">The <see cref="object"/> to compare with the current <see cref="AtomicInt64"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="AtomicInt64"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? other)
        {
            if (other is AtomicInt64 ai)
                return Equals(ai);
            if (other is long i)
                return Equals(i);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of the value of this instance, consisting of
        /// a negative sign if the value is negative,
        /// and a sequence of digits ranging from 0 to 9 with no leading zeroes.</returns>
        public override string ToString()
        {
            return J2N.Numerics.Int64.ToString(Value);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation,
        /// using the specified <paramref name="format"/>.
        /// </summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <returns>The string representation of the value of this instance as specified
        /// by <paramref name="format"/>.</returns>
        public override string ToString(string? format)
        {
            return J2N.Numerics.Int64.ToString(Value, format);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation
        /// using the specified culture-specific format information.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="provider"/>.</returns>
        public override string ToString(IFormatProvider? provider)
        {
            return J2N.Numerics.Int64.ToString(Value, provider);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation using the
        /// specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the value of this instance as specified by
        /// <paramref name="format"/> and <paramref name="provider"/>.</returns>
        public override string ToString(string? format, IFormatProvider? provider)
        {
            return J2N.Numerics.Int64.ToString(Value, format, provider);
        }

        #region TryFormat

        /// <summary>
        /// Tries to format <see cref="Value"/> into the provided span of characters.
        /// </summary>
        /// <param name="destination">The span in which to write this instance's value formatted as a span of characters.</param>
        /// <param name="charsWritten">When this method returns, contains the number of characters that were written in
        /// <paramref name="destination"/>.</param>
        /// <param name="format">A span containing the characters that represent a standard or custom format string that
        /// defines the acceptable format for <paramref name="destination"/>.</param>
        /// <param name="provider">An optional object that supplies culture-specific formatting information for
        /// <paramref name="destination"/>.</param>
        /// <returns><c>true</c> if the formatting was successful; otherwise, <c>false</c>.</returns>
        public override bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
        {
            return DotNetNumber.TryFormatInt64(Value, format, provider, destination, out charsWritten);
        }

        #endregion

        #region IConvertible Members

        /// <inheritdoc/>
        public override byte ToByte()
        {
            return (byte)Value;
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public override sbyte ToSByte()
        {
            return (sbyte)Value;
        }

        /// <inheritdoc/>
        public override double ToDouble()
        {
            return Value;
        }

        /// <inheritdoc/>
        public override float ToSingle()
        {
            return (float)Value;
        }

        /// <inheritdoc/>
        public override int ToInt32()
        {
            return (int)Value;
        }

        /// <inheritdoc/>
        public override long ToInt64()
        {
            return Value;
        }

        /// <inheritdoc/>
        public override short ToInt16()
        {
            return (short)Value;
        }

        /// <summary>
        /// Returns the <see cref="TypeCode"/> for value type <see cref="int"/>.
        /// </summary>
        /// <returns></returns>
        public TypeCode GetTypeCode() => ((IConvertible)Value).GetTypeCode();

        bool IConvertible.ToBoolean(IFormatProvider? provider) => Convert.ToBoolean(Value);

        byte IConvertible.ToByte(IFormatProvider? provider) => Convert.ToByte(Value);

        char IConvertible.ToChar(IFormatProvider? provider) => Convert.ToChar(Value);

        DateTime IConvertible.ToDateTime(IFormatProvider? provider) => Convert.ToDateTime(Value);

        decimal IConvertible.ToDecimal(IFormatProvider? provider) => Convert.ToDecimal(Value);

        double IConvertible.ToDouble(IFormatProvider? provider) => Convert.ToDouble(Value);

        short IConvertible.ToInt16(IFormatProvider? provider) => Convert.ToInt16(Value);

        int IConvertible.ToInt32(IFormatProvider? provider) => Convert.ToInt32(Value);

        long IConvertible.ToInt64(IFormatProvider? provider) => Value;

        sbyte IConvertible.ToSByte(IFormatProvider? provider) => Convert.ToSByte(Value);

        float IConvertible.ToSingle(IFormatProvider? provider) => Convert.ToSingle(Value);

        object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => ((IConvertible)Value).ToType(conversionType, provider);

        ushort IConvertible.ToUInt16(IFormatProvider? provider) => Convert.ToUInt16(Value);

        uint IConvertible.ToUInt32(IFormatProvider? provider) => Convert.ToUInt32(Value);

        ulong IConvertible.ToUInt64(IFormatProvider? provider) => Convert.ToUInt64(Value);

        #endregion IConvertible Members

        #region Operator Overrides

        /// <summary>
        /// Implicitly converts an <see cref="AtomicInt64"/> to a <see cref="long"/>.
        /// </summary>
        /// <param name="atomicInt64">The <see cref="AtomicInt64"/> to convert.</param>
        public static implicit operator long(AtomicInt64 atomicInt64)
        {
            return atomicInt64.Value;
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value equality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(AtomicInt64 a1, AtomicInt64 a2)
        {
            return a1.Value == a2.Value;
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value inequality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(AtomicInt64 a1, AtomicInt64 a2)
        {
            return !(a1 == a2);
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value equality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(AtomicInt64 a1, long a2)
        {
            return a1.Value == a2;
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value inequality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(AtomicInt64 a1, long a2)
        {
            return !(a1 == a2);
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value equality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(long a1, AtomicInt64 a2)
        {
            return a1 == a2.Value;
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value inequality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(long a1, AtomicInt64 a2)
        {
            return !(a1 == a2);
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value equality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(AtomicInt64 a1, long? a2)
        {
            return a1.Value == a2.GetValueOrDefault();
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value inequality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(AtomicInt64 a1, long? a2)
        {
            return !(a1 == a2);
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value equality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(long? a1, AtomicInt64 a2)
        {
            return a1.GetValueOrDefault() == a2.Value;
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value inequality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(long? a1, AtomicInt64 a2)
        {
            return !(a1 == a2);
        }

        #endregion Operator Overrides
    }
}
