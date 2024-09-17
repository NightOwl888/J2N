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

using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Threading;

namespace J2N.Threading.Atomic
{
    /// <summary>
    /// An object reference that may be updated atomically.
    /// <para/>
    /// Uses <see cref="Interlocked"/> to enforce ordering of writes without any explicit locking.
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerDisplay("{Value}")]
    public class AtomicReference<T> where T : class
    {
        private T? value;
#if !FEATURE_VOLATILE
        private static readonly T Comparand = (T)FormatterServices.GetUninitializedObject(typeof(T)); //does not call ctor
#endif

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
#if FEATURE_VOLATILE
            get => Volatile.Read(ref value);
#else
            get => Interlocked.CompareExchange(ref value, Comparand, Comparand);
#endif
            set => Interlocked.Exchange(ref this.value, value);
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
