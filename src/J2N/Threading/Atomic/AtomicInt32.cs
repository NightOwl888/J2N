using System;
using System.Threading;
#nullable enable

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
    public class AtomicInt32 : IEquatable<AtomicInt32>, IEquatable<int>, IFormattable, IConvertible
    {
        private int _value;

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
            _value = value;
        }

        /// <summary>
        /// Gets or sets the current value. Note that these operations can be done
        /// implicitly by setting the <see cref="AtomicInt32"/> to an <see cref="int"/>.
        /// <code>
        /// AtomicInt32 aint = new AtomicInt32(4);
        /// int x = aint;
        /// </code>
        /// </summary>
        public int Value // Port Note: This is a replacement for Get() and Set()
        {
            get => this._value;  // read operations atomic in 64 bit
            set => Interlocked.Exchange(ref this._value, value);
        }

        /// <summary>
        /// Atomically sets to the given value and returns the old value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>The previous value.</returns>
        public int GetAndSet(int newValue)
        {
            return Interlocked.Exchange(ref _value, newValue);
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
            int rc = Interlocked.CompareExchange(ref _value, update, expect);
            return rc == expect;
        }

        /// <summary>
        /// Atomically increments by one the current value.
        /// </summary>
        /// <returns>The previous value, before the increment.</returns>
        public int GetAndIncrement()
        {
            return Interlocked.Increment(ref _value) - 1;
        }

        /// <summary>
        /// Atomically decrements by one the current value.
        /// </summary>
        /// <returns>The previous value, before the decrement.</returns>
        public int GetAndDecrement()
        {
            return Interlocked.Decrement(ref _value) + 1;
        }

        /// <summary>
        /// Atomically adds the given value to the current value.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>The previous value, before the addition.</returns>
        public int GetAndAdd(int value)
        {
            return Interlocked.Add(ref this._value, value) - value;
        }

        /// <summary>
        /// Atomically increments by one the current value.
        /// </summary>
        /// <returns>The updated value.</returns>
        public int IncrementAndGet()
        {
            return Interlocked.Increment(ref _value);
        }

        /// <summary>
        /// Atomically decrements by one the current value.
        /// </summary>
        /// <returns>The updated value.</returns>
        public int DecrementAndGet()
        {
            return Interlocked.Decrement(ref _value);
        }

        /// <summary>
        /// Atomically adds the given value to the current value.
        /// </summary>
        /// <param name="value">The value to add.</param>
        /// <returns>The updated value.</returns>
        public int AddAndGet(int value)
        {
            return Interlocked.Add(ref this._value, value);
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
            return Value.ToString();
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation,
        /// using the specified <paramref name="format"/>.
        /// </summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <returns>The string representation of the value of this instance as specified
        /// by <paramref name="format"/>.</returns>
        public string ToString(string? format)
        {
            return Value.ToString(format);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation
        /// using the specified culture-specific format information.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the value of this instance as specified by <paramref name="provider"/>.</returns>
        public string ToString(IFormatProvider? provider)
        {
            return Value.ToString(provider);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation using the
        /// specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">A standard or custom numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the value of this instance as specified by
        /// <paramref name="format"/> and <paramref name="provider"/>.</returns>
        public string ToString(string? format, IFormatProvider? provider)
        {
            return Value.ToString(format, provider);
        }

        #region IConvertible Members

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
