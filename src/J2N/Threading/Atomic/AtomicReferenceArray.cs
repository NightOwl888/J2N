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

using J2N.Collections;
using J2N.Text;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace J2N.Threading.Atomic
{
    /// <summary>
    /// An array of object references in which elements may be updated
    /// atomically.
    /// </summary>
    /// <typeparam name="T">The class type to store in the array.</typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class AtomicReferenceArray<T> : IStructuralFormattable where T : class
    {
        private readonly T?[] array;
#if !FEATURE_VOLATILE
        private static readonly T Comparand = (T)FormatterServices.GetUninitializedObject(typeof(T)); //does not call ctor
#endif

        /// <summary>
        /// Creates a new <see cref="AtomicReferenceArray{T}"/> of given <paramref name="length"/>.
        /// </summary>
        /// <param name="length">The length of the array.</param>
        public AtomicReferenceArray(int length)
        {
            this.array = new T?[length];
        }

        /// <summary>
        /// Creates a new <see cref="AtomicReferenceArray{T}"/> with the same length as, and
        /// all elements copied from, the given array.
        /// </summary>
        /// <param name="array">The array to copy elements from.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        public AtomicReferenceArray(T?[] array)
        {
            if (array is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            int length = array.Length;
            this.array = new T?[length];
            if (length > 0)
            {
                for (int i = 0; i < length; ++i)
                    this.array[i] = array[i];
            }
        }

        /// <summary>
        /// Gets the length of the array.
        /// </summary>
        public int Length => array.Length;

        /// <summary>
        /// Gets or sets the current value at position <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The current value.</returns>
        public T? this[int index]
        {
#if FEATURE_VOLATILE
            get => Volatile.Read(ref array[index]);
#else
            get => Interlocked.CompareExchange(ref array[index], Comparand, Comparand);
#endif
            set => Interlocked.Exchange(ref array[index], value);
        }

        /// <summary>
        /// Atomically sets the element at position <paramref name="index"/> to the given
        /// value and returns the old value.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The previous value.</returns>
        public T? GetAndSet(int index, T? newValue)
        {
            return Interlocked.Exchange(ref array[index], newValue);
        }

        /// <summary>
        /// Atomically sets the element at position <paramref name="index"/> to the given
        /// updated value if the current value <c>==</c> the expected value.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="expect">The expected value.</param>
        /// <param name="update">The new value.</param>
        /// <returns><c>true</c> if successful. A <c>false</c> return value indicates that the actual value
        /// was not equal to the expected value.</returns>
        public bool CompareAndSet(int index, T? expect, T? update)
        {
            return Interlocked.CompareExchange(ref array[index], update, expect) == expect;
        }

        /// <summary>
        /// Returns the <see cref="string"/> representation of the current values of array.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of the current values of array.</returns>
        public override string ToString()
        {
            return ToString(null);
        }

        /// <summary>
        /// Returns the <see cref="string"/> representation of the current values of array using the
        /// specified culture-specific formatting information.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The <see cref="string"/> representation of the current values of array.</returns>
        public virtual string ToString(IFormatProvider? provider)
        {
            if (array.Length == 0)
                return "[]"; //$NON-NLS-1$

            provider ??= StringFormatter.CurrentCulture;
            StringBuilder sb = new StringBuilder(2 + array.Length * 4);
            sb.Append('[');
            T? val = this[0];
            if (val is null)
                sb.Append("null");
            else
                sb.AppendFormat(provider, "{0}", val);
            for (int i = 1; i < array.Length; i++)
            {
                sb.Append(", "); //$NON-NLS-1$
                val = this[i];
                if (val is null)
                    sb.Append("null");
                else
                    sb.AppendFormat(provider, "{0}", val);
            }
            sb.Append(']');
            return sb.ToString();
        }

        string IStructuralFormattable.ToString(string? format, IFormatProvider? provider)
        {
            return ToString(provider);
        }

        string IFormattable.ToString(string? format, IFormatProvider? provider)
        {
            return ToString(provider);
        }
    }
}
