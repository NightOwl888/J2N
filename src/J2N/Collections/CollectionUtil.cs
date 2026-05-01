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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace J2N.Collections
{
    /// <summary>
    /// Static methods for assisting with making .NET collections check for equality and print
    /// strings the same way they are done in Java.
    /// </summary>
    internal static class CollectionUtil
    {
        private const string SingleFormatArgument = "{0}";
        private const int CharStackBufferSize = 256;

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
        [RequiresDynamicCode("Uses reflection-based structural comparison.")]
        public static bool Equals<T>(IList<T>? listA, IList<T>? listB)
        {
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                ThrowHelper.ThrowPlatformNotSupportedException(ExceptionResource.PlatformNotSupported_DynamicCode);
                return false; // J2N: Important to have this return after the throw to satisfy the compiler, even though it is unreachable.
            }

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
        [RequiresDynamicCode("Uses reflection-based structural comparison.")]
        public static bool Equals<T>(ISet<T>? setA, ISet<T>? setB)
        {
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                ThrowHelper.ThrowPlatformNotSupportedException(ExceptionResource.PlatformNotSupported_DynamicCode);
                return false; // J2N: Important to have this return after the throw to satisfy the compiler, even though it is unreachable.
            }

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
        [RequiresDynamicCode("Uses reflection-based structural comparison.")]
        public static bool Equals<TKey, TValue>(IDictionary<TKey, TValue>? dictionaryA, IDictionary<TKey, TValue>? dictionaryB)
        {
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                ThrowHelper.ThrowPlatformNotSupportedException(ExceptionResource.PlatformNotSupported_DynamicCode);
                return false; // J2N: Important to have this return after the throw to satisfy the compiler, even though it is unreachable.
            }

            return DictionaryEqualityComparer<TKey, TValue>.Aggressive.Equals(dictionaryA, dictionaryB);
        }

        /// <summary>
        /// A helper method to recursively determine equality based on
        /// the values of the collection and all nested collections.
        /// <para/>
        /// Note this operation currently only supports <see cref="IList{T}"/>, <see cref="ISet{T}"/>, 
        /// and <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        [RequiresDynamicCode("Uses reflection-based structural comparison.")]
        new public static bool Equals(object? objA, object? objB)
        {
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                ThrowHelper.ThrowPlatformNotSupportedException(ExceptionResource.PlatformNotSupported_DynamicCode);
                return false; // J2N: Important to have this return after the throw to satisfy the compiler, even though it is unreachable.
            }

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
            {
                return seObj.Equals(objB, StructuralEqualityComparer.Aggressive);
            }

            Type? dictA = GetGenericInterface(tA, typeof(IDictionary<,>));
            if (dictA != null)
            {
                Type? dictB = GetGenericInterface(tB, typeof(IDictionary<,>));
                if (dictB == null)
                    return false;

                return EqualsGenericDispatcher.Dispatch(objA, objB, dictA);
            }

            Type? setA = GetGenericInterface(tA, typeof(ISet<>));
            if (setA != null)
            {
                Type? setB = GetGenericInterface(tB, typeof(ISet<>));
                if (setB == null)
                    return false;

                return EqualsGenericDispatcher.Dispatch(objA, objB, setA);
            }

            Type? listA = GetGenericInterface(tA, typeof(IList<>));
            if (listA != null)
            {
                Type? listB = GetGenericInterface(tB, typeof(IList<>));
                if (listB == null)
                    return false;

                return EqualsGenericDispatcher.Dispatch(objA, objB, listA);
            }

            return J2N.Collections.Generic.EqualityComparer<object>.Default.Equals(objA, objB);
        }

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
        [RequiresDynamicCode("Uses reflection-based structural comparison.")]
        public static int GetHashCode<T>(IList<T>? list)
        {
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                ThrowHelper.ThrowPlatformNotSupportedException(ExceptionResource.PlatformNotSupported_DynamicCode);
                return 0; // J2N: Important to have this return after the throw to satisfy the compiler, even though it is unreachable.
            }

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
        [RequiresDynamicCode("Uses reflection-based structural comparison.")]
        public static int GetHashCode<T>(ISet<T>? set)
        {
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                ThrowHelper.ThrowPlatformNotSupportedException(ExceptionResource.PlatformNotSupported_DynamicCode);
                return 0; // J2N: Important to have this return after the throw to satisfy the compiler, even though it is unreachable.
            }

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
        [RequiresDynamicCode("Uses reflection-based structural comparison.")]
        public static int GetHashCode<TKey, TValue>(IDictionary<TKey, TValue>? dictionary)
        {
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                ThrowHelper.ThrowPlatformNotSupportedException(ExceptionResource.PlatformNotSupported_DynamicCode);
                return 0; // J2N: Important to have this return after the throw to satisfy the compiler, even though it is unreachable.
            }

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
        [RequiresDynamicCode("Uses reflection-based structural comparison.")]
        public static int GetHashCode(object? obj)
        {
            if (!RuntimeFeature.IsDynamicCodeSupported)
            {
                ThrowHelper.ThrowPlatformNotSupportedException(ExceptionResource.PlatformNotSupported_DynamicCode);
                return 0; // J2N: Important to have this return after the throw to satisfy the compiler, even though it is unreachable.
            }

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
            {
                return seObj.GetHashCode(StructuralEqualityComparer.Aggressive);
            }

            Type? dict = GetGenericInterface(t, typeof(IDictionary<,>));
            if (dict != null)
            {
                return GetHashCodeGenericDispatcher.Dispatch(obj, dict);
            }

            Type? set = GetGenericInterface(t, typeof(ISet<>));
            if (set != null)
            {
                return GetHashCodeGenericDispatcher.Dispatch(obj, set);
            }

            Type? list = GetGenericInterface(t, typeof(IList<>));
            if (list != null)
            {
                return GetHashCodeGenericDispatcher.Dispatch(obj, list);
            }

            return J2N.Collections.Generic.EqualityComparer<object>.Default.GetHashCode(obj);
        }

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
            int bufferLength = 2 + collection.Count * 4; // J2N: Borrowed the calculation from Arrays
            using ValueStringBuilder sb = bufferLength <= CharStackBufferSize
                ? new(stackalloc char[CharStackBufferSize])
                : new(bufferLength);
            sb.Append('[');
            it.MoveNext();
            while (true)
            {
                T? e = it.Current;
                sb.Append(ReferenceEquals(e, collection) ?
                    "(this Collection)" :
                    (e is IStructuralFormattable formattable ?
                        formattable.ToString(SingleFormatArgument, provider) :
                        string.Format(provider, SingleFormatArgument, e)));
                if (!it.MoveNext())
                {
                    sb.Append(']');
                    return sb.ToString();
                }
                sb.Append(", ");
            }
        }

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractCollection
        /// (the default implementation for all sets and lists), plus the ability
        /// to specify culture for formatting of nested numbers and dates.
        /// </summary>
        public static string ToStringCollectionNonGeneric(ICollection? collection, IFormatProvider? provider)
        {
            if (collection == null) return "null";
            if (collection.Count == 0)
                return "[]";

            provider ??= StringFormatter.CurrentCulture;

            var it = collection.GetEnumerator();
            int bufferLength = 2 + collection.Count * 4; // J2N: Borrowed the calculation from Arrays
            using ValueStringBuilder sb = bufferLength <= CharStackBufferSize
                 ? new(stackalloc char[CharStackBufferSize])
                 : new(bufferLength);
            sb.Append('[');
            it.MoveNext();
            while (true)
            {
                object? e = it.Current;
                sb.Append(ReferenceEquals(e, collection) ?
                    "(this Collection)" :
                    (e is IStructuralFormattable formattable ?
                        formattable.ToString(SingleFormatArgument, provider) :
                        string.Format(provider, SingleFormatArgument, e)));
                if (!it.MoveNext())
                {
                    sb.Append(']');
                    return sb.ToString();
                }
                sb.Append(", ");
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
        /// to specify culture for formatting of nested numbers and dates.
        /// </summary>
        public static string ToString<TKey, TValue>(IDictionary<TKey, TValue>? dictionary, IFormatProvider? provider)
        {
            if (dictionary == null) return "null";
            if (dictionary.Count == 0)
                return "{}";

            provider ??= StringFormatter.CurrentCulture;

            using var i = dictionary.GetEnumerator();
            int bufferLength = 2 + dictionary.Count * 8; // J2N: Based on the calculation from Arrays
            using ValueStringBuilder sb = bufferLength <= CharStackBufferSize
                 ? new(stackalloc char[CharStackBufferSize])
                 : new(bufferLength);
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
                    sb.Append('}');
                    return sb.ToString();
                }
                sb.Append(", ");
            }
        }

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractMap
        /// (the default implementation for all dictionaries), plus the ability
        /// to specify culture for formatting of nested numbers and dates.
        /// </summary>
        public static string ToStringDictionaryNonGeneric(IDictionary? dictionary, IFormatProvider? provider)
        {
            if (dictionary == null) return "null";
            if (dictionary.Count == 0)
                return "{}";

            provider ??= StringFormatter.CurrentCulture;

            var i = dictionary.GetEnumerator();
            int bufferLength = 2 + dictionary.Count * 8; // J2N: Based on the calculation from Arrays
            using ValueStringBuilder sb = bufferLength <= CharStackBufferSize
                 ? new(stackalloc char[CharStackBufferSize])
                 : new(bufferLength);
            sb.Append('{');
            i.MoveNext();
            while (true)
            {
                DictionaryEntry e = (DictionaryEntry)i.Current;
                object? key = e.Key;
                object? value = e.Value;
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
                    sb.Append('}');
                    return sb.ToString();
                }
                sb.Append(", ");
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
            if (TryFormat(obj, obj.GetType(), provider, out string? result))
                return result;

            return obj.ToString()!;
        }

        [UnconditionalSuppressMessage(
            "ReflectionAnalysis",
            "IL3050",
            Justification = "The call to Reflection is guarded by a check for RuntimeFeature.IsDynamicCodeSupported.")]
        public static bool TryFormat(object obj, Type type, IFormatProvider? provider, [NotNullWhen(true)] out string? result)
        {
            if (RuntimeFeature.IsDynamicCodeSupported)
            {
                Type? dict = GetGenericInterface(type, typeof(IDictionary<,>));
                if (dict != null)
                {
                    result = ToStringGenericDispatcher.Dispatch(obj, dict, provider);
                    return true;
                }
                Type? col = GetGenericInterface(type, typeof(ICollection<>));
                if (col != null)
                {
                    result = ToStringGenericDispatcher.Dispatch(obj, col, provider);
                    return true;
                }
            }
            if (obj is IDictionary dictionary)
            {
                result = ToStringDictionaryNonGeneric(dictionary, provider);
                return true;
            }
            if (obj is ICollection collection)
            {
                result = ToStringCollectionNonGeneric(collection, provider);
                return true;
            }
            result = default;
            return false;
        }

        #endregion ToString

        [RequiresDynamicCode("Reflection-based generic interface discovery.")]
        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2070:Type.GetInterfaces may be trimmed",
            Justification = "Only used when dynamic code is supported; not reachable in trimmed/AOT scenarios.")]
        private static Type? GetGenericInterface(Type type, Type openGeneric)
        {
            foreach (var i in type.GetInterfaces())
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == openGeneric)
                    return i;
            }
            return null;
        }

        #region Nested Static Class: EqualsGenericDispatcher

        [RequiresDynamicCode("Uses Reflection-based generic dispatch.")]
        private static class EqualsGenericDispatcher
        {
            private static readonly LurchTable<Type, Func<object, object, bool>> cache = new(LurchTableOrder.Access, 256);

            public static bool Dispatch(object objA, object objB, Type interfaceType)
            {
                Func<object, object, bool> dispatcher = cache.GetOrAdd(interfaceType, CreateDispatcher);
                return dispatcher(objA, objB);
            }

            private static Func<object, object, bool> CreateDispatcher(Type interfaceType)
            {
                if (interfaceType.GetGenericTypeDefinition() == typeof(ISet<>))
                    return CreateSetEqualsDispatcher(interfaceType);

                if (interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    return CreateDictionaryEqualsDispatcher(interfaceType);

                if (interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                    return CreateListEqualsDispatcher(interfaceType);

                ThrowHelper.ThrowInvalidOperationException_UnexpectedDispatcherType(interfaceType);
                return null!; // Unreachable
            }


            #region Equals Dispatchers

            private static Func<object, object, bool> CreateDictionaryEqualsDispatcher(Type dictInterface)
            {
                Type[] args = dictInterface.GetGenericArguments();

                MethodInfo method = typeof(EqualsGenericDispatcher)
                    .GetMethod(nameof(EqualsDictionaryGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(args[0], args[1]);

                return (Func<object, object, bool>)Delegate.CreateDelegate(typeof(Func<object, object, bool>), method);
            }

            private static bool EqualsDictionaryGeneric<TKey, TValue>(object a, object b)
            {
                return CollectionUtil.Equals((IDictionary<TKey, TValue>)a, (IDictionary<TKey, TValue>)b);
            }

            private static Func<object, object, bool> CreateSetEqualsDispatcher(Type setInterface)
            {
                Type elementType = setInterface.GetGenericArguments()[0];

                MethodInfo method = typeof(EqualsGenericDispatcher)
                    .GetMethod(nameof(EqualsSetGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(elementType);

                return (Func<object, object, bool>)Delegate.CreateDelegate(typeof(Func<object, object, bool>), method);
            }

            private static bool EqualsSetGeneric<T>(object a, object b)
            {
                return CollectionUtil.Equals((ISet<T>)a, (ISet<T>)b);
            }

            private static Func<object, object, bool> CreateListEqualsDispatcher(Type listInterface)
            {
                Type elementType = listInterface.GetGenericArguments()[0];

                MethodInfo method = typeof(EqualsGenericDispatcher)
                    .GetMethod(nameof(EqualsListGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(elementType);

                return (Func<object, object, bool>)Delegate.CreateDelegate(typeof(Func<object, object, bool>), method);
            }

            private static bool EqualsListGeneric<T>(object a, object b)
            {
                return CollectionUtil.Equals((IList<T>)a, (IList<T>)b);
            }

            #endregion Equals Dispatchers
        }

        #endregion Nested Static Class: EqualsGenericDispatcher

        #region Nested Static Class: GetHashCodeGenericDispatcher

        [RequiresDynamicCode("Uses Reflection-based generic dispatch.")]
        private static class GetHashCodeGenericDispatcher
        {
            private static readonly LurchTable<Type, Func<object, int>> cache = new(LurchTableOrder.Access, 256);

            public static int Dispatch(object obj, Type interfaceType)
            {
                Func<object, int> dispatcher = cache.GetOrAdd(interfaceType, CreateDispatcher);
                return dispatcher(obj);
            }

            private static Func<object, int> CreateDispatcher(Type interfaceType)
            {
                if (interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    return CreateDictionaryHashCodeDispatcher(interfaceType);

                if (interfaceType.GetGenericTypeDefinition() == typeof(ISet<>))
                    return CreateSetHashCodeDispatcher(interfaceType);

                if (interfaceType.GetGenericTypeDefinition() == typeof(IList<>))
                    return CreateListHashCodeDispatcher(interfaceType);

                ThrowHelper.ThrowInvalidOperationException_UnexpectedDispatcherType(interfaceType);
                return null!; // Unreachable
            }

            #region GetHashCode Dispatchers

            private static Func<object, int> CreateDictionaryHashCodeDispatcher(Type dictInterface)
            {
                Type[] args = dictInterface.GetGenericArguments();

                MethodInfo method = typeof(GetHashCodeGenericDispatcher)
                    .GetMethod(nameof(GetHashCodeDictionaryGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(args[0], args[1]);

                return (Func<object, int>)Delegate.CreateDelegate(typeof(Func<object, int>), method);
            }

            private static int GetHashCodeDictionaryGeneric<TKey, TValue>(object obj)
            {
                return CollectionUtil.GetHashCode((IDictionary<TKey, TValue>)obj);
            }

            private static Func<object, int> CreateSetHashCodeDispatcher(Type setInterface)
            {
                Type elementType = setInterface.GetGenericArguments()[0];

                MethodInfo method = typeof(GetHashCodeGenericDispatcher)
                    .GetMethod(nameof(GetHashCodeSetGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(elementType);

                return (Func<object, int>)Delegate.CreateDelegate(typeof(Func<object, int>), method);
            }

            private static int GetHashCodeSetGeneric<T>(object obj)
            {
                return CollectionUtil.GetHashCode((ISet<T>)obj);
            }

            private static Func<object, int> CreateListHashCodeDispatcher(Type listInterface)
            {
                Type elementType = listInterface.GetGenericArguments()[0];

                MethodInfo method = typeof(GetHashCodeGenericDispatcher)
                    .GetMethod(nameof(GetHashCodeListGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(elementType);

                return (Func<object, int>)Delegate.CreateDelegate(typeof(Func<object, int>), method);
            }

            private static int GetHashCodeListGeneric<T>(object obj)
            {
                return CollectionUtil.GetHashCode((IList<T>)obj);
            }

            #endregion GetHashCode Dispatchers
        }

        #endregion Nested Static Class: GetHashCodeGenericDispatcher

        #region Nested Static Class: ToStringGenericDispatcher

        [RequiresDynamicCode("Uses Reflection-based generic dispatch.")]
        private static class ToStringGenericDispatcher
        {
            private static readonly LurchTable<Type, Func<object, IFormatProvider?, string>> cache = new(LurchTableOrder.Access, 256);

            public static string Dispatch(object obj, Type interfaceType, IFormatProvider? provider)
            {
                if (!RuntimeFeature.IsDynamicCodeSupported)
                    ThrowHelper.ThrowPlatformNotSupportedException(ExceptionResource.PlatformNotSupported_DynamicCode);

                Func<object, IFormatProvider?, string> dispatcher = cache.GetOrAdd(interfaceType, CreateDispatcher);
                return dispatcher(obj, provider);
            }

            private static Func<object, IFormatProvider?, string> CreateDispatcher(Type interfaceType)
            {
                if (interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    return CreateDictionaryToStringDispatcher(interfaceType);

                if (interfaceType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    return CreateCollectionToStringDispatcher(interfaceType);

                ThrowHelper.ThrowInvalidOperationException_UnexpectedDispatcherType(interfaceType);
                return null!; // Unreachable
            }

            private static Func<object, IFormatProvider?, string> CreateDictionaryToStringDispatcher(Type dictInterface)
            {
                Type[] args = dictInterface.GetGenericArguments();

                MethodInfo method = typeof(ToStringGenericDispatcher)
                    .GetMethod(nameof(ToStringDictionaryGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(args[0], args[1]);

                return (Func<object, IFormatProvider?, string>)Delegate.CreateDelegate(typeof(Func<object, IFormatProvider?, string>), method);
            }

            private static Func<object, IFormatProvider?, string> CreateCollectionToStringDispatcher(Type collectionInterface)
            {
                Type elementType = collectionInterface.GetGenericArguments()[0];

                MethodInfo method = typeof(ToStringGenericDispatcher)
                    .GetMethod(nameof(ToStringCollectionGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(elementType);

                return (Func<object, IFormatProvider?, string>)Delegate.CreateDelegate(typeof(Func<object, IFormatProvider?, string>), method);
            }

            private static string ToStringDictionaryGeneric<TKey, TValue>(object obj, IFormatProvider? provider)
            {
                return CollectionUtil.ToString((IDictionary<TKey, TValue>)obj, provider);
            }

            private static string ToStringCollectionGeneric<T>(object obj, IFormatProvider? provider)
            {
                return CollectionUtil.ToString((ICollection<T>)obj, provider);
            }
        }

        #endregion Nested Static Class: ToStringGenericDispatcher
    }
}
