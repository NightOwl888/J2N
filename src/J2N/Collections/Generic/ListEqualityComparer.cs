using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Provides comparers that can be used to compare <see cref="IList{T}"/>
    /// implementations for structural equality using rules similar to those
    /// in the JDK's AbstractList class.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public abstract class ListEqualityComparer<T> : IEqualityComparer<IList<T>>
    {
        private static readonly bool TIsValueType = typeof(T).GetTypeInfo().IsValueType;
        private static readonly bool TIsObject = typeof(T).Equals(typeof(object));

        private readonly StructuralEqualityComparer structuralEqualityComparer;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private Func<T, int> getHashCode;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private Func<T, T, bool> equals;

        /// <summary>
        /// Gets a <see cref="ListEqualityComparer{T}"/> object that compares
        /// <see cref="IList{T}"/> implementations for structural equality
        /// using rules similar to those in Java. Nested elemements that implement 
        /// <see cref="IStructuralEquatable"/> are also compared.
        /// </summary>
        public static ListEqualityComparer<T> Default { get; } = new DefaultListEqualityComparer();

        /// <summary>
        /// Gets a <see cref="ListEqualityComparer{T}"/> object that compares
        /// <see cref="IList{T}"/> implementations for structural equality
        /// using rules similar to those in Java. Nested elemements are also compared.
        /// <para/>
        /// If a nested object implements <see cref="IStructuralEquatable"/>, it will be used
        /// to compare structural equality. If not, a reflection call is made to determine
        /// if the object can be converted to <see cref="IList{T}"/>, <see cref="ISet{T}"/>, or
        /// <see cref="IDictionary{TKey, TValue}"/> and then the object is converted to a <c>dynamic</c>
        /// using <see cref="Convert.ChangeType(object, Type)"/>. The compiler then uses the converted type
        /// to decide which comparison rules to use using method overloading.
        /// <para/>
        /// Usage Note: This comparer can be used to patch standard built-in .NET collections for structural equality,
        /// but it is slower to use built-in .NET collections than ensuring all nested types
        /// implement <see cref="IStructuralEquatable"/>. This mode only supports types that
        /// implement <see cref="IStructuralEquatable"/>, <see cref="IList{T}"/>,
        /// <see cref="ISet{T}"/>, or <see cref="IDictionary{TKey, TValue}"/>. All other types will
        /// be compared using <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        public static ListEqualityComparer<T> Aggressive { get; } = new AggressiveListEqualityComparer();

        internal ListEqualityComparer(StructuralEqualityComparer structuralEqualityComparer)
        {
            this.structuralEqualityComparer = structuralEqualityComparer ?? throw new ArgumentNullException(nameof(structuralEqualityComparer));
            LoadEqualityDelegates();
        }

        private void LoadEqualityDelegates()
        {
            this.getHashCode = StructuralEqualityUtil.LoadGetHashCodeDelegate<T>(TIsValueType, TIsObject, structuralEqualityComparer);
            this.equals = StructuralEqualityUtil.LoadEqualsDelegate<T>(TIsValueType, TIsObject, structuralEqualityComparer);
        }

        /// <summary>
        /// Compares two lists for structural equality using rules similar to those in
        /// the JDK's AbstactList class. Two lists are considered equal when they both contain
        /// the same objects in the same order.
        /// </summary>
        /// <param name="listA">The first list to compare.</param>
        /// <param name="listB">The second list to compare.</param>
        /// <returns><c>true</c> if the specified lists are equal; otherwise, <c>false</c>.</returns>
        public virtual bool Equals(IList<T> listA, IList<T> listB)
        {
            if (ReferenceEquals(listA, listB))
                return true;

            if (!TIsValueType)
            {
                if (listA == null)
                    return listB == null;
                else if (listB == null)
                    return false;
            }

            if (listA.Count != listB.Count)
                return false;

            using (IEnumerator<T> eA = listA.GetEnumerator())
            using (IEnumerator<T> eB = listB.GetEnumerator())
            {
                while (eA.MoveNext() && eB.MoveNext())
                {
                    if (!equals(eA.Current, eB.Current))
                        return false;
                }

                return (!(eA.MoveNext() || eB.MoveNext()));
            }
        }

        /// <summary>
        /// Returns the hash code of the specified <paramref name="list"/>. The hash code is calculated by
        /// taking each nested element's hash code into account.
        /// </summary>
        /// <param name="list">The list to calculate the hash code for.</param>
        /// <returns>The hash code of <paramref name="list"/>.</returns>
        public virtual int GetHashCode(IList<T> list)
        {
            if (list == null)
                return 0;

            int hashCode = 1;
            foreach (T e in list)
                hashCode = (31 * hashCode) + getHashCode(e);

            return hashCode;
        }

#if FEATURE_SERIALIZABLE
        [System.Runtime.Serialization.OnDeserialized]
        internal void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
            => LoadEqualityDelegates();
#endif

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class DefaultListEqualityComparer : ListEqualityComparer<T>
        {
            public DefaultListEqualityComparer()
                : base(StructuralEqualityComparer.Default)
            { }
        }

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class AggressiveListEqualityComparer : ListEqualityComparer<T>
        {
            public AggressiveListEqualityComparer()
                : base(StructuralEqualityComparer.Aggressive)
            { }
        }
    }
}
