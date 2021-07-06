#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *Unless required by applicable law or agreed to in writing, software
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
    /// Provides comparers that can be used to compare <see cref="ISet{T}"/>
    /// implementations for structural equality using rules similar to those
    /// in the JDK's AbstractSet class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public abstract class SetEqualityComparer<T> : IEqualityComparer<ISet<T>>, IEqualityComparer
    {
#if FEATURE_TYPEEXTENSIONS_GETTYPEINFO
        private static readonly bool TIsValueType = typeof(T).GetTypeInfo().IsValueType;
#else
        private static readonly bool TIsValueType = typeof(T).IsValueType;

#endif
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

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        internal SetEqualityComparer(StructuralEqualityComparer structuralEqualityComparer)
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
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
        /// This method is similar in behavior to <see cref="System.Collections.Generic.HashSet{T}.SetEquals(IEnumerable{T})"/>,
        /// with the exception that both collections must implement <see cref="ISet{T}"/> in order to be equal.
        /// </summary>
        /// <param name="setA">The first set to compare.</param>
        /// <param name="setB">The second set to compare.</param>
        /// <returns><c>true</c> if the specified sets are equal; otherwise, <c>false</c>.</returns>
        public virtual bool Equals(ISet<T>? setA, ISet<T>? setB)
        {
            if (ReferenceEquals(setA, setB))
                return true;

            if (setA is null)
                return setB is null;
            else if (setB is null)
                return false;

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
        public virtual int GetHashCode(ISet<T>? set)
        {
            if (set is null)
                return 0;

            int hashCode = 0;
            using (var i = set.GetEnumerator())
            {
                while (i.MoveNext())
                    hashCode += getHashCode(i.Current);
            }
            return hashCode;
        }

        /// <summary>
        /// Compares two objects for structural equality using rules similar to those in
        /// the JDK's AbstactSet class. Two sets are considered equal if they contain
        /// the same elements (in any order).
        /// <para/>
        /// This method is similar in behavior to <see cref="System.Collections.Generic.HashSet{T}.SetEquals(IEnumerable{T})"/>,
        /// with the exception that both collections must implement <see cref="ISet{T}"/> in order to be equal.
        /// </summary>
        /// <param name="a">The first set to compare.</param>
        /// <param name="b">The second set to compare.</param>
        /// <returns><c>true</c> if the specified objects both implement <see cref="ISet{T}"/> and contain the same elements; otherwise, <c>false</c>.</returns>
        public new bool Equals(object? a, object? b)
        {
            if (a is ISet<T> setA && b is ISet<T> setB)
                return Equals(setA, setB);
            return false;
        }

        /// <summary>
        /// Returns the hash code for this set. Two sets which are equal must return
        /// the same value. This implementation calculates the hash code by adding
        /// each element's hash code as defined by <see cref="EqualityComparer{T}.Default"/>.
        /// </summary>
        /// <param name="obj">The set to calculate the hash code for.</param>
        /// <returns>The hash code of <paramref name="obj"/>.</returns>
        public int GetHashCode(object? obj)
        {
            if (obj is null)
                return 0;
            if (obj is ISet<T> set)
                return GetHashCode(set);
            return EqualityComparer<object>.Default.GetHashCode(obj);
        }

        /// <summary>
        /// Tries to convert the specified <paramref name="comparer"/> to a strongly typed <see cref="SetEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">The comparer to convert to a <see cref="SetEqualityComparer{T}"/>, if possible.</param>
        /// <param name="equalityComparer">The result <see cref="SetEqualityComparer{T}"/> of the conversion.</param>
        /// <returns><c>true</c> if the conversion was successful; otherwise, <c>false</c>.</returns>
        public static bool TryGetSetEqualityComparer(IEqualityComparer comparer, [MaybeNullWhen(false)] out SetEqualityComparer<T> equalityComparer)
        {
            // StructuralEqualityComparer is too "dumb" to resolve generic collections.
            // This is done on purpose for performance reasons. Sets
            // must convert the comparison mode to the resolved SetEqualityComparer<T>
            // to prevent StructuralEqualityComparer from needing to use reflection to do it.
            if (comparer is StructuralEqualityComparer seComparer)
            {
                if (seComparer.Equals(StructuralEqualityComparer.Default))
                    equalityComparer = Default;
                else
                    equalityComparer = Aggressive;
                return true;
            }
            else if (comparer is SetEqualityComparer<T> setComparer)
            {
                equalityComparer = setComparer;
                return true;
            }
            equalityComparer = null!;
            return false;
        }

        /// <summary>
        /// Compares two objects for structural equality using rules similar to those in
        /// the JDK's AbstactSet class. Two sets are considered equal when they both contain
        /// the same objects (in any order).
        /// <para/>
        /// Usage Note: This overload can be used in a collection of <see cref="ISet{T}"/> to
        /// implement <see cref="IStructuralEquatable.Equals(object, IEqualityComparer)"/> for the
        /// set.
        /// </summary>
        /// <param name="set">The first object to compare for structural equality.</param>
        /// <param name="other">The other object to compare for structural equality.</param>
        /// <param name="comparer">The comparer that is passed to <see cref="IStructuralEquatable.Equals(object, IEqualityComparer)"/>.</param>
        /// <returns><c>true</c> if the specified sets are equal; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
        public static bool Equals(ISet<T> set, object? other, IEqualityComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if (!(other is ISet<T> otherSet))
                return false;

            if (TryGetSetEqualityComparer(comparer, out SetEqualityComparer<T>? setComparer))
                return setComparer.Equals(set, otherSet);

            // If we got here, we have an unknown comparer type. We assume that it can resolve
            // structural equality of a set and call it directly. This may result in infinite recursion
            // if it cannot.
            return comparer.Equals(set, otherSet);
        }

        /// <summary>
        /// Returns the hash code of the specified <paramref name="set"/>. The hash code is calculated by
        /// taking each nested element's hash code into account.
        /// <para/>
        /// Usage Note: This overload can be used in a collection of <see cref="ISet{T}"/> to
        /// implement <see cref="IStructuralEquatable.GetHashCode(IEqualityComparer)"/> for the
        /// set.
        /// </summary>
        /// <param name="set">The set to calculate the hash code for.</param>
        /// <param name="comparer">The comparer that is passed to <see cref="IStructuralEquatable.GetHashCode(IEqualityComparer)"/>.</param>
        /// <returns>The hash code of <paramref name="set"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
        public static int GetHashCode(ISet<T> set, IEqualityComparer comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            if (TryGetSetEqualityComparer(comparer, out SetEqualityComparer<T>? setComparer))
                return setComparer.GetHashCode(set);

            // If we got here, we have an unknown comparer type. We assume that it can resolve
            // structural equality of a set and call it directly. This may result in infinite recursion
            // if it cannot.
            return comparer.GetHashCode(set);
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
