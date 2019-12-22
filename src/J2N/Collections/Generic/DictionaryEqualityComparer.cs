using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Provides comparers that can be used to compare <see cref="IDictionary{TKey, TValue}"/>
    /// implementations for structural equality using rules similar to those
    /// in the JDK's AbstractMap class.
    /// </summary>
    /// <typeparam name="TKey">The key element type.</typeparam>
    /// <typeparam name="TValue">The value element type.</typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public abstract class DictionaryEqualityComparer<TKey, TValue> : IEqualityComparer<IDictionary<TKey, TValue>>
    {
        private static readonly bool TKeyIsValueType = typeof(TKey).GetTypeInfo().IsValueType;
        private static readonly bool TKeyIsObject = typeof(TKey).Equals(typeof(object));
        private static readonly bool TValueIsValueType = typeof(TValue).GetTypeInfo().IsValueType;
        private static readonly bool TValueIsObject = typeof(TValue).Equals(typeof(object));

        private readonly StructuralEqualityComparer structuralEqualityComparer;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private Func<TKey, int> getKeyHashCode;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private Func<TValue, int> getValueHashCode;

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private Func<TValue, TValue, bool> valueEquals;

        /// <summary>
        /// Gets a <see cref="DictionaryEqualityComparer{TKey, TValue}"/> object that compares
        /// <see cref="IDictionary{TKey, TValue}"/> implementations for structural equality
        /// using rules similar to those in Java. Nested elemements that implement 
        /// <see cref="IStructuralEquatable"/> are also compared.
        /// </summary>
        public static DictionaryEqualityComparer<TKey, TValue> Default { get; } = new DefaultDictionaryEqualityComparer();

        /// <summary>
        /// Gets a <see cref="DictionaryEqualityComparer{TKey, TValue}"/> object that compares
        /// <see cref="IDictionary{TKey, TValue}"/> implementations for structural equality
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
        public static DictionaryEqualityComparer<TKey, TValue> Aggressive { get; } = new AggressiveDictionaryEqualityComparer();

        internal DictionaryEqualityComparer(StructuralEqualityComparer structuralEqualityComparer)
        {
            this.structuralEqualityComparer = structuralEqualityComparer ?? throw new ArgumentNullException(nameof(structuralEqualityComparer));
            LoadEqualityDelegates();
        }

        private void LoadEqualityDelegates()
        {
            this.getKeyHashCode = StructuralEqualityUtil.LoadGetHashCodeDelegate<TKey>(TKeyIsValueType, TKeyIsObject, structuralEqualityComparer);
            this.getValueHashCode = StructuralEqualityUtil.LoadGetHashCodeDelegate<TValue>(TValueIsValueType, TValueIsObject, structuralEqualityComparer);
            this.valueEquals = StructuralEqualityUtil.LoadEqualsDelegate<TValue>(TValueIsValueType, TValueIsObject, structuralEqualityComparer);
        }

        /// <summary>
        /// Compares two lists for structural equality using rules similar to those in
        /// the JDK's AbstactMap class. Two dictionaries are considered equal if
        /// both of them contain the same mappings (ignoring order).
        /// </summary>
        /// <param name="dictionaryA">The first dictionary to compare.</param>
        /// <param name="dictionaryB">The second dictionary to compare.</param>
        /// <returns><c>true</c> if both dictionaries contain the same mappings; otherwise, <c>false</c>.</returns>
        public virtual bool Equals(IDictionary<TKey, TValue> dictionaryA, IDictionary<TKey, TValue> dictionaryB)
        {
            if (ReferenceEquals(dictionaryA, dictionaryB))
                return true;

            if (dictionaryA == null)
                return dictionaryB == null;
            else if (dictionaryB == null)
                return false;

            if (dictionaryA.Count != dictionaryB.Count)
                return false;

            using (var i = dictionaryB.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    KeyValuePair<TKey, TValue> e = i.Current;
                    TKey keyB = e.Key;
                    TValue valueB = e.Value;
                    if (valueB == null)
                    {
                        if (!(dictionaryA.ContainsKey(keyB)))
                            return false;
                    }
                    else
                    {
                        if (!dictionaryA.TryGetValue(keyB, out TValue valueA) || !valueEquals(valueA, valueB))
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the hash code for the specified <paramref name="dictionary"/>.
        /// <para/>
        /// This implementation iterates over the dictionary getting the hash code
        /// for each element using <see cref="EqualityComparer{T}.Default"/>,
        /// uses a bitwise logical XOR <c>^</c> to combine key and value hash codes, and adds
        /// up the result.
        /// </summary>
        /// <param name="dictionary">The dictionary to calculate the hash code for.</param>
        /// <returns>The hash code for <paramref name="dictionary"/>.</returns>
        public virtual int GetHashCode(IDictionary<TKey, TValue> dictionary)
        {
            int hashCode = 0;
            using (var i = dictionary.GetEnumerator())
            while (i.MoveNext())
                hashCode += getKeyHashCode(i.Current.Key) ^ getValueHashCode(i.Current.Value);

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
        internal class DefaultDictionaryEqualityComparer : DictionaryEqualityComparer<TKey, TValue>
        {
            public DefaultDictionaryEqualityComparer()
                : base(StructuralEqualityComparer.Default)
            { }
        }

#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class AggressiveDictionaryEqualityComparer : DictionaryEqualityComparer<TKey, TValue>
        {
            public AggressiveDictionaryEqualityComparer()
                : base(StructuralEqualityComparer.Aggressive)
            { }
        }
    }
}
