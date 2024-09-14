#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    public abstract class DictionaryEqualityComparer<TKey, TValue> : IEqualityComparer<IDictionary<TKey, TValue>>, IEqualityComparer, IDictionaryEqualityComparer
    {
        private static readonly bool TKeyIsValueType = typeof(TKey).IsValueType;
        private static readonly bool TKeyIsObject = typeof(TKey).Equals(typeof(object));
        private static readonly bool TValueIsValueType = typeof(TValue).IsValueType;
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

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        internal DictionaryEqualityComparer(StructuralEqualityComparer structuralEqualityComparer)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
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
        /// Compares two dictionaries for structural equality using rules similar to those in
        /// the JDK's AbstactMap class. Two dictionaries are considered equal if
        /// both of them contain the same mappings (ignoring order).
        /// </summary>
        /// <param name="dictionaryA">The first dictionary to compare.</param>
        /// <param name="dictionaryB">The second dictionary to compare.</param>
        /// <returns><c>true</c> if both dictionaries contain the same mappings; otherwise, <c>false</c>.</returns>
        public virtual bool Equals(IDictionary<TKey, TValue>? dictionaryA, IDictionary<TKey, TValue>? dictionaryB)
        {
            if (ReferenceEquals(dictionaryA, dictionaryB))
                return true;

            if (dictionaryA is null)
                return dictionaryB is null;
            else if (dictionaryB is null)
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
                        if (!dictionaryA.TryGetValue(keyB, out TValue? valueA) || !valueEquals(valueA, valueB))
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
        public virtual int GetHashCode(IDictionary<TKey, TValue>? dictionary)
        {
            if (dictionary is null)
                return 0;

            int hashCode = 0;
            using (var i = dictionary.GetEnumerator())
            while (i.MoveNext())
                hashCode += getKeyHashCode(i.Current.Key) ^ getValueHashCode(i.Current.Value);

            return hashCode;
        }

        /// <summary>
        /// Compares two dictionaries for structural equality using rules similar to those in
        /// the JDK's AbstactMap class. Two dictionaries are considered equal if
        /// both of them contain the same mappings (ignoring order).
        /// </summary>
        /// <param name="a">The first dictionary to compare.</param>
        /// <param name="b">The second dictionary to compare.</param>
        /// <returns><c>true</c> if both objects implement <see cref="IDictionary{TKey, TValue}"/> and contain the same mappings; otherwise, <c>false</c>.</returns>
        public new bool Equals(object? a, object? b)
        {
            if (a is IDictionary<TKey, TValue> dictionaryA && b is IDictionary<TKey, TValue> dictionaryB)
                return Equals(dictionaryA, dictionaryB);
            return false;
        }

        /// <summary>
        /// Returns the hash code for the specified <paramref name="obj"/>.
        /// <para/>
        /// If the <paramref name="obj"/> argument is a <see cref="IDictionary{TKey, TValue}"/>, 
        /// this implementation iterates over the dictionary getting the hash code
        /// for each element using <see cref="EqualityComparer{T}.Default"/>,
        /// uses a bitwise logical XOR <c>^</c> to combine key and value hash codes, and adds
        /// up the result.
        /// <para/>
        /// If the <paramref name="obj"/> argument is not a <see cref="IDictionary{TKey, TValue}"/>,
        /// the hash code is calculated using <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        /// <param name="obj">The dictionary to calculate the hash code for.</param>
        /// <returns>The hash code for <paramref name="obj"/>.</returns>
        public int GetHashCode(object? obj)
        {
            if (obj is null)
                return 0;
            if (obj is IDictionary<TKey, TValue> dictionary)
                return GetHashCode(dictionary);
            return EqualityComparer<object>.Default.GetHashCode(obj);
        }

        /// <summary>
        /// Tries to convert the specified <paramref name="comparer"/> to a strongly typed <see cref="DictionaryEqualityComparer{TKey, TValue}"/>.
        /// </summary>
        /// <param name="comparer">The comparer to convert to a <see cref="DictionaryEqualityComparer{TKey, TValue}"/>, if possible.</param>
        /// <param name="equalityComparer">The result <see cref="DictionaryEqualityComparer{TKey, TValue}"/> of the conversion.</param>
        /// <returns><c>true</c> if the conversion was successful; otherwise, <c>false</c>.</returns>
        public static bool TryGetDictionaryEqualityComparer(IEqualityComparer comparer, [MaybeNullWhen(false)] out DictionaryEqualityComparer<TKey, TValue> equalityComparer)
        {
            // StructuralEqualityComparer is too "dumb" to resolve generic collections.
            // This is done on purpose for performance reasons. Dictionaries
            // must convert the comparison mode to the resolved DictionaryEqualityComparer<TKey, TValue>
            // to prevent StructuralEqualityComparer from needing to use reflection to do it.
            if (comparer is StructuralEqualityComparer seComparer)
            {
                if (seComparer.Equals(StructuralEqualityComparer.Default))
                    equalityComparer = Default;
                else
                    equalityComparer = Aggressive;
                return true;
            }
            else if (comparer is DictionaryEqualityComparer<TKey, TValue> dictionaryComparer)
            {
                equalityComparer = dictionaryComparer;
                return true;
            }
            equalityComparer = null!;
            return false;
        }

        /// <summary>
        /// Compares two objects for structural equality using rules similar to those in
        /// the JDK's AbstactMap class. Two dictionaries are considered equal when they both contain
        /// the same mappings (in any order).
        /// <para/>
        /// Usage Note: This overload can be used in a collection of <see cref="IDictionary{TKey, TValue}"/> to
        /// implement <see cref="IStructuralEquatable.Equals(object, IEqualityComparer)"/> for the
        /// dictionary.
        /// </summary>
        /// <param name="dictionary">The first object to compare for structural equality.</param>
        /// <param name="other">The other object to compare for structural equality.</param>
        /// <param name="comparer">The comparer that is passed to <see cref="IStructuralEquatable.Equals(object, IEqualityComparer)"/>.</param>
        /// <returns><c>true</c> if the specified dictionaries are equal; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
        public static bool Equals(IDictionary<TKey, TValue> dictionary, object? other, IEqualityComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if (!(other is IDictionary<TKey, TValue> otherDictionary))
                return false;

            if (TryGetDictionaryEqualityComparer(comparer, out DictionaryEqualityComparer<TKey, TValue>? dictionaryComparer))
                return dictionaryComparer.Equals(dictionary, otherDictionary);

            // If we got here, we have an unknown comparer type. We assume that it can resolve
            // structural equality of a set and call it directly. This may result in infinite recursion
            // if it cannot.
            return comparer.Equals(dictionary, otherDictionary);
        }

        /// <summary>
        /// Returns the hash code of the specified <paramref name="dictionary"/>. The hash code is calculated by
        /// taking each nested element's hash code into account.
        /// <para/>
        /// Usage Note: This overload can be used in a collection of <see cref="IDictionary{TKey, TValue}"/> to
        /// implement <see cref="IStructuralEquatable.GetHashCode(IEqualityComparer)"/> for the
        /// dictionary.
        /// </summary>
        /// <param name="dictionary">The dictionary to calculate the hash code for.</param>
        /// <param name="comparer">The comparer that is passed to <see cref="IStructuralEquatable.GetHashCode(IEqualityComparer)"/>.</param>
        /// <returns>The hash code of <paramref name="dictionary"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
        public static int GetHashCode(IDictionary<TKey, TValue> dictionary, IEqualityComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if (TryGetDictionaryEqualityComparer(comparer, out DictionaryEqualityComparer<TKey, TValue>? dictionaryComparer))
                return dictionaryComparer.GetHashCode(dictionary);

            // If we got here, we have an unknown comparer type. We assume that it can resolve
            // structural equality of a set and call it directly. This may result in infinite recursion
            // if it cannot.
            return comparer.GetHashCode(dictionary);
        }

        // Hack so we don't need to know the generic closing types
        bool IDictionaryEqualityComparer.Equals(object dictionary, object other, IEqualityComparer comparer)
        {
            return Equals((dictionary as IDictionary<TKey, TValue>)!, other, comparer);
        }

        int IDictionaryEqualityComparer.GetHashCode(object dictionary, IEqualityComparer comparer)
        {
            return GetHashCode((dictionary as IDictionary<TKey, TValue>)!, comparer);
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

    // A simple interface used to identify a dictionary equality comparer without knowing its
    // generic closing types.
    internal interface IDictionaryEqualityComparer : IEqualityComparer
    {
        bool Equals(object dictionary, object other, IEqualityComparer comparer);

        int GetHashCode(object dictionary, IEqualityComparer comparer);
    }
}
