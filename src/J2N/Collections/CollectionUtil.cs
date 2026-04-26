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

using J2N.Collections.Concurrent;
using J2N.Collections.Generic;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Collections
{
    /// <summary>
    /// Static methods for assisting with making .NET collections check for equality and print
    /// strings the same way they are done in Java.
    /// </summary>
    internal static class CollectionUtil
    {
        private const string SingleFormatArgument = "{0}";

        private static readonly LurchTable<Type, Func<object, object, bool>> equalsCache = new(LurchTableOrder.Access, 256);
        private static readonly LurchTable<Type, Func<object, int>> hashCodeCache = new(LurchTableOrder.Access, 256);
        private static readonly LurchTable<Type, Func<object, IFormatProvider?, string>> toStringCache = new(LurchTableOrder.Access, 256);

        #region Equals

        /// <summary>
        /// The same implementation of Equals from Java's AbstractList
        /// (the default implementation for all lists)
        /// <para/>
        /// This algorithm depends on the order of the items in the list. 
        /// It is recursive and will determine equality based on the values of
        /// all nested collections.
        /// <para/>
        /// Note this operation currently only supports <see cref="IList{T}"/>, <see cref="ISet{T}"/>, 
        /// and <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals<T>(IList<T>? listA, IList<T>? listB)
        {
            return ListEqualityComparer<T>.Aggressive.Equals(listA, listB);
        }

        /// <summary>
        /// The same implementation of Equals from Java's AbstractSet
        /// (the default implementation for all sets)
        /// <para/>
        /// This algoritm does not depend on the order of the items in the set.
        /// It is recursive and will determine equality based on the values of
        /// all nested collections.
        /// <para/>
        /// Note this operation currently only supports <see cref="IList{T}"/>, <see cref="ISet{T}"/>, 
        /// and <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals<T>(ISet<T>? setA, ISet<T>? setB)
        {
            return SetEqualityComparer<T>.Aggressive.Equals(setA, setB);
        }

        /// <summary>
        /// This is the same implemenation of Equals from Java's AbstractMap
        /// (the default implementation of all dictionaries)
        /// <para/>
        /// This algoritm does not depend on the order of the items in the dictionary.
        /// It is recursive and will determine equality based on the values of
        /// all nested collections.
        /// <para/>
        /// Note this operation currently only supports <see cref="IList{T}"/>, <see cref="ISet{T}"/>, 
        /// and <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Equals<TKey, TValue>(IDictionary<TKey, TValue>? dictionaryA, IDictionary<TKey, TValue>? dictionaryB)
        {
            return DictionaryEqualityComparer<TKey, TValue>.Aggressive.Equals(dictionaryA, dictionaryB);
        }

        /// <summary>
        /// A helper method to recursively determine equality based on
        /// the values of the collection and all nested collections.
        /// <para/>
        /// Note this operation currently only supports <see cref="IList{T}"/>, <see cref="ISet{T}"/>, 
        /// and <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        new public static bool Equals(object? objA, object? objB)
        {
            if (objA is null)
                return objB is null;
            else if (objB is null)
                return false;

            Type tA = objA.GetType();
            Type tB = objB.GetType();
            if (objA is Array arrayA && arrayA.Rank == 1 && objB is Array arrayB && arrayB.Rank == 1)
            {
                Type? elementType = tA.GetElementType();
                bool isPrimitive = elementType != null && elementType.IsPrimitive;

                if (isPrimitive)
                    return ArrayEqualityUtil.GetPrimitiveOneDimensionalArrayEqualityComparer(elementType!).Equals(objA, objB);

                var eA = arrayA.GetEnumerator();
                var eB = arrayB.GetEnumerator();
                while (eA.MoveNext() && eB.MoveNext())
                {
                    // Recursively check each element in the array
                    if (!Equals(eA.Current, eB.Current))
                        return false;
                }

                return (!(eA.MoveNext() || eB.MoveNext()));
            }
            else if (objA is IStructuralEquatable seObj)
                return seObj.Equals(objB, StructuralEqualityComparer.Aggressive);

            var listA = GetGenericInterface(tA, typeof(IList<>));
            if (listA != null)
            {
                var listB = GetGenericInterface(tB, typeof(IList<>));
                if (listB == null)
                    return false;

                var dispatcher = equalsCache.GetOrAdd(listA, CreateListEqualsDispatcher);
                return dispatcher(objA, objB);
            }

            var setA = GetGenericInterface(tA, typeof(ISet<>));
            if (setA != null)
            {
                var setB = GetGenericInterface(tB, typeof(ISet<>));
                if (setB == null)
                    return false;

                var dispatcher = equalsCache.GetOrAdd(setA, CreateSetEqualsDispatcher);
                return dispatcher(objA, objB);
            }

            var dictA = GetGenericInterface(tA, typeof(IDictionary<,>));
            if (dictA != null)
            {
                var dictB = GetGenericInterface(tB, typeof(IDictionary<,>));
                if (dictB == null)
                    return false;

                var dispatcher = equalsCache.GetOrAdd(dictA, CreateDictionaryEqualsDispatcher);
                return dispatcher(objA, objB);
            }

            return J2N.Collections.Generic.EqualityComparer<object>.Default.Equals(objA, objB);
        }

        #region Equals Dispatchers

        private static Func<object, object, bool> CreateListEqualsDispatcher(Type listInterface)
        {
            Type elementType = listInterface.GetGenericArguments()[0];

            MethodInfo method = typeof(CollectionUtil)
                .GetMethod(nameof(EqualsListGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(elementType);

            return (Func<object, object, bool>)Delegate.CreateDelegate(typeof(Func<object, object, bool>), method);
        }

        private static bool EqualsListGeneric<T>(object a, object b)
        {
            return Equals((IList<T>)a, (IList<T>)b);
        }

        private static Func<object, object, bool> CreateSetEqualsDispatcher(Type setInterface)
        {
            Type elementType = setInterface.GetGenericArguments()[0];

            MethodInfo method = typeof(CollectionUtil)
                .GetMethod(nameof(EqualsSetGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(elementType);

            return (Func<object, object, bool>)Delegate.CreateDelegate(typeof(Func<object, object, bool>), method);
        }

        private static bool EqualsSetGeneric<T>(object a, object b)
        {
            return Equals((ISet<T>)a, (ISet<T>)b);
        }

        private static Func<object, object, bool> CreateDictionaryEqualsDispatcher(Type dictInterface)
        {
            Type[] args = dictInterface.GetGenericArguments();

            MethodInfo method = typeof(CollectionUtil)
                .GetMethod(nameof(EqualsDictionaryGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(args[0], args[1]);

            return (Func<object, object, bool>)Delegate.CreateDelegate(typeof(Func<object, object, bool>), method);
        }

        private static bool EqualsDictionaryGeneric<TKey, TValue>(object a, object b)
        {
            return Equals((IDictionary<TKey, TValue>)a, (IDictionary<TKey, TValue>)b);
        }

        #endregion Equals Dispatchers

        #endregion Equals

        #region GetHashCode

        /// <summary>
        /// The same implementation of GetHashCode from Java's AbstractList
        /// (the default implementation for all lists).
        /// <para/>
        /// This algorithm depends on the order of the items in the list.
        /// It is recursive and will build the hash code based on the values of
        /// all nested collections.
        /// <para/>
        /// Note this operation currently only supports <see cref="IList{T}"/>, <see cref="ISet{T}"/>, 
        /// and <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode<T>(IList<T>? list)
        {
            return ListEqualityComparer<T>.Aggressive.GetHashCode(list);
        }

        /// <summary>
        /// The same implementation of GetHashCode from Java's AbstractSet
        /// (the default implementation for all sets)
        /// <para/>
        /// This algorithm does not depend on the order of the items in the set.
        /// It is recursive and will build the hash code based on the values of
        /// all nested collections.
        /// <para/>
        /// Note this operation currently only supports <see cref="IList{T}"/>, <see cref="ISet{T}"/>, 
        /// and <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode<T>(ISet<T>? set)
        {
            return SetEqualityComparer<T>.Aggressive.GetHashCode(set);
        }

        /// <summary>
        /// The same implementation of GetHashCode from Java's AbstractMap
        /// (the default implementation for all dictionaries)
        /// <para/>
        /// This algoritm does not depend on the order of the items in the dictionary.
        /// It is recursive and will build the hash code based on the values of
        /// all nested collections.
        /// <para/>
        /// Note this operation currently only supports <see cref="IList{T}"/>, <see cref="ISet{T}"/>, 
        /// and <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode<TKey, TValue>(IDictionary<TKey, TValue>? dictionary)
        {
            return DictionaryEqualityComparer<TKey, TValue>.Aggressive.GetHashCode(dictionary);
        }

        /// <summary>
        /// This method generally assists with the recursive GetHashCode() that
        /// builds a hash code based on all of the values in a collection 
        /// including any nested collections (lists, sets, arrays, and dictionaries).
        /// <para/>
        /// Note this currently only supports <see cref="IList{T}"/>, <see cref="ISet{T}"/>, 
        /// and <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="obj">the object to build the hash code for</param>
        /// <returns>a value that represents the unique state of all of the values and 
        /// nested collection values in the object, provided the main object itself is 
        /// a collection, otherwise calls <see cref="object.GetHashCode()"/> on the 
        /// object that is passed.</returns>
        public static int GetHashCode(object? obj)
        {
            if (obj == null)
                return 0; // 0 for null

            Type t = obj.GetType();
            if (obj is Array array && array.Rank == 1)
            {
                Type? elementType = t.GetElementType();
                bool isPrimitive = elementType != null && elementType.IsPrimitive;
                if (isPrimitive)
                    return ArrayEqualityUtil.GetPrimitiveOneDimensionalArrayEqualityComparer(elementType!).GetHashCode(obj);

                int hashCode = 1, elementHashCode;
                foreach (var element in array)
                {
                    elementHashCode = 0;
                    if (element != null)
                    {
                        // Handle nested arrays.
                        if (element is IStructuralEquatable eStructuralEquatable)
                            elementHashCode = eStructuralEquatable.GetHashCode(StructuralEqualityComparer.Aggressive);

                        elementHashCode = J2N.Collections.Generic.EqualityComparer<object>.Default.GetHashCode(element);
                    }

                    hashCode = 31 * hashCode + elementHashCode;
                }
                return hashCode;
            }
            else if (obj is IStructuralEquatable seObj)
                return seObj.GetHashCode(StructuralEqualityComparer.Aggressive);

            var list = GetGenericInterface(t, typeof(IList<>));
            if (list != null)
            {
                var dispatcher = hashCodeCache.GetOrAdd(list, CreateListHashCodeDispatcher);
                return dispatcher(obj);
            }

            var set = GetGenericInterface(t, typeof(ISet<>));
            if (set != null)
            {
                var dispatcher = hashCodeCache.GetOrAdd(set, CreateSetHashCodeDispatcher);
                return dispatcher(obj);
            }

            var dict = GetGenericInterface(t, typeof(IDictionary<,>));
            if (dict != null)
            {
                var dispatcher = hashCodeCache.GetOrAdd(dict, CreateDictionaryHashCodeDispatcher);
                return dispatcher(obj);
            }

            return J2N.Collections.Generic.EqualityComparer<object>.Default.GetHashCode(obj);
        }

        #region GetHashCode Dispatchers

        private static Func<object, int> CreateListHashCodeDispatcher(Type listInterface)
        {
            Type elementType = listInterface.GetGenericArguments()[0];

            MethodInfo method = typeof(CollectionUtil)
                .GetMethod(nameof(GetHashCodeListGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(elementType);

            return (Func<object, int>)Delegate.CreateDelegate(typeof(Func<object, int>), method);
        }

        private static int GetHashCodeListGeneric<T>(object obj)
        {
            return GetHashCode((IList<T>)obj);
        }

        private static Func<object, int> CreateSetHashCodeDispatcher(Type setInterface)
        {
            Type elementType = setInterface.GetGenericArguments()[0];

            MethodInfo method = typeof(CollectionUtil)
                .GetMethod(nameof(GetHashCodeSetGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(elementType);

            return (Func<object, int>)Delegate.CreateDelegate(typeof(Func<object, int>), method);
        }

        private static int GetHashCodeSetGeneric<T>(object obj)
        {
            return GetHashCode((ISet<T>)obj);
        }

        private static Func<object, int> CreateDictionaryHashCodeDispatcher(Type dictInterface)
        {
            Type[] args = dictInterface.GetGenericArguments();

            MethodInfo method = typeof(CollectionUtil)
                .GetMethod(nameof(GetHashCodeDictionaryGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                .MakeGenericMethod(args[0], args[1]);

            return (Func<object, int>)Delegate.CreateDelegate(typeof(Func<object, int>), method);
        }

        private static int GetHashCodeDictionaryGeneric<TKey, TValue>(object obj)
        {
            return GetHashCode((IDictionary<TKey, TValue>)obj);
        }

        #endregion GetHashCode Dispatchers

        #endregion GetHashCode

        #region ToString

        ///// <summary>
        ///// This is the same implementation of ToString from Java's AbstractCollection
        ///// (the default implementation for all sets and lists)
        ///// </summary>
        //public static string ToString<T>(ICollection<T> collection)
        //{
        //    return ToString<T>(collection, StringFormatter.CurrentCulture);
        //}

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractCollection
        /// (the default implementation for all sets and lists)
        /// <para/>
        /// This overload is intended to be called from within collections to bypass the
        /// reflection/dynamic conversion of working out whether we are a collection type.
        /// </summary>
        public static string ToString<T>(IFormatProvider? provider, string? format, ICollection<T>? collection)
        {
            return string.Format(provider, format ?? SingleFormatArgument, ToString(collection, provider));
        }


        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractCollection
        /// (the default implementation for all sets and lists), plus the ability
        /// to specify culture for formatting of nested numbers and dates.
        /// </summary>
        public static string ToString<T>(ICollection<T>? collection, IFormatProvider? provider)
        {
            if (collection == null) return "null";
            if (collection.Count == 0)
                return "[]";

            provider ??= StringFormatter.CurrentCulture;

            using var it = collection.GetEnumerator();
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            it.MoveNext();
            while (true)
            {
                T e = it.Current;
                sb.Append(object.ReferenceEquals(e, collection) ?
                    "(this Collection)" :
                    (e is IStructuralFormattable formattable ?
                        formattable.ToString(SingleFormatArgument, provider) :
                        string.Format(provider, SingleFormatArgument, e)));
                if (!it.MoveNext())
                {
                    return sb.Append(']').ToString();
                }
                sb.Append(',').Append(' ');
            }
        }

        ///// <summary>
        ///// This is the same implementation of ToString from Java's AbstractMap
        ///// (the default implementation for all dictionaries)
        ///// </summary>
        //public static string ToString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        //{
        //    return ToString<TKey, TValue>(dictionary, StringFormatter.CurrentCulture);
        //}

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractMap
        /// (the default implementation for all dictionaries)
        /// <para/>
        /// This overload is intended to be called from within dictionaries to bypass the
        /// reflection/dynamic conversion of working out whether we are a dictionary type.
        /// </summary>
        public static string ToString<TKey, TValue>(IFormatProvider? provider, string? format, IDictionary<TKey, TValue>? dictionary)
        {
            return string.Format(provider, format ?? SingleFormatArgument, ToString(dictionary, provider));
        }

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractMap
        /// (the default implementation for all dictionaries), plus the ability
        /// to specify culture for formatting of nested numbers and dates. Note that
        /// this overload will change the culture of the current thread.
        /// </summary>
        public static string ToString<TKey, TValue>(IDictionary<TKey, TValue>? dictionary, IFormatProvider? provider)
        {
            if (dictionary == null) return "null";
            if (dictionary.Count == 0)
                return "{}";

            provider ??= StringFormatter.CurrentCulture;

            using var i = dictionary.GetEnumerator();
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            i.MoveNext();
            while (true)
            {
                KeyValuePair<TKey, TValue> e = i.Current;
                TKey key = e.Key;
                TValue value = e.Value;
                sb.Append(ReferenceEquals(key, dictionary) ?
                    "(this Dictionary)" :
                    (key is IStructuralFormattable formattableKey ?
                        formattableKey.ToString(SingleFormatArgument, provider) :
                        string.Format(provider, SingleFormatArgument, key)));
                sb.Append('=');
                sb.Append(ReferenceEquals(value, dictionary) ?
                    "(this Dictionary)" :
                    (value is IStructuralFormattable formattableValue ?
                        formattableValue.ToString(SingleFormatArgument, provider) :
                        string.Format(provider, SingleFormatArgument, value)));
                if (!i.MoveNext())
                {
                    return sb.Append('}').ToString();
                }
                sb.Append(',').Append(' ');
            }
        }

        /// <summary>
        /// This is a helper method that assists with recursively building
        /// a string of the current collection and all nested collections.
        /// </summary>
        public static string ToString(object? obj)
        {
            return ToString(obj, StringFormatter.CurrentCulture);
        }

        /// <summary>
        /// This is a helper method that assists with recursively building
        /// a string of the current collection and all nested collections, plus the ability
        /// to specify culture for formatting of nested numbers and dates.
        /// </summary>
        public static string ToString(object? obj, IFormatProvider? provider)
        {
            if (obj is null) return "null";
            Type t = obj.GetType();
            if (t.IsGenericType && (t.ImplementsGenericInterface(typeof(ICollection<>)))
                || t.ImplementsGenericInterface(typeof(IDictionary<,>)))
            {
                return ToStringImpl(obj, t, provider);
            }

            return obj.ToString()!;
        }

        public static string ToStringImpl(object? obj, Type type, IFormatProvider? provider)
        {
            var dispatcher = toStringCache.GetOrAdd(type, CreateToStringDispatcher);
            return dispatcher(obj!, provider);
        }

        #region ToString Dispatchers

        private static Func<object, IFormatProvider?, string> CreateToStringDispatcher(Type type)
        {
            if (GetGenericInterface(type, typeof(IDictionary<,>)) is Type dict)
            {
                var args = dict.GetGenericArguments();

                MethodInfo method = typeof(CollectionUtil)
                    .GetMethod(nameof(ToStringDictionaryGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(args[0], args[1]);

                return (Func<object, IFormatProvider?, string>)
                    Delegate.CreateDelegate(typeof(Func<object, IFormatProvider?, string>), method);
            }

            if (GetGenericInterface(type, typeof(ICollection<>)) is Type collection)
            {
                var arg = collection.GetGenericArguments()[0];

                MethodInfo method = typeof(CollectionUtil)
                    .GetMethod(nameof(ToStringCollectionGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(arg);

                return (Func<object, IFormatProvider?, string>)
                    Delegate.CreateDelegate(typeof(Func<object, IFormatProvider?, string>), method);
            }

            throw new InvalidOperationException("Unsupported type");
        }

        private static string ToStringCollectionGeneric<T>(object obj, IFormatProvider? provider)
        {
            return ToString((ICollection<T>)obj, provider);
        }

        private static string ToStringDictionaryGeneric<TKey, TValue>(object obj, IFormatProvider? provider)
        {
            return ToString((IDictionary<TKey, TValue>)obj, provider);
        }

        #endregion ToString Dispatchers

        #endregion ToString

        private static Type? GetGenericInterface(Type type, Type openGeneric)
        {
            foreach (var i in type.GetInterfaces())
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric)
                    return i;
            }
            return null;
        }
    }
}
