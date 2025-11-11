using System.Collections;
using System.Collections.Generic;
#if FEATURE_IMMUTABLEARRAY
using System.Collections.Immutable;
#endif

namespace J2N
{
    internal static class GenericType<T>
    {
        public static readonly bool IsStructuralEquatable = typeof(IStructuralEquatable).IsAssignableFrom(typeof(T));

        public static readonly bool IsCollection =
                typeof(ICollection).IsAssignableFrom(typeof(T))
                || typeof(IList).IsAssignableFrom(typeof(T))
                || typeof(IDictionary).IsAssignableFrom(typeof(T))
                || typeof(T).ImplementsGenericInterface(typeof(ICollection<>))
                || IsCollectionInterfaceType()
                || typeof(T).ImplementsGenericInterface(typeof(IList<>))
                || IsListInterfaceType()
                || typeof(T).ImplementsGenericInterface(typeof(IDictionary<,>))
                || IsDictionaryInterfaceType()
                || typeof(T).ImplementsGenericInterface(typeof(ISet<>))
                || IsSetInterfaceType()
#if FEATURE_IREADONLYCOLLECTIONS
                || typeof(T).ImplementsGenericInterface(typeof(IReadOnlyCollection<>))
                || typeof(T).ImplementsGenericInterface(typeof(IReadOnlyList<>))
                || typeof(T).ImplementsGenericInterface(typeof(IReadOnlyDictionary<,>))
                || IsReadOnlyCollectionInterfaceType()
                || IsReadOnlyListInterfaceType()
                || IsReadOnlyDictionaryInterfaceType()
#endif
#if FEATURE_READONLYSET
                || typeof(T).ImplementsGenericInterface(typeof(IReadOnlySet<>))
                || IsReadOnlySetInterfaceType()
#endif
#if FEATURE_IMMUTABLEARRAY
                || (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(ImmutableArray<>))
#endif
                ;

        private static bool IsCollectionInterfaceType()
        {
            return typeof(T).IsInterface && typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(ICollection<>);
        }

        private static bool IsListInterfaceType()
        {
            return typeof(T).IsInterface && typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(IList<>);
        }

        private static bool IsDictionaryInterfaceType()
        {
            return typeof(T).IsInterface && typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }

        private static bool IsSetInterfaceType()
        {
            return typeof(T).IsInterface && typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(ISet<>);
        }

#if FEATURE_IREADONLYCOLLECTIONS
        private static bool IsReadOnlyCollectionInterfaceType()
        {
            return typeof(T).IsInterface && typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(IReadOnlyCollection<>);
        }

        private static bool IsReadOnlyListInterfaceType()
        {
            return typeof(T).IsInterface && typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(IReadOnlyList<>);
        }

        private static bool IsReadOnlyDictionaryInterfaceType()
        {
            return typeof(T).IsInterface && typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>);
        }
#endif

#if FEATURE_READONLYSET
        private static bool IsReadOnlySetInterfaceType()
        {
            return typeof(T).IsInterface && typeof(T).IsGenericType &&
                typeof(T).GetGenericTypeDefinition() == typeof(IReadOnlySet<>);
        }
#endif
    }
}
