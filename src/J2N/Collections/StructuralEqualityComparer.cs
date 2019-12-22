using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace J2N.Collections
{
    /// <summary>
    /// A comparer that provides structural equality rules for collections.
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public abstract class StructuralEqualityComparer : IEqualityComparer<object>, IEqualityComparer
    {
        /// <summary>
        /// Gets a <see cref="StructuralEqualityComparer"/> object that compares
        /// objects for structural equality using rules similar to those in Java.
        /// Nested elemements that implement <see cref="IStructuralEquatable"/> are also compared.
        /// </summary>
        public static StructuralEqualityComparer Default { get; } = new DefaultStructuralEqualityComparer();

        /// <summary>
        /// Gets a <see cref="StructuralEqualityComparer"/> object that compares
        /// objects for structural equality using rules similar to those in Java.
        /// Nested elemements are also compared.
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
        public static StructuralEqualityComparer Aggressive { get; } = new AggressiveStructuralEqualityComparer();

        /// <summary>
        /// Compares two objects for structural equality.
        /// Two object are considered structurally equal if
        /// both of them contain the same types.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second dictionary to compare.</param>
        /// <returns><c>true</c> if both objects are structurally equivalent; otherwise, <c>false</c>.</returns>
        public new virtual bool Equals(object x, object y)
        {
            if (x == null)
                return y == null;
            if (y == null)
                return false;

            // Handle nested arrays.
            // NOTE: We don't want to let the Array type determine equality because we will end up with a different
            // result than our implementation, so we bypass the call to IStructuralEquatable for all arrays here.
            if (x is Array arrayX && y is Array arrayY) // J2N TODO: Add support for multiple dimensional arrays
            {
                if (arrayX.Rank == 1 && arrayY.Rank == 1)
                {
                    return ArrayEquals(arrayX, arrayY);
                }
                else
                {
                    // Currently more than 1 dimension is not supported.
                    throw new ArgumentException("Multiple dimensional arrays are not supported.");
                }

                // Arrays not same size are not equal
                //return false;
            }

            // Handle nested collections (that implement IStructuralEquatable)
            else if (x is IStructuralEquatable seObj)
                return seObj.Equals(y, this);

            // Handle non-structured types (ignoring built in .NET collections)
            return UnstructuredEquals(x, y);
        }

        private bool ArrayEquals(Array arrayX, Array arrayY)
        {
            Type elementType = arrayX.GetType().GetElementType();
            if (elementType.GetTypeInfo().IsPrimitive && arrayY.GetType().GetElementType().Equals(elementType))
                return ArrayEqualityComparer<object>.GetPrimitiveOneDimensionalArrayEqualityComparer(elementType).Equals(arrayX, arrayY);

            // Types don't match, or they are object[].
            // So, the only option is to enumerate the arrays to compare them.
            var eA = arrayX.GetEnumerator();
            var eB = arrayY.GetEnumerator();
            while (eA.MoveNext() && eB.MoveNext())
            {
                var o1 = eA.Current;
                var o2 = eB.Current;

                // Handle nested arrays.
                // NOTE: We don't want to let the Array type determine equality because we will end up with a different
                // result than our implementation, so we bypass the call to IStructuralEquatable for all arrays here.
                if (o1 is Array array1 && o2 is Array array2) // J2N TODO: Add support for multiple dimensional arrays
                {
                    if (array1.Rank == 1 && array2.Rank == 1)
                    {
                        if (!ArrayEquals(array1, array2))
                            return false;
                    }
                    else
                    {
                        // Currently more than 1 dimension is not supported.
                        throw new ArgumentException("Multiple dimensional arrays are not supported.");
                    }

                    // Arrays not same size are not equal
                    //return false;
                }
                else if (o1 is IStructuralEquatable eStructuralEquatable)
                {
                    if (!eStructuralEquatable.Equals(o2, this))
                        return false;
                }
                // Handle non-structured types (ignoring built in .NET collections)
                else if (!UnstructuredEquals(o1, o2))
                    return false;
            }

            return !(eA.MoveNext() || eB.MoveNext());
        }

        /// <summary>
        /// Returns the structural hash code for the specified object.
        /// <para/>
        /// This implementation iterates over the any nested arrays or collections getting the hash code
        /// for each element.
        /// </summary>
        /// <param name="obj">The object to calculate the hash code for.</param>
        /// <returns>The hash code for <paramref name="obj"/>.</returns>
        public virtual int GetHashCode(object obj)
        {
            if (obj == null) return 0;

            // Handle nested arrays
            // NOTE: We don't want to let the Array type calculate the hash code because we will end up with a different
            // hash value than our implementation, so we bypass the call to IStructuralEquatable for all arrays here.
            if (obj is Array array && array.Rank == 1) // J2N TODO: Add support for multiple dimensional arrays
                return GetArrayHashCode(array);

            // Handle nested collections (that implement IStructuralEquatable)
            if (obj is IStructuralEquatable seObj)
                return seObj.GetHashCode(this);

            // Handle non-structured types (ignoring built in .NET collections)
            return GetUnstructuredHashCode(obj);
        }

        private int GetArrayHashCode(Array array)
        {
            Type elementType = array.GetType().GetElementType();
            if (elementType.GetTypeInfo().IsPrimitive)
                return ArrayEqualityComparer<object>.GetPrimitiveOneDimensionalArrayEqualityComparer(elementType).GetHashCode(array);

            // Fallback for other array types - enumerate them
            int hashCode = 1, elementHashCode;
            foreach (var element in array)
            {
                elementHashCode = 0;
                if (element != null)
                {
                    // Handle nested arrays.
                    if (element is Array nestedArray)
                        elementHashCode = GetArrayHashCode(nestedArray);

                    // Handle nested collections (that implement IStructuralEquatable)
                    else if (element is IStructuralEquatable eStructuralEquatable)
                        elementHashCode = eStructuralEquatable.GetHashCode(this);

                    // Handle non-structured types (ignoring built in .NET collections)
                    else
                        elementHashCode = GetUnstructuredHashCode(element);
                }

                hashCode = 31 * hashCode + elementHashCode;
            }
            return hashCode;
        }

        /// <summary>
        /// Overridden in a derived class, handles the equality of types that are not
        /// arrays or collections.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><c>true</c> if the provided objects are equal; otherwise, <c>false</c>.</returns>
        protected abstract bool UnstructuredEquals(object x, object y);

        /// <summary>
        /// Overridden in a derived class, handles the get hash code of types that are not
        /// arrays or collections.
        /// </summary>
        /// <param name="obj">The object to provide the hash code for.</param>
        /// <returns>The hash code for <paramref name="obj"/>.</returns>
        protected abstract int GetUnstructuredHashCode(object obj);
    }

#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    internal class DefaultStructuralEqualityComparer : StructuralEqualityComparer
    {
        protected override bool UnstructuredEquals(object x, object y)
        {
            // Handle non-structured types (ignoring built in .NET collections)
            if (x is double dblX && y is double dblY)
                return EqualityComparer<double>.Default.Equals(dblX, dblY);
            if (x is float fltX && y is float fltY)
                return EqualityComparer<float>.Default.Equals(fltX, fltY);
            if (x is string strX && y is string strY)
                return StringComparer.Ordinal.Equals(strX, strY);
            return EqualityComparer<object>.Default.Equals(x, y);
        }

        protected override int GetUnstructuredHashCode(object obj)
        {
            // Handle non-structured types (ignoring built in .NET collections)
            if (obj is double dbl)
                return EqualityComparer<double>.Default.GetHashCode(dbl);
            if (obj is float flt)
                return EqualityComparer<float>.Default.GetHashCode(flt);
            if (obj is string str)
                return StringComparer.Ordinal.GetHashCode(str);
            return EqualityComparer<object>.Default.GetHashCode(obj);
        }
    }

    /// <summary>
    /// In addition to supporting <see cref="IStructuralComparable"/>, this comparer patches existing
    /// <see cref="IList{T}"/>, <see cref="ISet{T}"/>, and <see cref="IDictionary{TKey, TValue}"/> collections
    /// to make them structurally comparable (at additional runtime performance cost), even if they
    /// do not implement <see cref="IStructuralComparable"/>.
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    internal class AggressiveStructuralEqualityComparer : StructuralEqualityComparer
    {
        protected override int GetUnstructuredHashCode(object obj)
        {
            if (StructuralEqualityUtil.IsValueType(obj))
            {
                if (obj is double dbl)
                    return EqualityComparer<double>.Default.GetHashCode(dbl);
                if (obj is float flt)
                    return EqualityComparer<float>.Default.GetHashCode(flt);
                return EqualityComparer<object>.Default.GetHashCode(obj);
            }
            else
            {
                if (obj is string str)
                    return StringComparer.Ordinal.GetHashCode(str);

                // Handle non-structured types (including built in .NET collections)
                return CollectionUtil.GetHashCode(obj);
            }
        }

        protected override bool UnstructuredEquals(object x, object y)
        {
            if (StructuralEqualityUtil.IsValueType(x))
            {
                if (x is double dblX && y is double dblY)
                    return EqualityComparer<double>.Default.Equals(dblX, dblY);
                if (x is float fltX && y is float fltY)
                    return EqualityComparer<float>.Default.Equals(fltX, fltY);
                return EqualityComparer<object>.Default.Equals(x, y);
            }
            else
            {
                if (x is string strX && y is string strY)
                    return StringComparer.Ordinal.Equals(strX, strY);

                // Handle non-structured types (including built in .NET collections)
                return CollectionUtil.Equals(x, y);
            }
        }
    }
}
