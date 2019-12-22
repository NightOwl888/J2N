using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Provides comparers that can be used to compare <see cref="ISet{T}"/>
    /// implementations for structural equality using rules similar to those
    /// in the JDK's AbstractSet class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public abstract class SetEqualityComparer<T> : IEqualityComparer<ISet<T>>
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
        /// Gets a <see cref="SetEqualityComparer{T}"/> object that compares
        /// <see cref="ISet{T}"/> implementations for structural equality
        /// using rules similar to those in Java. Nested elemements that implement 
        /// <see cref="IStructuralEquatable"/> are also compared.
        /// </summary>
        public static SetEqualityComparer<T> Default { get; } = new DefaultSetEqualityComparer();

        /// <summary>
        /// Gets a <see cref="SetEqualityComparer{T}"/> object that compares
        /// <see cref="ISet{T}"/> implementations for structural equality
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
        public static SetEqualityComparer<T> Aggressive { get; } = new AggressiveSetEqualityComparer();

        internal SetEqualityComparer(StructuralEqualityComparer structuralEqualityComparer)
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
        /// Compares two sets for structural equality using rules similar to those in
        /// the JDK's AbstactSet class. Two sets are considered equal if they contain
        /// the same elements (in any order).
        /// <para/>
        /// This method is similar in behavior to <see cref="HashSet{T}.SetEquals(IEnumerable{T})"/>.
        /// </summary>
        /// <param name="setA">The first set to compare.</param>
        /// <param name="setB">The second set to compare.</param>
        /// <returns>True if the </returns>
        public virtual bool Equals(ISet<T> setA, ISet<T> setB)
        {
            if (ReferenceEquals(setA, setB))
                return true;

            if (!TIsValueType)
            {
                if (setA == null)
                    return setB == null;
                else if (setB == null)
                    return false;
            }

            if (setA.Count != setB.Count)
                return false;

            // same operation as containsAll()
            foreach (T eB in setB)
            {
                bool contains = false;
                foreach (T eA in setA)
                {
                    if (equals(eA, eB))
                    {
                        contains = true;
                        break;
                    }
                }
                if (!contains)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Returns the hash code for this set. Two sets which are equal must return
        /// the same value. This implementation calculates the hash code by adding
        /// each element's hash code as defined by <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        /// <param name="set">The set to calculate the hash code for.</param>
        /// <returns>The hash code of <paramref name="set"/>.</returns>
        public virtual int GetHashCode(ISet<T> set)
        {
            if (set == null)
                return 0;

            int hashCode = 0;
            using (var i = set.GetEnumerator())
            {
                while (i.MoveNext())
                    hashCode += getHashCode(i.Current);
            }
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
        internal class DefaultSetEqualityComparer : SetEqualityComparer<T>
        {
            public DefaultSetEqualityComparer()
                : base(StructuralEqualityComparer.Default)
            { }
        }

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class AggressiveSetEqualityComparer : SetEqualityComparer<T>
        {
            public AggressiveSetEqualityComparer()
                : base(StructuralEqualityComparer.Aggressive)
            { }
        }
    }
}
