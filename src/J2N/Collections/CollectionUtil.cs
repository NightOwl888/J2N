using J2N.Collections.Generic;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        public static bool Equals<T>(IList<T> listA, IList<T> listB)
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
        public static bool Equals<T>(ISet<T> setA, ISet<T> setB)
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
        public static bool Equals<TKey, TValue>(IDictionary<TKey, TValue> dictionaryA, IDictionary<TKey, TValue> dictionaryB)
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
        new public static bool Equals(object objA, object objB)
        {
            if (objA is null)
                return objB is null;
            else if (objB is null)
                return false;

            Type tA = objA.GetType();
            Type tB = objB.GetType();
            if (objA is Array arrayA && arrayA.Rank == 1 && objB is Array arrayB && arrayB.Rank == 1)
            {
                Type elementType = tA.GetElementType();
                if (
#if FEATURE_TYPEEXTENSIONS_GETTYPEINFO
                    elementType.GetTypeInfo().IsPrimitive
#else
                    elementType.IsPrimitive
#endif
                    )
                    return ArrayEqualityComparer<object>.GetPrimitiveOneDimensionalArrayEqualityComparer(elementType).Equals(objA, objB);

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
            if (
#if FEATURE_TYPEEXTENSIONS_GETTYPEINFO
                tA.GetTypeInfo().IsGenericType
#else
                tA.IsGenericType
#endif
                )
            {
                bool shouldReturn = false;

                if (tA.ImplementsGenericInterface(typeof(IList<>)))
                {
                    if (!tB.ImplementsGenericInterface(typeof(IList<>)))
                        return false; // type mismatch - must be a list

                    shouldReturn = true;
                }
                else if (tA.ImplementsGenericInterface(typeof(ISet<>)))
                {
                    if (!tB.ImplementsGenericInterface(typeof(ISet<>)))
                        return false; // type mismatch - must be a set

                    shouldReturn = true;
                }
                else if (tA.ImplementsGenericInterface(typeof(IDictionary<,>)))
                {
                    if (!tB.ImplementsGenericInterface(typeof(IDictionary<,>)))
                        return false; // type mismatch - must be a dictionary

                    shouldReturn = true;
                }

                if (shouldReturn)
                {
                    dynamic genericTypeA = Convert.ChangeType(objA, tA);
                    dynamic genericTypeB = Convert.ChangeType(objB, tB);
                    return Equals(genericTypeA, genericTypeB);
                }
            }

            return J2N.Collections.Generic.EqualityComparer<object>.Default.Equals(objA, objB);
        }

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
        public static int GetHashCode<T>(IList<T> list)
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
        public static int GetHashCode<T>(ISet<T> set)
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
        public static int GetHashCode<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
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
        public static int GetHashCode(object obj)
        {
            if (obj == null)
                return 0; // 0 for null

#if FEATURE_TYPEEXTENSIONS_GETTYPEINFO
            TypeInfo t = obj.GetType().GetTypeInfo();
#else
            Type t = obj.GetType();
#endif
            if (obj is Array array && array.Rank == 1)
            {
                Type elementType = t.GetElementType();
                if (
#if FEATURE_TYPEEXTENSIONS_GETTYPEINFO
                    elementType.GetTypeInfo().IsPrimitive
#else
                    elementType.IsPrimitive
#endif
                    )
                    return ArrayEqualityComparer<object>.GetPrimitiveOneDimensionalArrayEqualityComparer(elementType).GetHashCode(obj);

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
            else if (t.IsGenericType
                && (t.ImplementsGenericInterface(typeof(IList<>))
                || t.ImplementsGenericInterface(typeof(ISet<>))
                || t.ImplementsGenericInterface(typeof(IDictionary<,>))))
            {
                dynamic genericType = Convert.ChangeType(obj, obj.GetType());
                return GetHashCode(genericType);
            }
            
            return J2N.Collections.Generic.EqualityComparer<object>.Default.GetHashCode(obj);
        }

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
        public static string ToString<T>(IFormatProvider provider, string format, ICollection<T> collection)
        {
            return string.Format(provider, format ?? SingleFormatArgument, ToString(collection, provider));
        }


        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractCollection
        /// (the default implementation for all sets and lists), plus the ability
        /// to specify culture for formatting of nested numbers and dates. Note that
        /// this overload will change the culture of the current thread.
        /// </summary>
        public static string ToString<T>(ICollection<T> collection, IFormatProvider provider)
        {
            if (collection == null) return "null";
            if (collection.Count == 0)
                return "[]";

            provider ??= StringFormatter.CurrentCulture;

            using (var it = collection.GetEnumerator())
            {
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
        public static string ToString<TKey, TValue>(IFormatProvider provider, string format, IDictionary<TKey, TValue> dictionary)
        {
            return string.Format(provider, format ?? SingleFormatArgument, ToString(dictionary, provider));
        }

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractMap
        /// (the default implementation for all dictionaries), plus the ability
        /// to specify culture for formatting of nested numbers and dates. Note that
        /// this overload will change the culture of the current thread.
        /// </summary>
        public static string ToString<TKey, TValue>(IDictionary<TKey, TValue> dictionary, IFormatProvider provider)
        {
            if (dictionary == null) return "null";
            if (dictionary.Count == 0)
                return "{}";

            provider ??= StringFormatter.CurrentCulture;

            using (var i = dictionary.GetEnumerator())
            {
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
        }

        /// <summary>
        /// This is a helper method that assists with recursively building
        /// a string of the current collection and all nested collections.
        /// </summary>
        public static string ToString(object obj)
        {
            return ToString(obj, StringFormatter.CurrentCulture);
        }

        /// <summary>
        /// This is a helper method that assists with recursively building
        /// a string of the current collection and all nested collections, plus the ability
        /// to specify culture for formatting of nested numbers and dates. Note that
        /// this overload will change the culture of the current thread.
        /// </summary>
        public static string ToString(object obj, IFormatProvider provider)
        {
            if (obj == null) return "null";
            Type t = obj.GetType();
            if (
#if FEATURE_TYPEEXTENSIONS_GETTYPEINFO
                t.GetTypeInfo().IsGenericType
#else
                t.IsGenericType
#endif
                && (t.ImplementsGenericInterface(typeof(ICollection<>)))
                || t.ImplementsGenericInterface(typeof(IDictionary<,>)))
            {
                return ToStringImpl(obj, t, provider);
            }

            return obj.ToString();
        }

        public static string ToStringImpl(object obj, Type type, IFormatProvider provider)
        {
            dynamic genericType = Convert.ChangeType(obj, type);
            return ToString(genericType, provider);
        }
    }
}
