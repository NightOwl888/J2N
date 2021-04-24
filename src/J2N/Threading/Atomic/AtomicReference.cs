using System;
using System.Threading;


namespace J2N.Threading.Atomic
{
#if NET40
    /// <summary>
    /// An object reference that may be updated atomically.
    /// </summary>
#else
    /// <summary>
    /// An object reference that may be updated atomically.
    /// <para/>
    /// Uses <see cref="Volatile"/> to enforce ordering of writes without any explicit locking.
    /// </summary>
#endif
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class AtomicReference<T> where T : class
    {
        private T? value;

        /// <summary>
        /// Creates a new <see cref="AtomicReference{T}"/> with the given initial <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The initial value.</param>
        public AtomicReference(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// Creates a new <see cref="AtomicReference{T}"/> with <c>null</c> initial value.
        /// </summary>
        public AtomicReference()
        {
            value = null;
        }

        /// <summary>
        /// Gets or sets the current value. Note that these operations can be done
        /// implicitly by setting the <see cref="AtomicReference{T}"/> to a variable
        /// of type <typeparamref name="T"/>.
        /// <code>
        /// var someObject = new SomeObject();
        /// AtomicReference&lt;SomeObject&gt; aref = new AtomicReference&lt;SomeObject&gt;(someObject);
        /// SomeObject x = aref;
        /// </code>
        /// </summary>
        public T? Value
        {
#if NET40
            get => value;
            set => this.value = value;
#else
            get => Volatile.Read(ref value);
            set => Volatile.Write(ref this.value, value);
#endif
        }

        /// <summary>
        /// Atomically sets the value to the given updated value
        /// if the current value <c>==</c> the expected value.
        /// </summary>
        /// <param name="expect">The expected value.</param>
        /// <param name="update">The new value.</param>
        /// <returns><c>true</c> if successful. A <c>false</c> return value indicates that the actual value
        /// was not equal to the expected value.</returns>
        public bool CompareAndSet(T? expect, T? update)
        {
            var previous = Interlocked.CompareExchange(ref value, update, expect);
            return ReferenceEquals(previous, expect);
        }

        /// <summary>
        /// Atomically sets the given <paramref name="value"/> and returns the old value.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <returns>The previous value.</returns>
        public T? GetAndSet(T? value)
        {
            return Interlocked.Exchange(ref this.value, value);
        }

        /// <summary>
        /// Returns the <see cref="string"/> representation of the current value.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of the current value.</returns>
        public override string ToString()
        {
            return Value?.ToString() ?? "null";
        }

        #region Operator Overrides

        /// <summary>
        /// Implicitly converts an <see cref="AtomicReference{T}"/> to a <typeparamref name="T"/>.
        /// </summary>
        /// <param name="atomicReference">The <see cref="AtomicReference{T}"/> to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator T?(AtomicReference<T> atomicReference)
        {
            return atomicReference.Value;
        }

        #endregion
    }
}
