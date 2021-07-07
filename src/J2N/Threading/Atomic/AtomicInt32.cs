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
    /// An <see cref="int"/> value that may be updated atomically. An
    /// <see cref="AtomicInt32"/> is used in applications such as atomically
    /// incremented counters, and cannot be used as a replacement for an
    /// <see cref="int"/>. However, this class does
    /// implement implicit conversion to <see cref="long"/>, so it can
    /// be utilized with language features, tools and utilities that deal
    /// with numerical operations.
    /// <para/>
    /// NOTE: This was AtomicInteger in the JDK
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerDisplay("{Value}")]
    public class AtomicInt32 : Number, IEquatable<AtomicInt32>, IEquatable<int>, IFormattable, IConvertible
    {
        private int value;

        /// <summary>
        /// Creates a new <see cref="AtomicInt32"/> with the default inital value, <c>0</c>.
        /// </summary>
        public AtomicInt32()
            : this(0)
        { }

        /// <summary>
        /// Creates a new <see cref="AtomicInt32"/> with the given initial <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The initial value.</param>
        public AtomicInt32(int value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets or sets the current value. Note that these operations can be done
        /// implicitly by setting the <see cref="AtomicInt32"/> to an <see cref="int"/>.
        /// <code>
        /// AtomicInt32 aint = new AtomicInt32(4);
        /// int x = aint;
        /// </code>
        /// </summary>
        /// <remarks>
        /// Properties are inherently not atomic. Operators such as ++ and -- should not
        /// be used on <see cref="Value"/> because they perform both a separate get and a set operation. Instead,
        /// use the atomic methods such as <see cref="IncrementAndGet()"/>, <see cref="GetAndIncrement()"/>
        /// <see cref="DecrementAndGet()"/> and <see cref="GetAndDecrement()"/> to ensure atomicity.
        /// </remarks>
        public int Value // Port Note: This is a replacement for Get() and Set()
        {
            get => Interlocked.CompareExchange(ref value, 0, 0);
            set => Interlocked.Exchange(ref this.value, value);
        }

        /// <summary>
        /// Atomically sets to the given value and returns the old value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>The previous value.</returns>
        public int GetAndSet(int newValue)
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
        public bool CompareAndSet(int expect, int update)
        {
            int rc = Interlocked.CompareExchange(ref value, update, expect);
            return rc == expect;
        }

        /// <summary>
        /// Atomically increments by one the current value.
        /// </summary>
        /// <returns>The previous value, before the increment.</returns>
        public int GetAndIncrement()
        {
            return Interlocked.Increment(ref value) - 1;
        }

        /// <summary>
        /// Atomically decrements by one the current value.
        /// </summary>
        /// <returns>The previous value, before the decrement.</returns>
        public int GetAndDecrement()
        {
            return Interlocked.Decrement(ref value) + 1;
        }

        /// <summary>
        /// Atomically adds the given value to the current value.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>The previous value, before the addition.</returns>
        public int GetAndAdd(int value)
        {
            return Interlocked.Add(ref this.value, value) - value;
        }

        /// <summary>
        /// Atomically increments by one the current value.
        /// </summary>
        /// <returns>The updated value.</returns>
        public int IncrementAndGet()
        {
            return Interlocked.Increment(ref value);
        }

        /// <summary>
        /// Atomically decrements by one the current value.
        /// </summary>
        /// <returns>The updated value.</returns>
        public int DecrementAndGet()
        {
            return Interlocked.Decrement(ref value);
        }

        /// <summary>
        /// Atomically adds the given value to the current value.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>The updated value.</returns>
        public int AddAndGet(int value)
        {
            return Interlocked.Add(ref this.value, value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="AtomicInt32"/> is equal to the current <see cref="AtomicInt32"/>.
        /// </summary>
        /// <param name="other">The <see cref="AtomicInt32"/> to compare with the current <see cref="AtomicInt32"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="AtomicInt32"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(AtomicInt32? other)
        {
            if (other is null)
                return false;
            return Value == other.Value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="int"/> is equal to the current <see cref="AtomicInt32"/>.
        /// </summary>
        /// <param name="other">The <see cref="int"/> to compare with the current <see cref="AtomicInt32"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="AtomicInt32"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(int other)
        {
            return Value == other;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="AtomicInt32"/>.
        /// <para/>
        /// If <paramref name="other"/> is a <see cref="AtomicInt32"/>, the comparison is not done atomically.
        /// </summary>
        /// <param name="other">The <see cref="object"/> to compare with the current <see cref="AtomicInt32"/>.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is equal to the current <see cref="AtomicInt32"/>; otherwise, <c>false</c>.</returns>
        public override bool Equals(object? other)
        {
            if (other is AtomicInt32 ai)
                return Equals(ai);
            if (other is int i)
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
            return J2N.Numerics.Int32.ToString(Value);
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
            return J2N.Numerics.Int32.ToString(Value, format);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation
        /// using the specified culture-specific format information.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="provider"/>.</returns>
        public override string ToString(IFormatProvider? provider)
        {
            return J2N.Numerics.Int32.ToString(Value, provider);
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
            return J2N.Numerics.Int32.ToString(Value, format, provider);
        }

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
            return Value;
        }

        /// <inheritdoc/>
        public override int ToInt32()
        {
            return Value;
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
        public TypeCode GetTypeCode() => TypeCode.Int32;

        bool IConvertible.ToBoolean(IFormatProvider? provider) => Convert.ToBoolean(Value);

        byte IConvertible.ToByte(IFormatProvider? provider) => Convert.ToByte(Value);

        char IConvertible.ToChar(IFormatProvider? provider) => Convert.ToChar(Value);

        DateTime IConvertible.ToDateTime(IFormatProvider? provider) => Convert.ToDateTime(Value);

        decimal IConvertible.ToDecimal(IFormatProvider? provider) => Convert.ToDecimal(Value);

        double IConvertible.ToDouble(IFormatProvider? provider) => Convert.ToDouble(Value);

        short IConvertible.ToInt16(IFormatProvider? provider) => Convert.ToInt16(Value);

        int IConvertible.ToInt32(IFormatProvider? provider) => Value;

        long IConvertible.ToInt64(IFormatProvider? provider) => Convert.ToInt64(Value);

        sbyte IConvertible.ToSByte(IFormatProvider? provider) => Convert.ToSByte(Value);

        float IConvertible.ToSingle(IFormatProvider? provider) => Convert.ToSingle(Value);

        object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => ((IConvertible)Value).ToType(conversionType, provider);

        ushort IConvertible.ToUInt16(IFormatProvider? provider) => Convert.ToUInt16(Value);

        uint IConvertible.ToUInt32(IFormatProvider? provider) => Convert.ToUInt32(Value);

        ulong IConvertible.ToUInt64(IFormatProvider? provider) => Convert.ToUInt64(Value);

        #endregion IConvertible Members

        #region Operator Overrides

        /// <summary>
        /// Implicitly converts an <see cref="AtomicInt32"/> to an <see cref="int"/>.
        /// </summary>
        /// <param name="atomicInt32">The <see cref="AtomicInt32"/> to convert.</param>
        public static implicit operator int(AtomicInt32 atomicInt32)
        {
            return atomicInt32.Value;
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value equality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(AtomicInt32 a1, AtomicInt32 a2)
        {
            return a1.Value == a2.Value;
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value inequality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(AtomicInt32 a1, AtomicInt32 a2)
        {
            return !(a1 == a2);
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value equality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(AtomicInt32 a1, int a2)
        {
            return a1.Value == a2;
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value inequality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(AtomicInt32 a1, int a2)
        {
            return !(a1 == a2);
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value equality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(int a1, AtomicInt32 a2)
        {
            return a1 == a2.Value;
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value inequality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(int a1, AtomicInt32 a2)
        {
            return !(a1 == a2);
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value equality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(AtomicInt32 a1, int? a2)
        {
            return a1.Value == a2.GetValueOrDefault();
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value inequality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(AtomicInt32 a1, int? a2)
        {
            return !(a1 == a2);
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value equality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(int? a1, AtomicInt32 a2)
        {
            return a1.GetValueOrDefault() == a2.Value;
        }

        /// <summary>
        /// Compares <paramref name="a1"/> and <paramref name="a2"/> for value inequality.
        /// </summary>
        /// <param name="a1">The first number.</param>
        /// <param name="a2">The second number.</param>
        /// <returns><c>true</c> if the given numbers are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(int? a1, AtomicInt32 a2)
        {
            return !(a1 == a2);
        }

        #endregion Operator Overrides
    }
}
