using J2N.Globalization;
using J2N.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace J2N.Collections
{
    /// <summary>
    /// Static methods for assisting with making .NET collections check for equality and print
    /// strings the same way they are done in Java.
    /// </summary>
    public static class CollectionUtil
    {
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
            if (object.ReferenceEquals(listA, listB))
            {
                return true;
            }

            bool isValueType = typeof(T).GetTypeInfo().IsValueType;
            if (!isValueType)
            {
                if (listA == null)
                {
                    return listB == null;
                }
                else if (listB == null)
                {
                    return false;
                }
            }

            if (listA.Count != listB.Count)
            {
                return false;
            }

            using (IEnumerator<T> eA = listA.GetEnumerator())
            {
                using (IEnumerator<T> eB = listB.GetEnumerator())
                {
                    while (eA.MoveNext() && eB.MoveNext())
                    {
                        T o1 = eA.Current;
                        T o2 = eB.Current;

                        if (isValueType ?
                            !EqualityComparer<T>.Default.Equals(o1, o2) :
                            (!(o1 == null ? o2 == null : Equals(o1, o2))))
                        {
                            return false;
                        }
                    }

                    return (!(eA.MoveNext() || eB.MoveNext()));
                }
            }
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
            if (object.ReferenceEquals(setA, setB))
            {
                return true;
            }

            bool isValueType = typeof(T).GetTypeInfo().IsValueType;
            if (!isValueType)
            {
                if (setA == null)
                {
                    return setB == null;
                }
                else if (setB == null)
                {
                    return false;
                }
            }

            if (setA.Count != setB.Count)
            {
                return false;
            }

            // same operation as containsAll()
            foreach (T eB in setB)
            {
                bool contains = false;
                foreach (T eA in setA)
                {
                    if (isValueType ?
                        EqualityComparer<T>.Default.Equals(eA, eB) :
                        Equals(eA, eB))
                    {
                        contains = true;
                        break;
                    }
                }
                if (!contains)
                {
                    return false;
                }
            }

            return true;
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
            if (object.ReferenceEquals(dictionaryA, dictionaryB))
            {
                return true;
            }

            bool isValueType = typeof(TValue).GetTypeInfo().IsValueType;
            if (!isValueType)
            {
                if (dictionaryA == null)
                {
                    return dictionaryB == null;
                }
                else if (dictionaryB == null)
                {
                    return false;
                }
            }

            if (dictionaryA.Count != dictionaryB.Count)
            {
                return false;
            }

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
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!dictionaryA.TryGetValue(keyB, out TValue valueA) || 
                            (isValueType ? !EqualityComparer<TValue>.Default.Equals(valueA, valueB) : !Equals(valueA, valueB)))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
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
            if (objA == null)
            {
                return objB == null;
            }
            else if (objB == null)
            {
                return false;
            }

            Type tA = objA.GetType();
            Type tB = objB.GetType();
            if (tA.GetTypeInfo().IsGenericType)
            {
                bool shouldReturn = false;

                if (tA.ImplementsGenericInterface(typeof(IList<>)))
                {
                    if (!(tB.GetTypeInfo().IsGenericType && tB.ImplementsGenericInterface(typeof(IList<>))))
                    {
                        return false; // type mismatch - must be a list
                    }
                    shouldReturn = true;
                }
                else if (tA.ImplementsGenericInterface(typeof(ISet<>)))
                {
                    if (!(tB.GetTypeInfo().IsGenericType && tB.ImplementsGenericInterface(typeof(ISet<>))))
                    {
                        return false; // type mismatch - must be a set
                    }
                    shouldReturn = true;
                }
                else if (tA.ImplementsGenericInterface(typeof(IDictionary<,>)))
                {
                    if (!(tB.GetTypeInfo().IsGenericType && tB.ImplementsGenericInterface(typeof(IDictionary<,>))))
                    {
                        return false; // type mismatch - must be a dictionary
                    }
                    shouldReturn = true;
                }

                if (shouldReturn)
                {
                    dynamic genericTypeA = Convert.ChangeType(objA, tA);
                    dynamic genericTypeB = Convert.ChangeType(objB, tB);
                    return Equals(genericTypeA, genericTypeB);
                }
            }

            return objA.Equals(objB);
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
            int hashCode = 1;
            bool isValueType = typeof(T).GetTypeInfo().IsValueType;
            foreach (T e in list)
            {
                hashCode = 31 * hashCode +
                    (isValueType ? EqualityComparer<T>.Default.GetHashCode(e) : (e == null ? 0 : GetHashCode(e)));
            }

            return hashCode;
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
            int h = 0;
            bool isValueType = typeof(T).GetTypeInfo().IsValueType;
            using (var i = set.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    T obj = i.Current;
                    if (isValueType)
                    {
                        h += EqualityComparer<T>.Default.GetHashCode(obj);
                    }
                    else if (obj != null)
                    {
                        h += GetHashCode(obj);
                    }
                }
            }
            return h;
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
            int h = 0;
            bool keyIsValueType = typeof(TKey).GetTypeInfo().IsValueType;
            bool valueIsValueType = typeof(TValue).GetTypeInfo().IsValueType;
            using (var i = dictionary.GetEnumerator())
            {
                while (i.MoveNext())
                {
                    TKey key = i.Current.Key;
                    TValue value = i.Current.Value;
                    int keyHash = (keyIsValueType ? EqualityComparer<TKey>.Default.GetHashCode(key) : (key == null ? int.MaxValue : GetHashCode(key)));
                    int valueHash = (valueIsValueType ? EqualityComparer<TValue>.Default.GetHashCode(value) : (value == null ? int.MaxValue : GetHashCode(value)));
                    h += keyHash ^ valueHash;
                }
            }
            return h;
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
            {
                return 0; // 0 for null
            }

            Type t = obj.GetType();
            if (t.GetTypeInfo().IsGenericType
                && (t.ImplementsGenericInterface(typeof(IList<>))
                || t.ImplementsGenericInterface(typeof(ISet<>))
                || t.ImplementsGenericInterface(typeof(IDictionary<,>))))
            {
                dynamic genericType = Convert.ChangeType(obj, t);
                return GetHashCode(genericType);
            }

            return obj.GetHashCode();
        }

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractCollection
        /// (the default implementation for all sets and lists)
        /// </summary>
        public static string ToString<T>(ICollection<T> collection)
        {
            if (collection == null) return "null";
            if (collection.Count == 0)
            {
                return "[]";
            }

            bool isValueType = typeof(T).GetTypeInfo().IsValueType;
            using (var it = collection.GetEnumerator())
            {
                StringBuilder sb = new StringBuilder();
                sb.Append('[');
                it.MoveNext();
                while (true)
                {
                    T e = it.Current;
                    sb.Append(object.ReferenceEquals(e, collection) ? "(this Collection)" : (isValueType ? string.Format(new StringFormatter(), "{0}", e) : ToString(e)));
                    if (!it.MoveNext())
                    {
                        return sb.Append(']').ToString();
                    }
                    sb.Append(',').Append(' ');
                }
            }
        }

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractCollection
        /// (the default implementation for all sets and lists), plus the ability
        /// to specify culture for formatting of nested numbers and dates. Note that
        /// this overload will change the culture of the current thread.
        /// </summary>
        public static string ToString<T>(ICollection<T> collection, CultureInfo culture)
        {
            using (var context = new CultureContext(culture))
            {
                return ToString(collection);
            }
        }

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractMap
        /// (the default implementation for all dictionaries)
        /// </summary>
        public static string ToString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null) return "null";
            if (dictionary.Count == 0)
            {
                return "{}";
            }

            bool keyIsValueType = typeof(TKey).GetTypeInfo().IsValueType;
            bool valueIsValueType = typeof(TValue).GetTypeInfo().IsValueType;
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
                    sb.Append(object.ReferenceEquals(key, dictionary) ? "(this Dictionary)" : (keyIsValueType ? string.Format(new StringFormatter(), "{0}", key) : ToString(key)));
                    sb.Append('=');
                    sb.Append(object.ReferenceEquals(value, dictionary) ? "(this Dictionary)" : (valueIsValueType ? string.Format(new StringFormatter(), "{0}", value) : ToString(value)));
                    if (!i.MoveNext())
                    {
                        return sb.Append('}').ToString();
                    }
                    sb.Append(',').Append(' ');
                }
            }
        }

        /// <summary>
        /// This is the same implementation of ToString from Java's AbstractMap
        /// (the default implementation for all dictionaries), plus the ability
        /// to specify culture for formatting of nested numbers and dates. Note that
        /// this overload will change the culture of the current thread.
        /// </summary>
        public static string ToString<TKey, TValue>(IDictionary<TKey, TValue> dictionary, CultureInfo culture)
        {
            using (var context = new CultureContext(culture))
            {
                return ToString(dictionary);
            }
        }

        /// <summary>
        /// This is a helper method that assists with recursively building
        /// a string of the current collection and all nested collections.
        /// </summary>
        public static string ToString(object obj)
        {
            if (obj == null) return "null";
            Type t = obj.GetType();
            if (t.GetTypeInfo().IsGenericType
                && (t.ImplementsGenericInterface(typeof(ICollection<>)))
                || t.ImplementsGenericInterface(typeof(IDictionary<,>)))
            {
                dynamic genericType = Convert.ChangeType(obj, t);
                return ToString(genericType);
            }

            return obj.ToString();
        }

        /// <summary>
        /// This is a helper method that assists with recursively building
        /// a string of the current collection and all nested collections, plus the ability
        /// to specify culture for formatting of nested numbers and dates. Note that
        /// this overload will change the culture of the current thread.
        /// </summary>
        public static string ToString(object obj, CultureInfo culture)
        {
            using (var context = new CultureContext(culture))
            {
                return ToString(obj);
            }
        }
    }
}
