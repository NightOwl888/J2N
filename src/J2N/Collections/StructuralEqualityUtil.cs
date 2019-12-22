using System;
using System.Reflection;

namespace J2N.Collections
{
    /// <summary>
    /// Utilities for structural equality of arrays and collections.
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    internal static class StructuralEqualityUtil
    {
        internal static Func<T, int> LoadGetHashCodeDelegate<T>(bool isValueType, bool isObject, StructuralEqualityComparer structuralEqualityComparer)
        {
            if (isValueType)
                return (value) => EqualityComparer<T>.Default.GetHashCode(value);
            if (isObject)
                return (value) => IsValueType(value) ?
                    EqualityComparer<T>.Default.GetHashCode(value) :
                    (value == null ? 0 : structuralEqualityComparer.GetHashCode(value));
            else
                return (value) => value == null ? 0 : structuralEqualityComparer.GetHashCode(value);
        }

        internal static Func<T, T, bool> LoadEqualsDelegate<T>(bool isValueType, bool isObject, StructuralEqualityComparer structuralEqualityComparer)
        {
            if (isValueType)
                return (valueA, valueB) => EqualityComparer<T>.Default.Equals(valueA, valueB);
            else if (isObject)
                return (valueA, valueB) => IsValueType(valueA) || IsValueType(valueB) ?
                                EqualityComparer<T>.Default.Equals(valueA, valueB) :
                                (valueA == null ? valueB == null : structuralEqualityComparer.Equals(valueA, valueB));
            else // Reference type
                return (valueA, valueB) => valueA == null ? valueB == null : structuralEqualityComparer.Equals(valueA, valueB);
        }

        internal static bool IsValueType<TElement>(TElement value) => value == null ? false : value.GetType().GetTypeInfo().IsValueType;
    }
}
