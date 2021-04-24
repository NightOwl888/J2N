using J2N.Collections;
using System;
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
    public class AtomicReferenceArray<T> where T : class
    {
        private readonly T[] array;

        /// <summary>
        /// Creates a new <see cref="AtomicReferenceArray{T}"/> of given <paramref name="length"/>.
        /// </summary>
        /// <param name="length">The length of the array.</param>
        public AtomicReferenceArray(int length)
        {
            this.array = new T[length];
        }

        /// <summary>
        /// Creates a new <see cref="AtomicReferenceArray{T}"/> with the same length as, and
        /// all elements copied from, the given array.
        /// </summary>
        /// <param name="array">The array to copy elements from.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        public AtomicReferenceArray(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            int length = array.Length;
            this.array = new T[length];
            if (length > 0)
            {
                int last = length - 1;
                for (int i = 0; i < last; ++i)
                    this.array[i] = array[i];
                // Do the last write as volatile
                T e = array[last];
#if NET40
                this.array[last] = e;
#else
                Volatile.Write(ref this.array[last], e);
#endif
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
#if NET40
            get => array[index];
            set => array[index] = value!;
#else
            get => Volatile.Read(ref array[index]);
            set => Volatile.Write(ref array[index]!, value);
#endif
        }

        /// <summary>
        /// Atomically sets the element at position <paramref name="index"/> to the given
        /// value and returns the old value.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="newValue">The new value.</param>
        /// <returns>The previous value.</returns>
        public T GetAndSet(int index, T newValue)
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
        public bool CompareAndSet(int index, T expect, T update)
        {
            return Interlocked.CompareExchange(ref array[index], update, expect) == expect;
        }

        /// <summary>
        /// Returns the <see cref="string"/> representation of the current values of array.
        /// </summary>
        /// <returns>The <see cref="string"/> representation of the current values of array.</returns>
        public override string ToString()
        {
            return Arrays.ToString(array);
        }
    }
}
