using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace J2N.Collections.Generic.Extensions
{
    /// <summary>
    /// Extensions to the <see cref="IDictionary{TKey, TValue}"/> interface.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Returns a read-only <see cref="J2N.Collections.ObjectModel.ReadOnlyDictionary{TKey, TValue}"/> wrapper for the current dictionary.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="collection">The collection to make read-only.</param>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="IDictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// To prevent any modifications to the <see cref="IDictionary{TKey, TValue}"/> object, expose it only through this wrapper.
        /// A <see cref="J2N.Collections.ObjectModel.ReadOnlyDictionary{TKey, TValue}"/> object does not expose methods that modify the collection. However, if
        /// changes are made to the underlying <see cref="IDictionary{TKey, TValue}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        public static J2N.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> collection)
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
        {
            return new J2N.Collections.ObjectModel.ReadOnlyDictionary<TKey, TValue>(collection);
        }
    }
}
