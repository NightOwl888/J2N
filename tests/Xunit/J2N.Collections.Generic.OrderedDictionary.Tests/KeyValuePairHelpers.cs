using System.Collections.Generic;

namespace J2N.Collections.Tests
{
    internal static class KeyValuePairHelpers
    {
        /// <summary>
        /// J2N-specific helper since KeyValuePair.Create is not in .NET Framework.
        /// Creates a new <see cref="KeyValuePair{TKey,TValue}"/>.
        /// Allows for not having to specify the type arguments.
        /// </summary>
        /// <param name="key">The key of the <see cref="KeyValuePair{TKey,TValue}"/>.</param>
        /// <param name="value">The value of the <see cref="KeyValuePair{TKey,TValue}"/>.</param>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>Returns a new <see cref="KeyValuePair{TKey,TValue}"/>.</returns>
        public static KeyValuePair<TKey, TValue> Create<TKey, TValue>(TKey key, TValue value)
            => new(key, value);
    }
}
