// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace J2N.Collections.Tests
{
    public static class CollectionExtensions
    {
        //[return: MaybeNull]
        //public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        //{
        //    return dictionary.GetValueOrDefault(key, default);
        //}

        //[return: MaybeNull]
        //public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, [AllowNull] TValue defaultValue)
        //{
        //    if (dictionary == null)
        //    {
        //        throw new ArgumentNullException(nameof(dictionary));
        //    }

        //    TValue value;
        //    return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        //}

        // J2N: Although these methods exist on all of our dictionaries, these extension methods are needed
        // because in the tests we only have IDictionary<TKey, TValue>, which doesn't define these.
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }

            return false;
        }

        public static bool Remove<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (dictionary.TryGetValue(key, out value))
            {
                dictionary.Remove(key);
                return true;
            }

            value = default(TValue);
            return false;
        }
    }
}
