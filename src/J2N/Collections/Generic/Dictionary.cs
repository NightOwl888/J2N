// Based on: https://github.com/dotnet/runtime/blob/v10.0.0-rc.2.25502.107/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/Dictionary.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.ObjectModel;
using J2N.Runtime.CompilerServices;
using J2N.Runtime.InteropServices;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Represents a collection of keys and values.
    /// <para/>
    /// <see cref="Dictionary{TKey, TValue}"/> is similar to <see cref="SCG.Dictionary{TKey, TValue}"/>,
    /// but adds the following features:
    /// <list type="bullet">
    ///     <item><description>
    ///         If <typeparamref name="TKey"/> is <see cref="Nullable{T}"/> or a reference type, the key can be
    ///         <c>null</c> and the value can be retrieved again by with a <c>null</c> key.
    ///     </description></item>
    ///     <item><description>
    ///         Overrides the <see cref="Equals(object)"/> and <see cref="GetHashCode()"/> methods to compare collections
    ///         using structural equality by default. Also, <see cref="IStructuralEquatable"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    ///     <item><description>
    ///         Overrides the <see cref="ToString()"/> method to list the contents of the dictionary
    ///         by default. Also, <see cref="IFormatProvider"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    ///     <item><description>
    ///         Uses <see cref="EqualityComparer{T}.Default"/> by default, which provides some specialized equality comparisons
    ///         for specific types to match the behavior of Java.
    ///     </description></item>
    ///     <item><description>
    ///         This implementation allows deleting while enumerating, much like the collections in Java and .NET Core 3.0+ do.
    ///         However, rather than having a method to delete on the enumerator itself, one must call
    ///         <see cref="Dictionary{TKey, TValue}.Remove(TKey)"/>.
    ///     </description></item>
    /// </list>
    /// <para/>
    /// Usage Note: This class is intended to be a direct replacement for <see cref="SCG.Dictionary{TKey, TValue}"/> in order
    /// to provide default structural equality and formatting behavior similar to Java. Note that the <see cref="ToString()"/>
    /// method uses the current culture by default to behave like other components in .NET. To exactly match Java's culture-neutral behavior,
    /// call <c>ToString(StringFormatter.InvariantCulture)</c>.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerTypeProxy(typeof(IDictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public class Dictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary,
#if FEATURE_IREADONLYCOLLECTIONS
        IReadOnlyDictionary<TKey, TValue>,
#endif
        IStructuralEquatable, IStructuralFormattable
#if FEATURE_SERIALIZABLE
        , System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
#endif
    {
#if FEATURE_SERIALIZABLE
        // constants for serialization
        private const string EqualityComparerName = "EqualityComparer"; // Do not rename (binary serialization)
        private const string HashSizeName = "HashSize"; // Do not rename (binary serialization). Must save buckets.Length
        private const string CountName = "Count"; // Do not rename (binary serialization) - used to allocate during deserialzation, not actually a field. This only exists for backward compatibility with J2N 2.0.0.
        private const string KeyValuePairsName = "KeyValuePairs"; // Do not rename (binary serialization)
        private const string VersionName = "Version"; // Do not rename (binary serialization)
#endif

        private int[]? _buckets;
        private Entry[]? _entries;
        private ulong _fastModMultiplier;
        private int _count;
        private int _freeList;
        private int _freeCount;
        private int _version;
        private IEqualityComparer<TKey>? _comparer;
        private KeyCollection? _keys;
        private ValueCollection? _values;
        private const int StartOfFreeList = -3;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class that is empty,
        /// has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <remarks>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the default equality comparer.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// This constructor uses J2N's default generic equality comparer, <see cref="EqualityComparer{T}.Default"/>. If type
        /// <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="IEqualityComparer{T}"/>
        /// generic interface by using a constructor that accepts a comparer parameter.
        /// <para/>
        /// NOTE: If you can estimate the size of the collection, using a constructor that specifies the initial capacity eliminates
        /// the need to perform a number of resizing operations while adding elements to the <see cref="Dictionary{TKey, TValue}"/>.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public Dictionary() : this(0, null) { /* Intentionally blank */ }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class that is empty, has the specified initial
        /// capacity, and uses the default equality comparer for the key type.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="Dictionary{TKey, TValue}"/> can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        /// <remarks>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the default equality comparer.
        /// <para/>
        /// The capacity of a <see cref="Dictionary{TKey, TValue}"/> is the number of elements that can be added to the
        /// <see cref="Dictionary{TKey, TValue}"/> before resizing is necessary. As elements are added to a <see cref="Dictionary{TKey, TValue}"/>,
        /// the capacity is automatically increased as required by reallocating the internal array.
        /// <para/>
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number
        /// of resizing operations while adding elements to the <see cref="Dictionary{TKey, TValue}"/>.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal. This
        /// constructor uses J2N's default generic equality comparer, <see cref="EqualityComparer{T}.Default"/>. If type <typeparamref name="TKey"/>
        /// implements the <see cref="IEquatable{T}"/> generic interface, the default equality comparer uses that implementation. Alternatively,
        /// you can specify an implementation of the <see cref="IEqualityComparer{T}"/> generic interface by using a constructor that accepts a
        /// comparer parameter.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public Dictionary(int capacity) : this(capacity, null) { /* Intentionally blank */ }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class that is empty, has the default
        /// initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or
        /// <c>null</c> to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        /// <remarks>
        /// Use this constructor with the case-insensitive string comparers provided by the <see cref="StringComparer"/>
        /// class to create dictionaries with case-insensitive string keys.
        /// <para/>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the specified comparer.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// If comparer is null, this constructor uses J2N's default generic equality comparer, <see cref="EqualityComparer{T}.Default"/>.
        /// If type <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default
        /// equality comparer uses that implementation.
        /// <para/>
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need to perform a number
        /// of resizing operations while adding elements to the <see cref="Dictionary{TKey, TValue}"/>.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public Dictionary(IEqualityComparer<TKey>? comparer) : this(0, comparer) { /* Intentionally blank */ }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class that is empty, has the specified
        /// initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="capacity">The initial number of elements that the <see cref="Dictionary{TKey, TValue}"/> can contain.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c> to
        /// use J2N's default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        /// <remarks>
        /// Use this constructor with the case-insensitive string comparers provided by the <see cref="StringComparer"/>
        /// class to create dictionaries with case-insensitive string keys.
        /// <para/>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the specified comparer.
        /// <para/>
        /// The capacity of a <see cref="Dictionary{TKey, TValue}"/> is the number of elements that can be added to
        /// the <see cref="Dictionary{TKey, TValue}"/> before resizing is necessary. As elements are added to a
        /// <see cref="Dictionary{TKey, TValue}"/>, the capacity is automatically increased as required by
        /// reallocating the internal array.
        /// <para/>
        /// If the size of the collection can be estimated, specifying the initial capacity eliminates the need
        /// to perform a number of resizing operations while adding elements to the <see cref="Dictionary{TKey, TValue}"/>.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are
        /// equal. If comparer is <c>null</c>, this constructor uses the default generic equality comparer,
        /// <see cref="EqualityComparer{T}.Default"/>. If type <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/>
        /// generic interface, the default equality comparer uses that implementation.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public Dictionary(int capacity, IEqualityComparer<TKey>? comparer)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);

            if (capacity > 0)
            {
                Initialize(capacity);
            }

            // For reference types, we always want to store a comparer instance, either
            // the one provided, or if one wasn't provided, the default (accessing
            // EqualityComparer<TKey>.Default with shared generics on every dictionary
            // access can add measurable overhead).  For value types, if no comparer is
            // provided, or if the default is provided, we'd prefer to use
            // EqualityComparer<TKey>.Default.Equals on every use, enabling the JIT to
            // devirtualize and possibly inline the operation.
            if (!typeof(TKey).IsValueType)
            {
                _comparer = comparer ?? EqualityComparer<TKey>.Default;

                // Special-case EqualityComparer<string>.Default, StringComparer.Ordinal, and StringComparer.OrdinalIgnoreCase.
                // We use a non-randomized comparer for improved perf, falling back to a randomized comparer if the
                // hash buckets become unbalanced.
                if (typeof(TKey) == typeof(string) && NonRandomizedStringEqualityComparer.GetStringComparer(_comparer!) is IEqualityComparer<string> stringComparer)
                {
                    _comparer = (IEqualityComparer<TKey>)stringComparer;
                }
            }
            else if (comparer is not null && // first check for null to avoid forcing default comparer instantiation unnecessarily
                     comparer != EqualityComparer<TKey>.Default)
            {
                _comparer = comparer;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class that contains elements
        /// copied from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the default equality comparer
        /// for the key type.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the
        /// new <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the default equality
        /// comparer; likewise, every key in the source <paramref name="dictionary"/> must also be unique according to the default
        /// equality comparer.
        /// <para/>
        /// The initial capacity of the new <see cref="Dictionary{TKey, TValue}"/> is large enough to contain all the
        /// elements in <paramref name="dictionary"/>.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// This constructor uses the default generic equality comparer, <see cref="EqualityComparer{T}.Default"/>. If type
        /// <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="IEqualityComparer{T}"/>
        /// generic interface by using a constructor that accepts a comparer parameter.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in <paramref name="dictionary"/>.
        /// </remarks>
        public Dictionary(IDictionary<TKey, TValue> dictionary) : this(dictionary, null) { /* Intentionally blank */ }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class that contains elements copied
        /// from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="dictionary">The <see cref="IDictionary{TKey, TValue}"/> whose elements are copied to the new
        /// <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c>
        /// to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="dictionary"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Use this constructor with the case-insensitive string comparers provided by the <see cref="StringComparer"/>
        /// class to create dictionaries with case-insensitive string keys.
        /// <para/>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the specified comparer;
        /// likewise, every key in the source <paramref name="dictionary"/> must also be unique according to the specified comparer.
        /// <para/>
        /// NOTE: For example, duplicate keys can occur if <paramref name="comparer"/> is one of the case-insensitive string
        /// comparers provided by the <see cref="StringComparer"/> class and <paramref name="dictionary"/> does not use a
        /// case-insensitive comparer key.
        /// <para/>
        /// The initial capacity of the new <see cref="Dictionary{TKey, TValue}"/> is large enough to contain all the elements in
        /// <paramref name="dictionary"/>.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// If comparer is <c>null</c>, this constructor uses the default generic equality comparer, <see cref="EqualityComparer{T}.Default"/>.
        /// If type <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation.
        /// <para/>
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in <paramref name="dictionary"/>.
        /// </remarks>
        public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer)
            : this(dictionary?.Count ?? 0, comparer)
        {
            if (dictionary is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
            AddRange(dictionary);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class that contains elements
        /// copied from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the default equality comparer
        /// for the key type.
        /// </summary>
        /// <param name="collection">The <see cref="T:IEnumerable{KeyValuePair{TKey, TValue}}"/> whose elements are copied to the
        /// new <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the default equality
        /// comparer; likewise, every key in the source <paramref name="collection"/> must also be unique according to the default
        /// equality comparer.
        /// <para/>
        /// The initial capacity of the new <see cref="Dictionary{TKey, TValue}"/> is large enough to contain all the
        /// elements in <paramref name="collection"/>.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// This constructor uses the default generic equality comparer, <see cref="EqualityComparer{T}.Default"/>. If type
        /// <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation. Alternatively, you can specify an implementation of the <see cref="IEqualityComparer{T}"/>
        /// generic interface by using a constructor that accepts a comparer parameter.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in <paramref name="collection"/>.
        /// </remarks>
        public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : this(collection, null) { /* Intentionally blank */ }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class that contains elements copied
        /// from the specified <see cref="IDictionary{TKey, TValue}"/> and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="collection">The <see cref="T:IEnumerable{KeyValuePair{TKey, TValue}}"/> whose elements are copied to the new
        /// <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c>
        /// to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="collection"/> contains one or more duplicate keys.</exception>
        /// <remarks>
        /// Use this constructor with the case-insensitive string comparers provided by the <see cref="StringComparer"/>
        /// class to create dictionaries with case-insensitive string keys.
        /// <para/>
        /// Every key in a <see cref="Dictionary{TKey, TValue}"/> must be unique according to the specified comparer;
        /// likewise, every key in the source <paramref name="collection"/> must also be unique according to the specified comparer.
        /// <para/>
        /// NOTE: For example, duplicate keys can occur if <paramref name="comparer"/> is one of the case-insensitive string
        /// comparers provided by the <see cref="StringComparer"/> class and <paramref name="collection"/> does not use a
        /// case-insensitive comparer key.
        /// <para/>
        /// The initial capacity of the new <see cref="Dictionary{TKey, TValue}"/> is large enough to contain all the elements in
        /// <paramref name="collection"/>.
        /// <para/>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine whether keys are equal.
        /// If comparer is <c>null</c>, this constructor uses the default generic equality comparer, <see cref="EqualityComparer{T}.Default"/>.
        /// If type <typeparamref name="TKey"/> implements the <see cref="IEquatable{T}"/> generic interface, the default equality
        /// comparer uses that implementation.
        /// <para/>
        /// This constructor is an O(<c>n</c>) operation, where <c>n</c> is the number of elements in <paramref name="collection"/>.
        /// </remarks>
        public Dictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer)
            : this((collection as ICollection<KeyValuePair<TKey, TValue>>)?.Count ?? 0, comparer)
        {
            if (collection is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            AddRange(collection);
        }

        private void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> enumerable)
        {
            // It is likely that the passed-in enumerable is Dictionary<TKey,TValue>. When this is the case,
            // avoid the enumerator allocation and overhead by looping through the entries array directly.
            // We only do this when dictionary is Dictionary<TKey,TValue> and not a subclass, to maintain
            // back-compat with subclasses that may have overridden the enumerator behavior.
            if (enumerable.GetType() == typeof(Dictionary<TKey, TValue>))
            {
                Dictionary<TKey, TValue> source = (Dictionary<TKey, TValue>)enumerable;

                if (source.Count == 0)
                {
                    // Nothing to copy, all done
                    return;
                }

                // This is not currently a true .AddRange as it needs to be an initialized dictionary
                // of the correct size, and also an empty dictionary with no current entities (and no argument checks).
                Debug.Assert(source._entries is not null);
                Debug.Assert(_entries is not null);
                Debug.Assert(_entries!.Length >= source.Count);
                Debug.Assert(_count == 0);

                Entry[] oldEntries = source._entries!; // [!]: asserted above
                if (source._comparer == _comparer)
                {
                    // If comparers are the same, we can copy _entries without rehashing.
                    CopyEntries(oldEntries, source._count);
                    return;
                }

                // Comparers differ need to rehash all the entries via Add
                int count = source._count;
                for (int i = 0; i < count; i++)
                {
                    // Only copy if an entry
                    if (oldEntries[i].next >= -1)
                    {
                        Add(oldEntries[i].key, oldEntries[i].value);
                    }
                }
                return;
            }

            // We similarly special-case KVP<>[] and List<KVP<>>, as they're commonly used to seed dictionaries, and
            // we want to avoid the enumerator costs (e.g. allocation) for them as well. Extract a span if possible.
            ReadOnlySpan<KeyValuePair<TKey, TValue>> span;
            if (enumerable is KeyValuePair<TKey, TValue>[] array)
            {
                span = array;
            }
            else if (enumerable.GetType() == typeof(List<KeyValuePair<TKey, TValue>>))
            {
                span = CollectionMarshal.AsSpan((List<KeyValuePair<TKey, TValue>>)enumerable);
            }
#if FEATURE_COLLECTIONSMARSHAL_ASSPAN_LIST
            else if (enumerable.GetType() == typeof(SCG.List<KeyValuePair<TKey, TValue>>))
            {
                span = CollectionsMarshal.AsSpan((SCG.List<KeyValuePair<TKey, TValue>>)enumerable);
            }
#endif
            else
            {
                // Fallback path for all other enumerables
                foreach (KeyValuePair<TKey, TValue> pair in enumerable)
                {
                    Add(pair.Key, pair.Value);
                }
                return;
            }

            // We got a span. Add the elements to the dictionary.
            foreach (KeyValuePair<TKey, TValue> pair in span)
            {
                Add(pair.Key, pair.Value);
            }
        }

#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}"/> class with serialized data.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object containing
        /// the information required to serialize the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> structure containing
        /// the source and destination of the serialized stream associated with the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <remarks>
        /// This constructor is called during deserialization to reconstitute an object transmitted over a stream.
        /// For more information, see
        /// <a href="https://docs.microsoft.com/en-us/dotnet/standard/serialization/xml-and-soap-serialization">XML and SOAP Serialization</a>.
        /// </remarks>
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected Dictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            // We can't do anything with the keys and values until the entire graph has been deserialized
            // and we have a resonable estimate that GetHashCode is not going to fail.  For the time being,
            // we'll just cache this.  The graph is not valid until OnDeserialization has been called.
            HashHelpers.SerializationInfoTable.Add(this, info);
        }

#endif

        #region AsReadOnly

        /// <summary>
        /// Returns a read-only <see cref="ReadOnlyDictionary{TKey, TValue}"/> wrapper for the current collection.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// To prevent any modifications to the <see cref="Dictionary{TKey, TValue}"/> object, expose it only through this wrapper.
        /// A <see cref="ReadOnlyDictionary{TKey, TValue}"/> object does not expose methods that modify the collection. However,
        /// if changes are made to the underlying <see cref="Dictionary{TKey, TValue}"/> object, the read-only collection reflects those changes.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public ReadOnlyDictionary<TKey, TValue> AsReadOnly()
            => new ReadOnlyDictionary<TKey, TValue>(this);

        #endregion AsReadOnly

        #region SCG.Dictionary<TKey, TValue> Members

        /// <summary>
        /// Gets the <see cref="IEqualityComparer{T}"/> that is used to determine equality of keys
        /// for the dictionary.
        /// </summary>
        /// <remarks>
        /// <see cref="Dictionary{TKey, TValue}"/> requires an equality implementation to determine
        /// whether keys are equal. You can specify an implementation of the <see cref="IEqualityComparer{T}"/>
        /// generic interface by using a constructor that accepts a comparer parameter; if you do not
        /// specify one, J2N's default generic equality comparer <see cref="EqualityComparer{T}.Default"/> is used.
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public IEqualityComparer<TKey> EqualityComparer
        {
            get
            {
                if (typeof(TKey) == typeof(string))
                {
                    Debug.Assert(_comparer is not null, "The comparer should never be null for a reference type.");
                    return (IEqualityComparer<TKey>)InternalStringEqualityComparer.GetUnderlyingEqualityComparer((IEqualityComparer<string?>)_comparer!); // [!]: asserted above
                }
                else
                {
                    return _comparer ?? EqualityComparer<TKey>.Default;
                }
            }
        }

        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains a specific value.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="Dictionary{TKey, TValue}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <returns><c>true</c> if the <see cref="Dictionary{TKey, TValue}"/> contains an element
        /// with the specified value; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method determines equality using J2N's default equality comparer
        /// <see cref="EqualityComparer{T}.Default"/> for <typeparamref name="TValue"/>,
        /// the type of values in the dictionary.
        /// <para/>
        /// This method performs a linear search; therefore, the average execution time
        /// is proportional to <see cref="Count"/>. That is, this method is an O(<c>n</c>) operation,
        /// where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool ContainsValue([AllowNull] TValue value)
        {
            Entry[]? entries = _entries;
            if (value == null)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (entries![i].next >= -1 && entries[i].value == null)
                    {
                        return true;
                    }
                }
            }
            else if (typeof(TValue).IsValueType)
            {
                // ValueType: Devirtualize with EqualityComparer<TValue>.Default intrinsic
                for (int i = 0; i < _count; i++)
                {
                    if (entries![i].next >= -1 && EqualityComparer<TValue>.Default.Equals(entries[i].value!, value))
                    {
                        return true;
                    }
                }
            }
            else
            {
                // Object type: Shared Generic, EqualityComparer<TValue>.Default won't devirtualize
                // https://github.com/dotnet/runtime/issues/10050
                // So cache in a local rather than get EqualityComparer per loop iteration
                IEqualityComparer<TValue> defaultComparer = EqualityComparer<TValue>.Default;
                for (int i = 0; i < _count; i++)
                {
                    if (entries![i].next >= -1 && defaultComparer.Equals(entries[i].value!, value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains a specific value
        /// as determined by the provided <paramref name="valueComparer"/>.
        /// </summary>
        /// <param name="value">The value to locate in the <see cref="Dictionary{TKey, TValue}"/>.
        /// The value can be <c>null</c> for reference types.</param>
        /// <param name="valueComparer">The <see cref="IEqualityComparer{TValue}"/> to use
        /// to test each value for equality.</param>
        /// <returns><c>true</c> if the <see cref="Dictionary{TKey, TValue}"/> contains an element
        /// with the specified value; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method performs a linear search; therefore, the average execution time
        /// is proportional to <see cref="Count"/>. That is, this method is an O(<c>n</c>) operation,
        /// where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public bool ContainsValue([AllowNull] TValue value, IEqualityComparer<TValue>? valueComparer) // Overload added so end user can override J2N's equality comparer
        {
            valueComparer ??= EqualityComparer<TValue>.Default;

            Entry[]? entries = _entries;
            if (value is null)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (entries![i].next >= -1 && entries[i].value is null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _count; i++)
                {
                    if (entries![i].next >= -1 && valueComparer.Equals(entries[i].value!, value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Ensures that the dictionary can hold up to a specified number of entries without any
        /// further expansion of its backing storage.
        /// </summary>
        /// <param name="capacity">The number of entries.</param>
        /// <returns>The current capacity of the <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than 0.</exception>
        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(capacity, ExceptionArgument.capacity);

            int currentCapacity = _entries == null ? 0 : _entries.Length;
            if (currentCapacity >= capacity)
            {
                return currentCapacity;
            }

            _version++;

            if (_buckets == null)
            {
                return Initialize(capacity);
            }

            int newSize = HashHelpers.GetPrime(capacity);
            Resize(newSize, forceNewHashCodes: false);
            return newSize;
        }

#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and returns the data needed to
        /// serialize the <see cref="Dictionary{TKey, TValue}"/> instance.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object that contains the
        /// information required to serialize the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> structure that contains
        /// the source and destination of the serialized stream associated with the <see cref="Dictionary{TKey, TValue}"/> instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="info"/> is <c>null</c>.</exception>
        /// <remarks>This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        [System.Security.SecurityCritical]
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            // Customized serialization for Dictionary.
            // We need to do this because it will give us flexibility to change the design
            // without changing the serialized info.
            if (info is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.info);

            info.AddValue(VersionName, _version);
            info.AddValue(EqualityComparerName, EqualityComparer, typeof(IEqualityComparer<TKey>));
            info.AddValue(HashSizeName, _buckets == null ? 0 : _buckets.Length); // This is the length of the bucket array

            if (_buckets != null)
            {
                var array = new KeyValuePair<TKey, TValue>[Count];
                CopyTo(array, 0);
                info.AddValue(KeyValuePairsName, array, typeof(KeyValuePair<TKey, TValue>[]));
            }
        }

        /// <summary>
        /// Implements the <see cref="System.Runtime.Serialization.ISerializable"/> interface and raises the deserialization
        /// event when the deserialization is complete.
        /// </summary>
        /// <param name="sender">The source of the deserialization event.</param>
        /// <exception cref="System.Runtime.Serialization.SerializationException">
        /// The <see cref="System.Runtime.Serialization.SerializationInfo"/> object associated with the current
        /// <see cref="Dictionary{TKey, TValue}"/> instance is invalid.</exception>
        /// <remarks>This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.</remarks>
        public virtual void OnDeserialization(object? sender)
        {
            HashHelpers.SerializationInfoTable.TryGetValue(this, out SerializationInfo? siInfo);

            if (siInfo == null)
            {
                // We can return immediately if this function is called twice.
                // Note we remove the serialization info from the table at the end of this method.
                return;
            }

            int realVersion = siInfo.GetInt32(VersionName);
            int hashsize = 0;

            // Try to get HashSizeName, and if it fails, use CountName as fallback.
            // CountName was used to serialize Dictionary<TKey, TValue> in J2N 2.0.0 and earlier,
            // so this is just for backward compatibility. It was used incorrectly, though.
            // It was tracking the count instead of the capacity.
            try
            {
                hashsize = siInfo.GetInt32(HashSizeName);
            }
            catch (SerializationException)
            {
                // Fallback to CountName if HashSizeName is not present.
                hashsize = siInfo.GetInt32(CountName);
            }
            _comparer = (IEqualityComparer<TKey>)siInfo.GetValue(EqualityComparerName, typeof(IEqualityComparer<TKey>))!; // When serialized if comparer is null, we use the default.

            if (hashsize != 0)
            {
                Initialize(hashsize);

                KeyValuePair<TKey, TValue>[]? array = (KeyValuePair<TKey, TValue>[]?)
                    siInfo.GetValue(KeyValuePairsName, typeof(KeyValuePair<TKey, TValue>[]));

                if (array == null)
                {
                    ThrowHelper.ThrowSerializationException(ExceptionResource.Serialization_MissingKeys);
                }

                for (int i = 0; i < array.Length; i++)
                {
                    // J2N allows null keys

                    Add(array[i].Key, array[i].Value);
                }
            }
            else
            {
                _buckets = null;
            }

            _version = realVersion;
            HashHelpers.SerializationInfoTable.Remove(this);
        }
#endif

        /// <summary>
        /// Sets the capacity of this dictionary to hold up a specified number of entries
        /// without any further expansion of its backing storage.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less
        /// than <see cref="Dictionary{TKey, TValue}"/>.</exception>
        /// <remarks>
        /// This method can be used to minimize the memory overhead once it is known that no
        /// new elements will be added.
        /// </remarks>
        public void TrimExcess(int capacity)
        {
            if (capacity < Count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
            }

            int newSize = HashHelpers.GetPrime(capacity);
            Entry[]? oldEntries = _entries;
            int currentCapacity = oldEntries == null ? 0 : oldEntries.Length;
            if (newSize >= currentCapacity)
            {
                return;
            }

            int oldCount = _count;
            _version++;
            Initialize(newSize);

            Debug.Assert(oldEntries is not null);

            CopyEntries(oldEntries!, oldCount);
        }

        /// <summary>
        /// Sets the capacity of this dictionary to what it would be if it had been originally
        /// initialized with all its entries.
        /// </summary>
        /// <remarks>
        /// This method can be used to minimize memory overhead once it is known that no new
        /// elements will be added to the dictionary. To allocate a minimum size storage array,
        /// execute the following statements:
        /// <code>
        /// dictionary.Clear();
        /// dictionary.TrimExcess();
        /// </code>
        /// </remarks>
        public void TrimExcess() => TrimExcess(Count);

        /// <summary>
        /// Attempts to add the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add. It can be <c>null</c>.</param>
        /// <param name="value">The value of the element to add. It can be <c>null</c>.</param>
        /// <returns><c>true</c> if the key/value pair was added to the dictionary successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>Unlike the <see cref="Add(TKey, TValue)"/> method, this method doesn't throw an exception
        /// if the element with the given key exists in the dictionary. Unlike the Dictionary indexer, <see cref="TryAdd(TKey, TValue)"/>
        /// doesn't override the element if the element with the given key exists in the dictionary. If the key already exists,
        /// <see cref="TryAdd(TKey, TValue)"/> does nothing and returns <c>false</c>.</remarks>
        // J2N: This is an extension method on IDictionary<TKey, TValue>, but only for .NET Standard 2.1+.
        // It is redefined here to ensure we have it in prior platforms.
        public bool TryAdd([AllowNull] TKey key, [AllowNull] TValue value) =>
            TryInsert(key, value, InsertionBehavior.None);

        #endregion SCG.Dictionary<TKey, TValue> Members

        #region IDictionary<TKey, TValue> Members

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The order of the keys in the <see cref="ICollection{TKey}"/> is unspecified, but it is the same order as the
        /// associated values in the <see cref="ICollection{TValue}"/> returned by the <see cref="Values"/> property.
        /// <para/>
        /// The returned <see cref="ICollection{TKey}"/> is not a static copy; instead,
        /// the <see cref="ICollection{TKey}"/> refers back to the keys in the original
        /// <see cref="Dictionary{TKey, TValue}"/>. Therefore, changes to the <see cref="Dictionary{TKey, TValue}"/>
        /// continue to be reflected in the <see cref="ICollection{TKey}"/>.
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public ICollection<TKey> Keys => _keys ??= new KeyCollection(this);

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        /// <summary>
        /// Gets a collection containing the values in the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The order of the values in the <see cref="ICollection{TValue}"/> is unspecified, but it is the same order as the
        /// associated keys in the <see cref="ICollection{TKey}"/> returned by the <see cref="Keys"/> property.
        /// <para/>
        /// The returned <see cref="ICollection{TValue}"/> is not a static copy;
        /// instead, the <see cref="ICollection{TValue}"/> refers back to the
        /// values in the original <see cref="SortedDictionary{TKey, TValue}"/>. Therefore, changes to
        /// the <see cref="SortedDictionary{TKey, TValue}"/> continue to be reflected in the
        /// <see cref="ICollection{TValue}"/>.
        /// <para/>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public ICollection<TValue> Values => _values ??= new ValueCollection(this);

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// Getting the value of this property is an O(1) operation.
        /// </remarks>
        public int Count => _count - _freeCount;

        /// <summary>
        /// Gets the total numbers of elements the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity => _entries?.Length ?? 0;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        /// <summary>
        /// Gets or sets the value associated with the specified key. The key may be <c>null</c>.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The value associated with the specified key. If the specified key
        /// is not found, a get operation throws a <see cref="KeyNotFoundException"/>, and
        /// a set operation creates a new element with the specified key.</returns>
        /// <exception cref="KeyNotFoundException">The property is retrieved and <paramref name="key"/>
        /// does not exist in the collection.</exception>
        /// <remarks>
        /// This property provides the ability to access a specific element in the collection by using
        /// the following C# syntax: <c>myCollection[key]</c> (<c>myCollection(key)</c> in Visual Basic).
        /// <para/>
        /// You can also use the <see cref="this[TKey]"/> property to add new elements by setting the value of a key
        /// that does not exist in the <see cref="Dictionary{TKey, TValue}"/>; for example,
        /// <c>myCollection["myNonexistentKey"] = myValue</c>. However, if the specified key already exists in
        /// the <see cref="Dictionary{TKey, TValue}"/>, setting the <see cref="this[TKey]"/> property overwrites
        /// the old value. In contrast, the <see cref="Add(TKey, TValue)"/> method does not modify existing elements.
        /// <para/>
        /// Unlike the <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>, both keys and values can
        /// be <c>null</c> if either <see cref="Nullable{T}"/> or a reference type.
        /// <para/>
        /// The C# language uses the <see cref="this"/> keyword to define the indexers instead of implementing the
        /// <c>Item[TKey]</c> property. Visual Basic implements <c>Item[TKey]</c> as a default property, which provides
        /// the same indexing functionality.
        /// <para/>
        /// Getting the value of this property is an O(log <c>n</c>) operation; setting the property is also
        /// an O(log <c>n</c>) operation.
        /// </remarks>
        public TValue this[[AllowNull] TKey key]
        {
            [return: MaybeNull]
            get
            {
                ref TValue value = ref FindValue(key);
                if (!UnsafeHelpers.IsNullRef(ref value))
                {
                    return value;
                }

                if (key is null)
                {
                    ThrowHelper.ThrowKeyNotFoundException("(null)");
                    return default;
                }

                ThrowHelper.ThrowKeyNotFoundException(key);
                return default;
            }
            set
            {
                bool modified = TryInsert(key, value, InsertionBehavior.OverwriteExisting);
                Debug.Assert(modified);
            }
        }

        /// <summary>
        /// Adds an element with the specified key and value into the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be <c>null</c> for reference types.</param>
        /// <exception cref="ArgumentException">An element with the same key already exists
        /// in the <see cref="Dictionary{TKey, TValue}"/>.</exception>
        /// <remarks>
        /// You can also use the <see cref="this[TKey]"/> property to add new elements by setting the value of
        /// a key that does not exist in the <see cref="Dictionary{TKey, TValue}"/>; for example,
        /// <c>myCollection["myNonexistentKey"] = myValue</c> (in Visual Basic, <c>myCollection("myNonexistantKey") = myValue</c>).
        /// However, if the specified key already exists in the <see cref="Dictionary{TKey, TValue}"/>, setting
        /// the <see cref="this[TKey]"/> property overwrites the old value. In contrast, the <see cref="Add(TKey, TValue)"/>
        /// method throws an exception if an element with the specified key already exists.
        /// <para/>
        /// Both keys and values can be <c>null</c> if the corresponding <typeparamref name="TKey"/> or
        /// <typeparamref name="TValue"/> is <see cref="Nullable{T}"/> or a reference type.
        /// <para/>
        /// This method is an O(log <c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
        /// </remarks>
        public void Add([AllowNull] TKey key, [AllowNull] TValue value)
        {
            bool modified = TryInsert(key, value, InsertionBehavior.ThrowOnExisting);
            Debug.Assert(modified); // If there was an existing key and the Add failed, an exception will already have been thrown.
        }

        /// <summary>
        /// Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains an
        /// element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="Dictionary{TKey, TValue}"/>. The key can be <c>null</c></param>
        /// <returns><c>true</c> if the <see cref="Dictionary{TKey, TValue}"/> contains an element
        /// with the specified key; otherwise, <c>false</c>.</returns>
        /// <remarks>This method is an O(log <c>n</c>) operation.</remarks>
        public bool ContainsKey([AllowNull] TKey key)
            => !UnsafeHelpers.IsNullRef(ref FindValue(key));

        /// <summary>
        /// Removes the element with the specified key from the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if key is not found in the <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// If the <see cref="Dictionary{TKey, TValue}"/> does not contain an element with the specified key, the
        /// <see cref="Dictionary{TKey, TValue}"/> remains unchanged. No exception is thrown.
        /// <para/>
        /// This method is an O(log <c>n</c>) operation.
        /// </remarks>
        public bool Remove([AllowNull] TKey key)
        {
            // The overload Remove(TKey key, out TValue value) is a copy of this method with one additional
            // statement to copy the value for entry being removed into the output parameter.
            // Code has been intentionally duplicated for performance reasons.

            // J2N allows null keys

            if (_buckets != null)
            {
                Debug.Assert(_entries != null, "entries should be non-null");
                uint collisionCount = 0;

                IEqualityComparer<TKey>? comparer = _comparer;
                Debug.Assert(typeof(TKey).IsValueType || comparer is not null);
                uint hashCode = (uint)(typeof(TKey).IsValueType && comparer == null ? key?.GetHashCode() ?? 0 : key is not null ? comparer!.GetHashCode(key!) : 0);

                ref int bucket = ref GetBucket(hashCode);
                Entry[]? entries = _entries;
                int last = -1;
                int i = bucket - 1; // Value in buckets is 1-based
                if (key is not null)
                {
                    while (i >= 0)
                    {
                        ref Entry entry = ref entries![i];

                        if (entry.hashCode == hashCode &&
                            (typeof(TKey).IsValueType && comparer == null ? EqualityComparer<TKey>.Default.Equals(entry.key, key) : comparer!.Equals(entry.key, key)))
                        {
                            if (last < 0)
                            {
                                bucket = entry.next + 1; // Value in buckets is 1-based
                            }
                            else
                            {
                                entries[last].next = entry.next;
                            }

                            Debug.Assert((StartOfFreeList - _freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                            entry.next = StartOfFreeList - _freeList;

                            if (RuntimeHelper.IsReferenceOrContainsReferences<TKey>())
                            {
                                entry.key = default!;
                            }

                            if (RuntimeHelper.IsReferenceOrContainsReferences<TValue>())
                            {
                                entry.value = default!;
                            }

                            _freeList = i;
                            _freeCount++;
                            return true;
                        }

                        last = i;
                        i = entry.next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
                else
                {
                    while (i >= 0)
                    {
                        ref Entry entry = ref entries![i];

                        if (entry.hashCode == hashCode && entry.key is null)
                        {
                            if (last < 0)
                            {
                                bucket = entry.next + 1; // Value in buckets is 1-based
                            }
                            else
                            {
                                entries[last].next = entry.next;
                            }

                            Debug.Assert((StartOfFreeList - _freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                            entry.next = StartOfFreeList - _freeList;

                            if (RuntimeHelper.IsReferenceOrContainsReferences<TKey>())
                            {
                                entry.key = default!;
                            }

                            if (RuntimeHelper.IsReferenceOrContainsReferences<TValue>())
                            {
                                entry.value = default!;
                            }

                            _freeList = i;
                            _freeCount++;
                            return true;
                        }

                        last = i;
                        i = entry.next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter.</param>
        /// <returns><c>true</c> if the <see cref="Dictionary{TKey, TValue}"/> contains an element with the
        /// specified key; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method combines the functionality of the <see cref="ContainsKey(TKey)"/> method
        /// and the <see cref="this[TKey]"/> property.
        /// <para/>
        /// If the key is not found, then the <paramref name="value"/> parameter gets the appropriate
        /// default value for the type <typeparamref name="TValue"/>; for example, 0 (zero) for
        /// integer types, <c>false</c> for Boolean types, and <c>null</c> for reference types.
        /// <para/>
        /// Use the <see cref="TryGetValue(TKey, out TValue)"/> method if your code frequently
        /// attempts to access keys that are not in the dictionary. Using this method is more
        /// efficient than catching the <see cref="KeyNotFoundException"/> thrown by the
        /// <see cref="this[TKey]"/> property.
        /// <para/>
        /// This method approaches an O(1) operation.
        /// </remarks>
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8767 // Nullability of reference types in type of parameter 'value' of 'bool Dictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' doesn't match implicitly implemented member 'bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' (possibly because of nullability attributes).
        public bool TryGetValue([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter 'value' of 'bool Dictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' doesn't match implicitly implemented member 'bool IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)' (possibly because of nullability attributes).
#pragma warning restore IDE0079 // Remove unnecessary suppression
        {
            ref TValue valRef = ref FindValue(key);
            if (!UnsafeHelpers.IsNullRef(ref valRef))
            {
                value = valRef;
                return true;
            }

            value = default;
            return false;
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);

        /// <summary>
        /// Removes all elements from the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The <see cref="Count"/> property is set to 0, and references to other objects
        /// from elements of the collection are also released.
        /// <para/>
        /// This method is an O(1) operation, since the root of the internal data structures
        /// is simply released for garbage collection.
        /// </remarks>
        public void Clear()
        {
            int count = _count;
            if (count > 0)
            {
                Debug.Assert(_buckets != null, "_buckets should be non-null");
                Debug.Assert(_entries != null, "_entries should be non-null");

#if FEATURE_ARRAY_CLEAR_ARRAY
                Array.Clear(_buckets);
#else
                Array.Clear(_buckets, 0, _buckets!.Length);
#endif

                _count = 0;
                _freeList = -1;
                _freeCount = 0;
                Array.Clear(_entries, 0, count);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            ref TValue value = ref FindValue(item.Key);
            if (!UnsafeHelpers.IsNullRef(ref value) && EqualityComparer<TValue>.Default.Equals(value, item.Value))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Copies the elements of the <see cref="Dictionary{TKey, TValue}"/> to the specified array
        /// of <see cref="KeyValuePair{TKey, TValue}"/> structures, starting at the specified index.
        /// </summary>
        /// <param name="array">The one-dimensional array of <see cref="KeyValuePair{TKey, TValue}"/> structures
        /// that is the destination of the elements copied from the current <see cref="Dictionary{TKey, TValue}"/>.
        /// The array must have zero-based indexing.</param>
        /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index) // J2N NOTE: This API as private upstream, but low priority to fix.
        {
            if (array is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if ((uint)index > (uint)array.Length)
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
            if (array.Length - index < Count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

            int count = _count;
            Entry[]? entries = _entries;
            for (int i = 0; i < count; i++)
            {
                if (entries![i].next >= -1)
                {
                    array[index++] = new KeyValuePair<TKey, TValue>(entries[i].key!, entries[i].value!);
                }
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            ref TValue value = ref FindValue(item.Key);
            if (!UnsafeHelpers.IsNullRef(ref value) && EqualityComparer<TValue>.Default.Equals(value, item.Value))
            {
                Remove(item.Key);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the element with the specified key from the <see cref="Dictionary{TKey, TValue}"/>.
        /// If the element exists, the associated <paramref name="value"/> is output after it is removed.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The value of the element before it is removed.</param>
        /// <returns><c>true</c> if the element is successfully removed; otherwise, <c>false</c>.
        /// This method also returns <c>false</c> if key is not found in the <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// If the <see cref="Dictionary{TKey, TValue}"/> does not contain an element with the specified key, the
        /// <see cref="Dictionary{TKey, TValue}"/> remains unchanged. No exception is thrown.
        /// </remarks>
        // J2N: This is an extension method on IDictionary<TKey, TValue>, but only for .NET Standard 2.1+.
        // It is redefined here to ensure we have it in prior platforms.
        public bool Remove([AllowNull] TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            // This overload is a copy of the overload Remove(TKey key) with one additional
            // statement to copy the value for entry being removed into the output parameter.
            // Code has been intentionally duplicated for performance reasons.

            // J2N supports null keys

            if (_buckets != null)
            {
                Debug.Assert(_entries != null, "entries should be non-null");
                uint collisionCount = 0;

                IEqualityComparer<TKey>? comparer = _comparer;
                Debug.Assert(typeof(TKey).IsValueType || comparer is not null);
                uint hashCode = (uint)(typeof(TKey).IsValueType && comparer == null ? key?.GetHashCode() ?? 0 : key is not null ? comparer!.GetHashCode(key) : 0);

                ref int bucket = ref GetBucket(hashCode);
                Entry[]? entries = _entries;
                int last = -1;
                int i = bucket - 1; // Value in buckets is 1-based
                if (key is not null)
                {
                    while (i >= 0)
                    {
                        ref Entry entry = ref entries![i];

                        if (entry.hashCode == hashCode &&
                            (typeof(TKey).IsValueType && comparer == null ? EqualityComparer<TKey>.Default.Equals(entry.key, key) : comparer!.Equals(entry.key, key)))
                        {
                            if (last < 0)
                            {
                                bucket = entry.next + 1; // Value in buckets is 1-based
                            }
                            else
                            {
                                entries[last].next = entry.next;
                            }

                            value = entry.value;

                            Debug.Assert((StartOfFreeList - _freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                            entry.next = StartOfFreeList - _freeList;

                            if (RuntimeHelper.IsReferenceOrContainsReferences<TKey>())
                            {
                                entry.key = default!;
                            }

                            if (RuntimeHelper.IsReferenceOrContainsReferences<TValue>())
                            {
                                entry.value = default!;
                            }

                            _freeList = i;
                            _freeCount++;
                            return true;
                        }

                        last = i;
                        i = entry.next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
                else
                {
                    while (i >= 0)
                    {
                        ref Entry entry = ref entries![i];

                        if (entry.hashCode == hashCode && entry.key is null)
                        {
                            if (last < 0)
                            {
                                bucket = entry.next + 1; // Value in buckets is 1-based
                            }
                            else
                            {
                                entries[last].next = entry.next;
                            }

                            value = entry.value;

                            Debug.Assert((StartOfFreeList - _freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                            entry.next = StartOfFreeList - _freeList;

                            if (RuntimeHelper.IsReferenceOrContainsReferences<TKey>())
                            {
                                entry.key = default!;
                            }

                            if (RuntimeHelper.IsReferenceOrContainsReferences<TValue>())
                            {
                                entry.value = default!;
                            }

                            _freeList = i;
                            _freeCount++;
                            return true;
                        }

                        last = i;
                        i = entry.next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
            }

            value = default;
            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>A <see cref="IEnumerator{T}"/> for the
        /// <see cref="Dictionary{TKey, TValue}"/>.</returns>
        /// <remarks>
        /// For purposes of enumeration, each item is a <see cref="KeyValuePair{TKey, TValue}"/> structure
        /// representing a value and its key.
        /// <para/>
        /// The <c>foreach</c> statement of the C# language (<c>for each</c> in C++, <c>For Each</c> in Visual Basic)
        /// hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended instead of directly manipulating the enumerator.
        /// <para/>
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// <para/>
        /// The dictionary is maintained in a sorted order using an internal tree. Every new element is positioned at the correct sort position,
        /// and the tree is adjusted to maintain the sort order whenever an element is removed. While enumerating, the sort order is maintained.
        /// <para/>
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, the
        /// <see cref="IEnumerator{T}.Current"/> property is undefined. Therefore, you must call the
        /// <see cref="IEnumerator.MoveNext()"/> method to advance the enumerator to the first element
        /// of the collection before reading the value of <see cref="IEnumerator{T}.Current"/>.
        /// <para/>
        /// The <see cref="IEnumerator{T}.Current"/> property returns the same object until
        /// <see cref="IEnumerator.MoveNext()"/> is called. <see cref="IEnumerator.MoveNext()"/>
        /// sets <see cref="IEnumerator{T}.Current"/> to the next element.
        /// <para/>
        /// If <see cref="IEnumerator.MoveNext()"/> passes the end of the collection, the enumerator is
        /// positioned after the last element in the collection and <see cref="IEnumerator.MoveNext()"/>
        /// returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="IEnumerator.MoveNext()"/>
        /// also return <c>false</c>. If the last call to <see cref="IEnumerator.MoveNext()"/> returned <c>false</c>,
        /// <see cref="IEnumerator{T}.Current"/> is undefined. You cannot set <see cref="IEnumerator{T}.Current"/>
        /// to the first element of the collection again; you must create a new enumerator object instead.
        /// <para/>
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
        /// such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call
        /// to <see cref="IEnumerator.MoveNext()"/> or <see cref="IEnumerator.Reset()"/> throws an
        /// <see cref="InvalidOperationException"/>.
        /// <para/>
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is
        /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the
        /// collection during the entire enumeration. To allow the collection to be accessed by multiple threads for
        /// reading and writing, you must implement your own synchronization.
        /// <para/>
        /// Default implementations of collections in the <see cref="J2N.Collections.Generic"/> namespace are not synchronized.
        /// <para/>
        /// This method is an O(1) operation.
        /// </remarks>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => new Enumerator(this, Enumerator.KeyValuePair);

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() =>
            Count == 0 ? GenericEmptyEnumerator<KeyValuePair<TKey, TValue>>.Instance :
            GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, TValue>>)this).GetEnumerator();

        #endregion IDictionary<TKey, TValue> Members

        #region IDictionary Members

        bool IDictionary.IsFixedSize => false;

        ICollection IDictionary.Keys => (ICollection)Keys;

        ICollection IDictionary.Values => (ICollection)Values;

        bool IDictionary.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        object? IDictionary.this[object? key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    ref TValue value = ref FindValue((TKey)key!);
                    if (!UnsafeHelpers.IsNullRef(ref value))
                    {
                        return value;
                    }
                }
                return null;
            }
            set
            {
                // J2N: Only throw if the generic closing type is not nullable
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TKey>(key, ExceptionArgument.key);
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);

                try
                {
                    TKey tempKey = (TKey)key!; // [!]: checked above

                    try
                    {
                        this[tempKey] = (TValue)value!; // [!]: checked above
                    }
                    catch (InvalidCastException)
                    {
                        ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                    }
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
                }
            }
        }

        void IDictionary.Add(object? key, object? value)
        {
            // J2N: Only throw if the generic closing type is not nullable
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TKey>(key, ExceptionArgument.key);
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);

            try
            {
                TKey tempKey = (TKey)key!; // [!]: checked above

                try
                {
                    Add(tempKey, (TValue)value!); // [!]: checked above
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                }
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
            }
        }

        bool IDictionary.Contains(object? key)
        {
            if (IsCompatibleKey(key))
            {
                return ContainsKey((TKey)key!); // [!]: checked above
            }
            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() => new Enumerator(this, Enumerator.DictEntry);

        void IDictionary.Remove(object? key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key!); // [!]: checked above
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            if (array.Rank != 1)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            if (array.GetLowerBound(0) != 0)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
            if ((uint)index > (uint)array.Length)
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
            if (array.Length - index < Count)
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

            if (array is KeyValuePair<TKey, TValue>[] pairs)
            {
                CopyTo(pairs, index);
            }
            else if (array is DictionaryEntry[] dictEntryArray)
            {
                Entry[]? entries = _entries;
                for (int i = 0; i < _count; i++)
                {
                    if (entries![i].next >= -1)
                    {
                        dictEntryArray[index++] = new DictionaryEntry(entries[i].key!, entries[i].value);
                    }
                }
            }
            else
            {
                object[]? objects = array as object[];
                if (objects == null)
                {
                    ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                }

                try
                {
                    int count = _count;
                    Entry[]? entries = _entries;
                    for (int i = 0; i < count; i++)
                    {
                        if (entries![i].next >= -1)
                        {
                            objects[index++] = new KeyValuePair<TKey, TValue>(entries[i].key!, entries[i].value!);
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                }
            }
        }

        private static bool IsCompatibleKey(object? key)
        {
            if (key is null)
                return default(TKey) == null;

            return (key is TKey);
        }

        #endregion IDictionary Members

#if FEATURE_IREADONLYCOLLECTIONS
        #region IReadOnlyDictionary<TKey, TValue> Members

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        #endregion IReadOnlyDictionary<TKey, TValue> Members
#endif

        #region Hash Table

        internal ref TValue FindValue([AllowNull] TKey key)
        {
            // J2N supports null keys

            ref Entry entry = ref UnsafeHelpers.NullRef<Entry>();
            if (_buckets != null)
            {
                Debug.Assert(_entries != null, "expected entries to be != null");
                IEqualityComparer<TKey>? comparer = _comparer;
                if (typeof(TKey).IsValueType && // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
                    comparer == null)
                {
                    uint hashCode = (uint)(key?.GetHashCode() ?? 0);
                    int i = GetBucket(hashCode);
                    Entry[]? entries = _entries;
                    uint collisionCount = 0;

                    // ValueType: Devirtualize with EqualityComparer<TKey>.Default intrinsic
                    i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                    if (key is not null)
                    {
                        do
                        {
                            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                            // Test in if to drop range check for following array access
                            if ((uint)i >= (uint)entries!.Length)
                            {
                                goto ReturnNotFound;
                            }

                            entry = ref entries[i];
                            if (entry.hashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entry.key, key))
                            {
                                goto ReturnFound;
                            }

                            i = entry.next;

                            collisionCount++;
                        } while (collisionCount <= (uint)entries.Length);
                    }
                    else
                    {
                        do
                        {
                            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                            // Test in if to drop range check for following array access
                            if ((uint)i >= (uint)entries!.Length)
                            {
                                goto ReturnNotFound;
                            }

                            entry = ref entries[i];
                            if (entry.hashCode == hashCode && entry.key is null)
                            {
                                goto ReturnFound;
                            }

                            i = entry.next;

                            collisionCount++;
                        } while (collisionCount <= (uint)entries.Length);
                    }

                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    goto ConcurrentOperation;
                }
                else
                {
                    Debug.Assert(comparer is not null);
                    if (key is not null)
                    {
                        uint hashCode = (uint)comparer!.GetHashCode(key);
                        int i = GetBucket(hashCode);
                        Entry[]? entries = _entries;
                        uint collisionCount = 0;
                        i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                        do
                        {
                            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                            // Test in if to drop range check for following array access
                            if ((uint)i >= (uint)entries!.Length)
                            {
                                goto ReturnNotFound;
                            }

                            entry = ref entries[i];
                            if (entry.hashCode == hashCode && comparer.Equals(entry.key, key))
                            {
                                goto ReturnFound;
                            }

                            i = entry.next;

                            collisionCount++;
                        } while (collisionCount <= (uint)entries.Length);
                    }
                    else
                    {
                        uint hashCode = 0;
                        int i = GetBucket(hashCode);
                        Entry[]? entries = _entries;
                        uint collisionCount = 0;
                        i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                        do
                        {
                            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                            // Test in if to drop range check for following array access
                            if ((uint)i >= (uint)entries!.Length)
                            {
                                goto ReturnNotFound;
                            }

                            entry = ref entries[i];
                            if (entry.hashCode == hashCode && key is null)
                            {
                                goto ReturnFound;
                            }

                            i = entry.next;

                            collisionCount++;
                        } while (collisionCount <= (uint)entries.Length);
                    }

                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    goto ConcurrentOperation;
                }
            }

            goto ReturnNotFound;

        ConcurrentOperation:
            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
        ReturnFound:
            ref TValue value = ref entry.value;
        Return:
            return ref value;
        ReturnNotFound:
            value = ref UnsafeHelpers.NullRef<TValue>();
            goto Return;
        }

        private int Initialize(int capacity)
        {
            int size = HashHelpers.GetPrime(capacity);
            int[] buckets = new int[size];
            Entry[] entries = new Entry[size];

            // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
            _freeList = -1;
            if (IntPtr.Size == 8) // 64-bit process
            {
                _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)size);
            }
            _buckets = buckets;
            _entries = entries;

            return size;
        }

        private bool TryInsert([AllowNull] TKey key, [AllowNull] TValue value, InsertionBehavior behavior)
        {
            // NOTE: this method is mirrored in CollectionsMarshal.GetValueRefOrAddDefault below.
            // If you make any changes here, make sure to keep that version in sync as well.

            // J2N supports null keys

            if (_buckets == null)
            {
                Initialize(0);
            }
            Debug.Assert(_buckets != null);

            Entry[]? entries = _entries;
            Debug.Assert(entries != null, "expected entries to be non-null");

            IEqualityComparer<TKey>? comparer = _comparer;
            Debug.Assert(comparer is not null || typeof(TKey).IsValueType);
            uint hashCode = (uint)((typeof(TKey).IsValueType && comparer == null) ? key?.GetHashCode() ?? 0 : key is not null ? comparer!.GetHashCode(key) : 0);

            uint collisionCount = 0;
            ref int bucket = ref GetBucket(hashCode);
            int i = bucket - 1; // Value in _buckets is 1-based

            if (typeof(TKey).IsValueType && // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
                comparer == null)
            {
                // ValueType: Devirtualize with EqualityComparer<TKey>.Default intrinsic
                if (key is not null)
                {
                    while (true)
                    {
                        // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                        // Test uint in if rather than loop condition to drop range check for following array access
                        if ((uint)i >= (uint)entries!.Length)
                        {
                            break;
                        }

                        if (entries[i].hashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entries[i].key!, key))
                        {
                            if (behavior == InsertionBehavior.OverwriteExisting)
                            {
                                entries[i].value = value;
                                return true;
                            }

                            if (behavior == InsertionBehavior.ThrowOnExisting)
                            {
                                ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
                            }

                            return false;
                        }

                        i = entries[i].next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                        // Test uint in if rather than loop condition to drop range check for following array access
                        if ((uint)i >= (uint)entries!.Length)
                        {
                            break;
                        }

                        if (entries[i].hashCode == hashCode && entries[i].key is null)
                        {
                            if (behavior == InsertionBehavior.OverwriteExisting)
                            {
                                entries[i].value = value;
                                return true;
                            }

                            if (behavior == InsertionBehavior.ThrowOnExisting)
                            {
                                ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException("(null)");
                            }

                            return false;
                        }

                        i = entries[i].next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
            }
            else
            {
                if (key is not null)
                {
                    Debug.Assert(comparer is not null);
                    while (true)
                    {
                        // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                        // Test uint in if rather than loop condition to drop range check for following array access
                        if ((uint)i >= (uint)entries!.Length)
                        {
                            break;
                        }

                        if (entries[i].hashCode == hashCode && comparer!.Equals(entries[i].key!, key))
                        {
                            if (behavior == InsertionBehavior.OverwriteExisting)
                            {
                                entries[i].value = value;
                                return true;
                            }

                            if (behavior == InsertionBehavior.ThrowOnExisting)
                            {
                                ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
                            }

                            return false;
                        }

                        i = entries[i].next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                        // Test uint in if rather than loop condition to drop range check for following array access
                        if ((uint)i >= (uint)entries!.Length)
                        {
                            break;
                        }

                        if (entries[i].hashCode == hashCode && entries[i].key is null)
                        {
                            if (behavior == InsertionBehavior.OverwriteExisting)
                            {
                                entries[i].value = value;
                                return true;
                            }

                            if (behavior == InsertionBehavior.ThrowOnExisting)
                            {
                                ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException("(null)");
                            }

                            return false;
                        }

                        i = entries[i].next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
            }

            int index;
            if (_freeCount > 0)
            {
                index = _freeList;
                Debug.Assert((StartOfFreeList - entries[_freeList].next) >= -1, "shouldn't overflow because `next` cannot underflow");
                _freeList = StartOfFreeList - entries[_freeList].next;
                _freeCount--;
            }
            else
            {
                int count = _count;
                if (count == entries.Length)
                {
                    Resize();
                    bucket = ref GetBucket(hashCode);
                }
                index = count;
                _count = count + 1;
                entries = _entries;
            }

            ref Entry entry = ref entries![index];
            entry.hashCode = hashCode;
            entry.next = bucket - 1; // Value in _buckets is 1-based
            entry.key = key;
            entry.value = value;
            bucket = index + 1; // Value in _buckets is 1-based
            _version++;

            // Value types never rehash

            if (!typeof(TKey).IsValueType && collisionCount > HashHelpers.HashCollisionThreshold && comparer is NonRandomizedStringEqualityComparer)
            {
                // If we hit the collision threshold we'll need to switch to the comparer which is using randomized string hashing
                // i.e. EqualityComparer<string>.Default.
                Resize(entries.Length, true);
            }

            return true;
        }

        private void Resize() => Resize(HashHelpers.ExpandPrime(_count), false);

        private void Resize(int newSize, bool forceNewHashCodes)
        {
            // Value types never rehash
            Debug.Assert(!forceNewHashCodes || !typeof(TKey).IsValueType);
            Debug.Assert(_entries != null, "_entries should be non-null");
            Debug.Assert(newSize >= _entries!.Length);

            Entry[] entries = new Entry[newSize];

            int count = _count;
            Array.Copy(_entries, entries, count);

            if (!typeof(TKey).IsValueType && forceNewHashCodes)
            {
                Debug.Assert(_comparer is NonRandomizedStringEqualityComparer);
                IEqualityComparer<TKey> comparer = _comparer = (IEqualityComparer<TKey>)((NonRandomizedStringEqualityComparer)_comparer!).GetRandomizedEqualityComparer();

                for (int i = 0; i < count; i++)
                {
                    if (entries[i].next >= -1)
                    {
                        TKey? key = entries[i].key;
                        entries[i].hashCode = key is not null ? (uint)comparer.GetHashCode(key) : 0;
                    }
                }
            }

            // Assign member variables after both arrays allocated to guard against corruption from OOM if second fails
            _buckets = new int[newSize];
            if (IntPtr.Size == 8) // 64-bit process
            {
                _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)newSize);
            }
            for (int i = 0; i < count; i++)
            {
                if (entries[i].next >= -1)
                {
                    ref int bucket = ref GetBucket(entries[i].hashCode);
                    entries[i].next = bucket - 1; // Value in _buckets is 1-based
                    bucket = i + 1;
                }
            }

            _entries = entries;
        }

        private void CopyEntries(Entry[] entries, int count)
        {
            Debug.Assert(_entries is not null);

            Entry[] newEntries = _entries!; // [!]: asserted above
            int newCount = 0;
            for (int i = 0; i < count; i++)
            {
                uint hashCode = entries[i].hashCode;
                if (entries[i].next >= -1)
                {
                    ref Entry entry = ref newEntries[newCount];
                    entry = entries[i];
                    ref int bucket = ref GetBucket(hashCode);
                    entry.next = bucket - 1; // Value in _buckets is 1-based
                    bucket = newCount + 1;
                    newCount++;
                }
            }

            _count = newCount;
            _freeCount = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref int GetBucket(uint hashCode)
        {
            int[] buckets = _buckets!;

            if (IntPtr.Size == 8) // 64-bit process
            {
                return ref buckets[HashHelpers.FastMod(hashCode, (uint)buckets.Length, _fastModMultiplier)];
            }
            else
            {
                return ref buckets[(uint)hashCode % buckets.Length];
            }
        }

        #endregion Hash Table

        #region AlternateLookup

#if FEATURE_IALTERNATEEQUALITYCOMPARER

        /// <summary>
        /// Gets an instance of a type that may be used to perform operations on the current <see cref="Dictionary{TKey, TValue}"/>
        /// using a <typeparamref name="TAlternateKey"/> as a key instead of a <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TAlternateKey">The alternate type of a key for performing lookups.</typeparam>
        /// <returns>The created lookup instance.</returns>
        /// <exception cref="InvalidOperationException">The dictionary's comparer is not compatible with <typeparamref name="TAlternateKey"/>.</exception>
        /// <remarks>
        /// The dictionary must be using a comparer that implements <see cref="IAlternateEqualityComparer{TAlternateKey, TKey}"/> with
        /// <typeparamref name="TAlternateKey"/> and <typeparamref name="TKey"/>. If it doesn't, an exception will be thrown.
        /// </remarks>
        public AlternateLookup<TAlternateKey> GetAlternateLookup<TAlternateKey>()
            where TAlternateKey : allows ref struct
        {
            if (!AlternateLookup<TAlternateKey>.IsCompatibleKey(this))
            {
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_IncompatibleComparer);
            }

            return new AlternateLookup<TAlternateKey>(this);
        }

        /// <summary>
        /// Gets an instance of a type that may be used to perform operations on the current <see cref="Dictionary{TKey, TValue}"/>
        /// using a <typeparamref name="TAlternateKey"/> as a key instead of a <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TAlternateKey">The alternate type of a key for performing lookups.</typeparam>
        /// <param name="lookup">The created lookup instance when the method returns true, or a default instance that should not be used if the method returns false.</param>
        /// <returns>true if a lookup could be created; otherwise, false.</returns>
        /// <remarks>
        /// The dictionary must be using a comparer that implements <see cref="IAlternateEqualityComparer{TAlternateKey, TKey}"/> with
        /// <typeparamref name="TAlternateKey"/> and <typeparamref name="TKey"/>. If it doesn't, the method will return false.
        /// </remarks>
        public bool TryGetAlternateLookup<TAlternateKey>(
            out AlternateLookup<TAlternateKey> lookup)
            where TAlternateKey : allows ref struct
        {
            if (AlternateLookup<TAlternateKey>.IsCompatibleKey(this))
            {
                lookup = new AlternateLookup<TAlternateKey>(this);
                return true;
            }

            lookup = default;
            return false;
        }

        /// <summary>
        /// Provides a type that may be used to perform operations on a <see cref="Dictionary{TKey, TValue}"/>
        /// using a <typeparamref name="TAlternateKey"/> as a key instead of a <typeparamref name="TKey"/>.
        /// </summary>
        /// <typeparam name="TAlternateKey">The alternate type of a key for performing lookups.</typeparam>
        public readonly struct AlternateLookup<TAlternateKey> where TAlternateKey : allows ref struct
        {
            /// <summary>Initialize the instance. The dictionary must have already been verified to have a compatible comparer.</summary>
            internal AlternateLookup(Dictionary<TKey, TValue> dictionary)
            {
                Debug.Assert(dictionary is not null);
                Debug.Assert(IsCompatibleKey(dictionary));
                Dictionary = dictionary;
            }

            /// <summary>Gets the <see cref="Dictionary{TKey, TValue}"/> against which this instance performs operations.</summary>
            public Dictionary<TKey, TValue> Dictionary { get; }

            /// <summary>Gets or sets the value associated with the specified alternate key.</summary>
            /// <param name="key">The alternate key of the value to get or set.</param>
            /// <value>
            /// The value associated with the specified alternate key. If the specified alternate key is not found, a get operation throws
            /// a <see cref="KeyNotFoundException"/>, and a set operation creates a new element with the specified key.
            /// </value>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            /// <exception cref="KeyNotFoundException">The property is retrieved and alternate key does not exist in the collection.</exception>
            public TValue this[[AllowNull] TAlternateKey key]
            {
                get
                {
                    ref TValue value = ref FindValue(key, out _);
                    if (Unsafe.IsNullRef(ref value))
                    {
                        ThrowHelper.ThrowKeyNotFoundException(GetAlternateComparer(Dictionary).Create(key));
                    }

                    return value;
                }
                set => GetValueRefOrAddDefault(key, out _) = value;
            }

            /// <summary>Checks whether the dictionary has a comparer compatible with <typeparamref name="TAlternateKey"/>.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static bool IsCompatibleKey(Dictionary<TKey, TValue> dictionary)
            {
                Debug.Assert(dictionary is not null);
                return dictionary._comparer is IAlternateEqualityComparer<TAlternateKey, TKey>;
            }

            /// <summary>Gets the dictionary's alternate comparer. The dictionary must have already been verified as compatible.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static IAlternateEqualityComparer<TAlternateKey, TKey> GetAlternateComparer(Dictionary<TKey, TValue> dictionary)
            {
                Debug.Assert(IsCompatibleKey(dictionary));
                return Unsafe.As<IAlternateEqualityComparer<TAlternateKey, TKey>>(dictionary._comparer)!;
            }

            /// <summary>Gets the value associated with the specified alternate key.</summary>
            /// <param name="key">The alternate key of the value to get.</param>
            /// <param name="value">
            /// When this method returns, contains the value associated with the specified key, if the key is found;
            /// otherwise, the default value for the type of the value parameter.
            /// </param>
            /// <returns><see langword="true"/> if an entry was found; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool TryGetValue([AllowNull] TAlternateKey key, [MaybeNullWhen(false)] out TValue value)
            {
                ref TValue valueRef = ref FindValue(key, out _);
                if (!Unsafe.IsNullRef(ref valueRef))
                {
                    value = valueRef;
                    return true;
                }

                value = default;
                return false;
            }

            /// <summary>Gets the value associated with the specified alternate key.</summary>
            /// <param name="key">The alternate key of the value to get.</param>
            /// <param name="actualKey">
            /// When this method returns, contains the actual key associated with the alternate key, if the key is found;
            /// otherwise, the default value for the type of the key parameter.
            /// </param>
            /// <param name="value">
            /// When this method returns, contains the value associated with the specified key, if the key is found;
            /// otherwise, the default value for the type of the value parameter.
            /// </param>
            /// <returns><see langword="true"/> if an entry was found; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool TryGetValue([AllowNull] TAlternateKey key, [MaybeNullWhen(false)] out TKey actualKey, [MaybeNullWhen(false)] out TValue value)
            {
                ref TValue valueRef = ref FindValue(key, out actualKey);
                if (!Unsafe.IsNullRef(ref valueRef))
                {
                    value = valueRef;
                    Debug.Assert(actualKey is not null);
                    return true;
                }

                value = default;
                return false;
            }

            /// <summary>Determines whether the <see cref="Dictionary{TKey, TValue}"/> contains the specified alternate key.</summary>
            /// <param name="key">The alternate key to check.</param>
            /// <returns><see langword="true"/> if the key is in the dictionary; otherwise, <see langword="false"/>.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool ContainsKey([AllowNull] TAlternateKey key) =>
                !Unsafe.IsNullRef(ref FindValue(key, out _));

            /// <summary>Finds the entry associated with the specified alternate key.</summary>
            /// <param name="key">The alternate key.</param>
            /// <param name="actualKey">The actual key, if found.</param>
            /// <returns>A reference to the value associated with the key, if found; otherwise, a null reference.</returns>
            internal ref TValue FindValue([AllowNull] TAlternateKey key, [MaybeNullWhen(false)] out TKey actualKey)
            {
                Dictionary<TKey, TValue> dictionary = Dictionary;

                ref Entry entry = ref Unsafe.NullRef<Entry>();
                if (dictionary._buckets != null)
                {
                    Debug.Assert(dictionary._entries != null, "expected entries to be != null");

                    if (key is not null)
                    {
                        IAlternateEqualityComparer<TAlternateKey, TKey> comparer = GetAlternateComparer(dictionary); // J2N: Moved within null check, since we don't need to look this up for null keys

                        uint hashCode = (uint)comparer.GetHashCode(key);
                        int i = dictionary.GetBucket(hashCode);
                        Entry[]? entries = dictionary._entries;
                        uint collisionCount = 0;
                        i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                        do
                        {
                            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                            // Test in if to drop range check for following array access
                            if ((uint)i >= (uint)entries.Length)
                            {
                                goto ReturnNotFound;
                            }

                            entry = ref entries[i];
                            if (entry.hashCode == hashCode && comparer.Equals(key, entry.key))
                            {
                                goto ReturnFound;
                            }

                            i = entry.next;

                            collisionCount++;
                        } while (collisionCount <= (uint)entries.Length);
                    }
                    else
                    {
                        uint hashCode = 0;
                        int i = dictionary.GetBucket(hashCode);
                        Entry[]? entries = dictionary._entries;
                        uint collisionCount = 0;
                        i--; // Value in _buckets is 1-based; subtract 1 from i. We do it here so it fuses with the following conditional.
                        do
                        {
                            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                            // Test in if to drop range check for following array access
                            if ((uint)i >= (uint)entries.Length)
                            {
                                goto ReturnNotFound;
                            }

                            entry = ref entries[i];
                            if (entry.hashCode == hashCode && entry.key is null)
                            {
                                goto ReturnFound;
                            }

                            i = entry.next;

                            collisionCount++;
                        } while (collisionCount <= (uint)entries.Length);
                    }

                    // The chain of entries forms a loop; which means a concurrent update has happened.
                    // Break out of the loop and throw, rather than looping forever.
                    goto ConcurrentOperation;
                }

                goto ReturnNotFound;

            ConcurrentOperation:
                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
            ReturnFound:
                ref TValue value = ref entry.value;
                actualKey = entry.key;
            Return:
                return ref value;
            ReturnNotFound:
                value = ref Unsafe.NullRef<TValue>();
                actualKey = default!;
                goto Return;
            }

            /// <summary>Removes the value with the specified alternate key from the <see cref="Dictionary{TKey, TValue}"/>.</summary>
            /// <param name="key">The alternate key of the element to remove.</param>
            /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool Remove([AllowNull] TAlternateKey key) =>
                Remove(key, out _, out _);

            /// <summary>
            /// Removes the value with the specified alternate key from the <see cref="Dictionary{TKey, TValue}"/>,
            /// and copies the element to the value parameter.
            /// </summary>
            /// <param name="key">The alternate key of the element to remove.</param>
            /// <param name="actualKey">The removed key.</param>
            /// <param name="value">The removed element.</param>
            /// <returns>true if the element is successfully found and removed; otherwise, false.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool Remove([AllowNull] TAlternateKey key, [MaybeNullWhen(false)] out TKey actualKey, [MaybeNullWhen(false)] out TValue value)
            {
                Dictionary<TKey, TValue> dictionary = Dictionary;

                if (dictionary._buckets != null)
                {
                    Debug.Assert(dictionary._entries != null, "entries should be non-null");
                    uint collisionCount = 0;

                    if (key is not null)
                    {
                        IAlternateEqualityComparer<TAlternateKey, TKey> comparer = GetAlternateComparer(dictionary); // J2N: Moved within null check, since we don't need to look this up for null keys

                        uint hashCode = (uint)comparer.GetHashCode(key);

                        ref int bucket = ref dictionary.GetBucket(hashCode);
                        Entry[]? entries = dictionary._entries;
                        int last = -1;
                        int i = bucket - 1; // Value in buckets is 1-based
                        while (i >= 0)
                        {
                            ref Entry entry = ref entries[i];

                            if (entry.hashCode == hashCode && comparer.Equals(key, entry.key))
                            {
                                if (last < 0)
                                {
                                    bucket = entry.next + 1; // Value in buckets is 1-based
                                }
                                else
                                {
                                    entries[last].next = entry.next;
                                }

                                actualKey = entry.key;
                                value = entry.value;

                                Debug.Assert((StartOfFreeList - dictionary._freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                                entry.next = StartOfFreeList - dictionary._freeList;

                                if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
                                {
                                    entry.key = default!;
                                }

                                if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
                                {
                                    entry.value = default!;
                                }

                                dictionary._freeList = i;
                                dictionary._freeCount++;
                                return true;
                            }

                            last = i;
                            i = entry.next;

                            collisionCount++;
                            if (collisionCount > (uint)entries.Length)
                            {
                                // The chain of entries forms a loop; which means a concurrent update has happened.
                                // Break out of the loop and throw, rather than looping forever.
                                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                            }
                        }
                    }
                    else
                    {
                        uint hashCode = 0;

                        ref int bucket = ref dictionary.GetBucket(hashCode);
                        Entry[]? entries = dictionary._entries;
                        int last = -1;
                        int i = bucket - 1; // Value in buckets is 1-based
                        while (i >= 0)
                        {
                            ref Entry entry = ref entries[i];

                            if (entry.hashCode == hashCode && entry.key is null)
                            {
                                if (last < 0)
                                {
                                    bucket = entry.next + 1; // Value in buckets is 1-based
                                }
                                else
                                {
                                    entries[last].next = entry.next;
                                }

                                actualKey = entry.key;
                                value = entry.value;

                                Debug.Assert((StartOfFreeList - dictionary._freeList) < 0, "shouldn't underflow because max hashtable length is MaxPrimeArrayLength = 0x7FEFFFFD(2146435069) _freelist underflow threshold 2147483646");
                                entry.next = StartOfFreeList - dictionary._freeList;

                                if (RuntimeHelpers.IsReferenceOrContainsReferences<TKey>())
                                {
                                    entry.key = default!;
                                }

                                if (RuntimeHelpers.IsReferenceOrContainsReferences<TValue>())
                                {
                                    entry.value = default!;
                                }

                                dictionary._freeList = i;
                                dictionary._freeCount++;
                                return true;
                            }

                            last = i;
                            i = entry.next;

                            collisionCount++;
                            if (collisionCount > (uint)entries.Length)
                            {
                                // The chain of entries forms a loop; which means a concurrent update has happened.
                                // Break out of the loop and throw, rather than looping forever.
                                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                            }
                        }
                    }
                }

                actualKey = default;
                value = default;
                return false;
            }

            /// <summary>Attempts to add the specified key and value to the dictionary.</summary>
            /// <param name="key">The alternate key of the element to add.</param>
            /// <param name="value">The value of the element to add.</param>
            /// <returns>true if the key/value pair was added to the dictionary successfully; otherwise, false.</returns>
            /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
            public bool TryAdd([AllowNull] TAlternateKey key, TValue value)
            {
                ref TValue? slot = ref GetValueRefOrAddDefault(key, out bool exists);
                if (!exists)
                {
                    slot = value;
                    return true;
                }

                return false;
            }

            /// <inheritdoc cref="CollectionMarshal.GetValueRefOrAddDefault{TKey, TValue}(Dictionary{TKey, TValue}, TKey, out bool)"/>
            internal ref TValue? GetValueRefOrAddDefault([AllowNull] TAlternateKey key, out bool exists)
            {
                // NOTE: this method is a mirror of GetValueRefOrAddDefault above. Keep it in sync.

                Dictionary<TKey, TValue> dictionary = Dictionary;
                IAlternateEqualityComparer<TAlternateKey, TKey> comparer = GetAlternateComparer(dictionary);

                if (dictionary._buckets == null)
                {
                    dictionary.Initialize(0);
                }
                Debug.Assert(dictionary._buckets != null);

                Entry[]? entries = dictionary._entries;
                Debug.Assert(entries != null, "expected entries to be non-null");

                uint hashCode = (uint)(key is not null ? comparer.GetHashCode(key) : 0);

                uint collisionCount = 0;
                ref int bucket = ref dictionary.GetBucket(hashCode);
                int i = bucket - 1; // Value in _buckets is 1-based

                Debug.Assert(comparer is not null);
                if (key is not null)
                {
                    while ((uint)i < (uint)entries.Length)
                    {
                        if (entries[i].hashCode == hashCode && comparer.Equals(key, entries[i].key!))
                        {
                            exists = true;

                            return ref entries[i].value!;
                        }

                        i = entries[i].next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }
                else
                {
                    while ((uint)i < (uint)entries.Length)
                    {
                        if (entries[i].hashCode == hashCode && entries[i].key is null)
                        {
                            exists = true;

                            return ref entries[i].value!;
                        }

                        i = entries[i].next;

                        collisionCount++;
                        if (collisionCount > (uint)entries.Length)
                        {
                            // The chain of entries forms a loop; which means a concurrent update has happened.
                            // Break out of the loop and throw, rather than looping forever.
                            ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                        }
                    }
                }

                TKey? actualKey = key is not null ? comparer.Create(key) : default;

                // J2N allows null keys

                int index;
                if (dictionary._freeCount > 0)
                {
                    index = dictionary._freeList;
                    Debug.Assert((StartOfFreeList - entries[dictionary._freeList].next) >= -1, "shouldn't overflow because `next` cannot underflow");
                    dictionary._freeList = StartOfFreeList - entries[dictionary._freeList].next;
                    dictionary._freeCount--;
                }
                else
                {
                    int count = dictionary._count;
                    if (count == entries.Length)
                    {
                        dictionary.Resize();
                        bucket = ref dictionary.GetBucket(hashCode);
                    }
                    index = count;
                    dictionary._count = count + 1;
                    entries = dictionary._entries;
                }

                ref Entry entry = ref entries![index];
                entry.hashCode = hashCode;
                entry.next = bucket - 1; // Value in _buckets is 1-based
                entry.key = actualKey;
                entry.value = default!;
                bucket = index + 1; // Value in _buckets is 1-based
                dictionary._version++;

                // Value types never rehash
                if (!typeof(TKey).IsValueType && collisionCount > HashHelpers.HashCollisionThreshold && comparer is NonRandomizedStringEqualityComparer)
                {
                    // If we hit the collision threshold we'll need to switch to the comparer which is using randomized string hashing
                    // i.e. EqualityComparer<string>.Default.
                    dictionary.Resize(entries.Length, true);

                    exists = false;

                    // At this point the entries array has been resized, so the current reference we have is no longer valid.
                    // We're forced to do a new lookup and return an updated reference to the new entry instance. This new
                    // lookup is guaranteed to always find a value though and it will never return a null reference here.
                    ref TValue? value = ref dictionary.FindValue(actualKey)!;

                    Debug.Assert(!Unsafe.IsNullRef(ref value), "the lookup result cannot be a null ref here");

                    return ref value;
                }

                exists = false;

                return ref entry.value!;
            }
        }

#endif

        #endregion AlternateLookup

        #region Structural Equality

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current dictionary
        /// using rules provided by the specified <paramref name="comparer"/>.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to determine
        /// whether the current object and <paramref name="other"/> are structurally equal.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is structurally equal to the current dictionary;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
        public virtual bool Equals(object? other, IEqualityComparer comparer)
            => DictionaryEqualityComparer<TKey, TValue>.Equals(this, other, comparer);

        /// <summary>
        /// Gets the hash code representing the current dictionary using rules specified by the
        /// provided <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to generate
        /// the hash code.</param>
        /// <returns>A hash code representing the current dictionary.</returns>
        public virtual int GetHashCode(IEqualityComparer comparer)
            => DictionaryEqualityComparer<TKey, TValue>.GetHashCode(this, comparer);

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current dictionary
        /// using rules similar to those in the JDK's AbstractMap class. Two dictionaries are considered
        /// equal when they both contain the same mappings (in any order).
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object implements <see cref="IDictionary{TKey, TValue}"/>
        /// and it contains the same elements; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Equals(object, IEqualityComparer)"/>
        public override bool Equals(object? obj)
            => Equals(obj, DictionaryEqualityComparer<TKey, TValue>.Default);

        /// <summary>
        /// Gets the hash code for the current dictionary. The hash code is calculated
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <seealso cref="GetHashCode(IEqualityComparer)"/>
        public override int GetHashCode()
            => GetHashCode(DictionaryEqualityComparer<TKey, TValue>.Default);

        #endregion Structural Equality

        #region ToString

        /// <summary>
        /// Returns a string that represents the current dictionary using the specified
        /// <paramref name="format"/> and <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current dictionary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is not zero.
        /// </exception>
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
            => CollectionUtil.ToString(formatProvider, format, this);

        /// <summary>
        /// Returns a string that represents the current dictionary using
        /// <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by curly
        /// brackets ("{}"). Keys and values are separated by '=',
        /// KeyValuePairs are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current list.</returns>
        public override string ToString()
            => ToString("{0}", StringFormatter.CurrentCulture);


        /// <summary>
        /// Returns a string that represents the current dictionary using the specified
        /// <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current dictionary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="formatProvider"/> is <c>null</c>.</exception>
        public virtual string ToString(IFormatProvider formatProvider)
            => ToString("{0}", formatProvider);

        /// <summary>
        /// Returns a string that represents the current dictionary using the specified
        /// <paramref name="format"/> and <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by curly
        /// brackets ("{}"). Keys and values are separated by '=',
        /// KeyValuePairs are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current dictionary.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is not zero.
        /// </exception>
        public virtual string ToString(string format)
            => ToString(format, StringFormatter.CurrentCulture);

        #endregion ToString

        #region Nested Class: KeyCollection

        /// <summary>
        /// Represents the collection of keys in a <see cref="Dictionary{TKey, TValue}"/>. This class cannot be inherited.
        /// </summary>
        /// <remarks>
        /// The <see cref="Dictionary{TKey, TValue}.Keys"/> property returns an instance of this type, containing all
        /// the keys in that <see cref="Dictionary{TKey, TValue}"/>. The order of the keys in the
        /// <see cref="Dictionary{TKey, TValue}.KeyCollection"/> is unspecified, but it is the same order as the
        /// associated values in the <see cref="Dictionary{TKey, TValue}.ValueCollection"/> returned by the
        /// <see cref="Dictionary{TKey, TValue}.Values"/> property.
        /// <para/>
        /// The <see cref="Dictionary{TKey, TValue}.KeyCollection"/> is not a static copy; instead, the
        /// <see cref="Dictionary{TKey, TValue}.KeyCollection"/> refers back to the keys in the original
        /// <see cref="Dictionary{TKey, TValue}"/>. Therefore, changes to the <see cref="Dictionary{TKey, TValue}"/>
        /// continue to be reflected in the <see cref="Dictionary{TKey, TValue}.KeyCollection"/>.
        /// </remarks>
#if FEATURE_SERIALIZABLE
        [Serializable] // J2N TODO: API - remove (BCL doesn't do this)
#endif
        [DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class KeyCollection : ICollection<TKey>, ICollection
#if FEATURE_IREADONLYCOLLECTIONS
            , IReadOnlyCollection<TKey>
#endif
        {
            private readonly Dictionary<TKey, TValue> dictionary;

            /// <summary>
            /// Initializes a new instance of the <see cref="Dictionary{TKey,TValue}.KeyCollection"/>
            /// class that reflects the keys in the specified <see cref="Dictionary{TKey,TValue}"/>.
            /// </summary>
            /// <param name="dictionary">The <see cref="Dictionary{TKey,TValue}"/> whose keys are
            /// reflected in the new <see cref="Dictionary{TKey,TValue}.KeyCollection"/>.</param>
            /// <exception cref="ArgumentNullException"></exception>
            public KeyCollection(Dictionary<TKey, TValue> dictionary)
            {
                if (dictionary is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                this.dictionary = dictionary;
            }

            /// <summary>
            ///  Gets the number of elements contained in the <see cref="KeyCollection"/>.
            /// </summary>
            /// <value>
            /// The number of elements contained in the <see cref="Dictionary{TKey,TValue}.KeyCollection"/>.
            /// <para/>
            /// Retrieving the value of this property is an O(1) operation.
            /// </value>
            public int Count => dictionary.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)dictionary).SyncRoot;

            void ICollection<TKey>.Add(TKey item)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            void ICollection<TKey>.Clear()
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);

            bool ICollection<TKey>.Contains([AllowNull] TKey item)
            {
                return dictionary.ContainsKey(item);
            }

            /// <summary>
            /// Copies the <see cref="Dictionary{TKey, TValue}.KeyCollection"/> elements to an
            /// existing one-dimensional <see cref="Array"/>, starting at the specified array index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination
            /// of the elements copied from <see cref="Dictionary{TKey, TValue}.KeyCollection"/>.
            /// The <see cref="Array"/> must have zero-based indexing.</param>
            /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
            /// <exception cref="ArgumentException">The number of elements in the source <see cref="Dictionary{TKey, TValue}.KeyCollection"/>
            /// is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.</exception>
            /// <remarks>
            /// The elements are copied to the <see cref="Array"/> in the same order in which the enumerator iterates through
            /// the <see cref="Dictionary{TKey, TValue}.KeyCollection"/>.
            /// <para/>
            /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
            /// </remarks>
            public void CopyTo(TKey[] array, int index)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (array is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                if ((uint)index > (uint)array.Length)
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
                if (array.Length - index < Count)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

                int count = dictionary._count;
                Entry[]? entries = dictionary._entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries![i].next >= -1) array[index++] = entries[i].key!;
                }
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="Dictionary{TKey, TValue}.KeyCollection"/>.
            /// </summary>
            /// <returns>A <see cref="Dictionary{TKey, TValue}.KeyCollection.Enumerator"/> for the <see cref="Dictionary{TKey, TValue}.KeyCollection"/>.</returns>
            /// <remarks>
            /// The <c>foreach</c> statement of the C# language (<c>for each</c> in C++, <c>For Each</c> in Visual Basic)
            /// hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended instead of directly manipulating the enumerator.
            /// <para/>
            /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
            /// <para/>
            /// Initially, the enumerator is positioned before the first element in the collection. At this position, the
            /// <see cref="Enumerator.Current"/> property is undefined. Therefore, you must call the
            /// <see cref="Enumerator.MoveNext()"/> method to advance the enumerator to the first element
            /// of the collection before reading the value of <see cref="Enumerator.Current"/>.
            /// <para/>
            /// The <see cref="Enumerator.Current"/> property returns the same object until
            /// <see cref="Enumerator.MoveNext()"/> is called. <see cref="Enumerator.MoveNext()"/>
            /// sets <see cref="Enumerator.Current"/> to the next element.
            /// <para/>
            /// If <see cref="Enumerator.MoveNext()"/> passes the end of the collection, the enumerator is
            /// positioned after the last element in the collection and <see cref="Enumerator.MoveNext()"/>
            /// returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="Enumerator.MoveNext()"/>
            /// also return <c>false</c>. If the last call to <see cref="Enumerator.MoveNext()"/> returned <c>false</c>,
            /// <see cref="Enumerator.Current"/> is undefined. You cannot set <see cref="Enumerator.Current"/>
            /// to the first element of the collection again; you must create a new enumerator object instead.
            /// <para/>
            /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
            /// such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call
            /// to <see cref="Enumerator.MoveNext()"/> or <see cref="IEnumerator.Reset()"/> throws an
            /// <see cref="InvalidOperationException"/>.
            /// <para/>
            /// The only mutating methods which do not invalidate enumerators are <see cref="Remove(TKey)"/>,
            /// <see cref="Remove(TKey, out TValue)"/> and <see cref="Clear"/>.
            /// <para/>
            /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is
            /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the
            /// collection during the entire enumeration. To allow the collection to be accessed by multiple threads for
            /// reading and writing, you must implement your own synchronization.
            /// <para/>
            /// Default implementations of collections in the <see cref="J2N.Collections.Generic"/> namespace are not synchronized.
            /// <para/>
            /// This method is an O(1) operation.
            /// </remarks>
            public Enumerator GetEnumerator() => new Enumerator(dictionary);

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator() =>
               Count == 0 ? SZGenericArrayEnumerator<TKey>.Empty :
               GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TKey>)this).GetEnumerator();

            bool ICollection<TKey>.Remove(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
                return false;
            }
            void ICollection.CopyTo(Array array, int index)
            {
                if (array is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                if (array.Rank != 1)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                if (array.GetLowerBound(0) != 0)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                if ((uint)index > (uint)array.Length)
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
                if (array.Length - index < dictionary.Count)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

                if (array is TKey[] keys)
                {
                    CopyTo(keys, index);
                }
                else
                {
                    object[]? objects = array as object[];
                    if (objects == null)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                    }

                    int count = dictionary._count;
                    Entry[]? entries = dictionary._entries;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries![i].next >= -1) objects[index++] = entries[i].key!;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                    }
                }
            }

            #region Nested Structure: Enumerator

            /// <summary>
            /// Enumerates the elements of a <see cref="Dictionary{TKey,TValue}.KeyCollection"/>.
            /// </summary>
            /// <remarks>
            /// The <c>foreach</c> statement of the C# language (<c>for each</c> in C++, <c>For Each</c> in Visual Basic)
            /// hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended instead of directly manipulating the enumerator.
            /// <para/>
            /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
            /// <para/>
            /// Initially, the enumerator is positioned before the first element in the collection. At this position, the
            /// <see cref="Enumerator.Current"/> property is undefined. Therefore, you must call the
            /// <see cref="Enumerator.MoveNext()"/> method to advance the enumerator to the first element
            /// of the collection before reading the value of <see cref="Enumerator.Current"/>.
            /// <para/>
            /// The <see cref="Enumerator.Current"/> property returns the same object until
            /// <see cref="Enumerator.MoveNext()"/> is called. <see cref="Enumerator.MoveNext()"/>
            /// sets <see cref="Enumerator.Current"/> to the next element.
            /// <para/>
            /// If <see cref="Enumerator.MoveNext()"/> passes the end of the collection, the enumerator is
            /// positioned after the last element in the collection and <see cref="Enumerator.MoveNext()"/>
            /// returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="Enumerator.MoveNext()"/>
            /// also return <c>false</c>. If the last call to <see cref="Enumerator.MoveNext()"/> returned <c>false</c>,
            /// <see cref="Enumerator.Current"/> is undefined. You cannot set <see cref="Enumerator.Current"/>
            /// to the first element of the collection again; you must create a new enumerator object instead.
            /// <para/>
            /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
            /// such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call
            /// to <see cref="Enumerator.MoveNext()"/> or <see cref="IEnumerator.Reset()"/> throws an
            /// <see cref="InvalidOperationException"/>.
            /// <para/>
            /// The only mutating methods which do not invalidate enumerators are <see cref="Remove(TKey)"/>,
            /// <see cref="Remove(TKey, out TValue)"/> and <see cref="Clear"/>.
            /// <para/>
            /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is
            /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the
            /// collection during the entire enumeration. To allow the collection to be accessed by multiple threads for
            /// reading and writing, you must implement your own synchronization.
            /// <para/>
            /// Default implementations of collections in the <see cref="J2N.Collections.Generic"/> namespace are not synchronized.
            /// </remarks>
#if FEATURE_SERIALIZABLE
            [Serializable] // J2N TODO: API - remove (BCL doesn't do this)
#endif
            public struct Enumerator : IEnumerator<TKey>, IEnumerator
            {
                private readonly Dictionary<TKey, TValue> dictionary;
                private int index;
                private readonly int version;
                private TKey? currentKey;
                internal Enumerator(Dictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary._version;
                    index = 0;
                    currentKey = default;
                }

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                /// <value>
                /// The element in the <see cref="Dictionary{TKey,TValue}.KeyCollection"/> at
                /// the current position of the enumerator.</value>
                /// <remarks>
                /// <see cref="Current"/> is undefined under any of the following conditions:
                /// <list type="bullet">
                ///     <item><description>The enumerator is positioned before the first element of the collection. That happens
                ///         after an enumerator is created or after the <see cref="IEnumerator.Reset"/> method is called.
                ///         The <see cref="MoveNext"/> method must be called to advance the enumerator to the first element
                ///         of the collection before reading the value of the <see cref="Current"/> property.</description></item>
                ///     <item><description>The last call to <see cref="MoveNext"/> returned <c>false</c>, which indicates the
                ///         end of the collection and that the enumerator is positioned after the last element of the
                ///         collection.</description></item>
                ///     <item><description>The enumerator is invalidated due to changes made in the collection, such as adding,
                ///         modifying, or deleting elements.</description></item>
                /// </list>
                /// <para/>
                /// <see cref="Current"/> does not move the position of the enumerator, and consecutive calls to
                /// <see cref="Current"/> return the same object until either <see cref="MoveNext"/> or <see cref="IEnumerator.Reset"/>
                /// is called.
                /// </remarks>
                public TKey Current => currentKey!;

                object? IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (index == dictionary._count + 1))
                        {
                            ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                        }

                        return currentKey;
                    }
                }

                /// <summary>
                /// Releases all resources used by the <see cref="Dictionary{TKey, TValue}.KeyCollection.Enumerator"/>.
                /// </summary>
                public void Dispose() { /* Intentionally blank */ }

                /// <summary>
                /// Advances the enumerator to the next element of the <see cref="Dictionary{TKey, TValue}.KeyCollection"/>.
                /// </summary>
                /// <returns><c>true</c> if the enumerator was successfully advanced to the next element;
                /// <c>false</c> if the enumerator has passed the end of the collection.</returns>
                /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
                /// <remarks>
                /// After an enumerator is created, the enumerator is positioned before the first element in the collection,
                /// and the first call to <see cref="MoveNext"/> advances the enumerator to the first element of the collection.
                /// <para/>
                /// If <see cref="MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element
                /// in the collection and <see cref="MoveNext"/> returns <c>false</c>. When the enumerator is at this position,
                /// subsequent calls to <see cref="MoveNext"/> also return <c>false</c>.
                /// <para/>
                /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
                /// such as adding elements or changing the capacity, the enumerator is irrecoverably invalidated and the next
                /// call to <see cref="MoveNext"/> or <see cref="IEnumerator.Reset"/> throws an <see cref="InvalidOperationException"/>.
                /// <para/>
                /// The only mutating methods which do not invalidate enumerators are <see cref="Remove(TKey)"/>,
                /// <see cref="Remove(TKey, out TValue)"/> and <see cref="Clear"/>.
                /// </remarks>
                public bool MoveNext()
                {
                    if (version != dictionary._version)
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();

                    while ((uint)index < (uint)dictionary._count)
                    {
                        ref Entry entry = ref dictionary._entries![index++];

                        if (entry.next >= -1)
                        {
                            currentKey = entry.key;
                            return true;
                        }
                    }

                    index = dictionary._count + 1;
                    currentKey = default;
                    return false;
                }

                void IEnumerator.Reset()
                {
                    if (version != dictionary._version)
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();

                    index = 0;
                    currentKey = default;
                }
            }

            #endregion
        }

        #endregion

        #region Nested Class: ValueCollection

        /// <summary>
        /// Represents the collection of values in a <see cref="Dictionary{TKey, TValue}"/>. This class cannot be inherited.
        /// </summary>
        /// <remarks>
        /// The <see cref="Dictionary{TKey, TValue}.Values"/> property returns an instance of this type, containing
        /// all the values in that <see cref="Dictionary{TKey, TValue}"/>. The order of the values in the
        /// <see cref="Dictionary{TKey, TValue}.ValueCollection"/> is unspecified, but it is the same order as
        /// the associated keys in the <see cref="Dictionary{TKey, TValue}.KeyCollection"/> returned by the
        /// <see cref="Dictionary{TKey, TValue}.Keys"/> property.
        /// <para/>
        /// The <see cref="Dictionary{TKey, TValue}.ValueCollection"/> is not a static copy; instead, the
        /// <see cref="Dictionary{TKey, TValue}.ValueCollection"/> refers back to the values in the original
        /// <see cref="Dictionary{TKey, TValue}"/>. Therefore, changes to the <see cref="Dictionary{TKey, TValue}"/>
        /// continue to be reflected in the <see cref="Dictionary{TKey, TValue}.ValueCollection"/>.
        /// </remarks>
#if FEATURE_SERIALIZABLE
        [Serializable] // J2N TODO: API - remove (BCL doesn't do this)
#endif
        [DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class ValueCollection : ICollection<TValue>, ICollection
#if FEATURE_IREADONLYCOLLECTIONS
            , IReadOnlyCollection<TValue>
#endif
        {
            private readonly Dictionary<TKey, TValue> dictionary;

            /// <summary>
            /// Initializes a new instance of the <see cref="Dictionary{TKey, TValue}.ValueCollection"/> class
            /// that reflects the values in the specified <see cref="Dictionary{TKey, TValue}"/>.
            /// </summary>
            /// <param name="dictionary">The <see cref="Dictionary{TKey, TValue}"/> whose values are reflected
            /// in the new <see cref="Dictionary{TKey, TValue}.ValueCollection"/>.</param>
            /// <exception cref="ArgumentNullException"><paramref name="dictionary"/> is <c>null</c>.</exception>
            /// <remarks>
            /// The <see cref="Dictionary{TKey, TValue}.ValueCollection"/> is not a static copy; instead,
            /// the <see cref="Dictionary{TKey, TValue}.ValueCollection"/> refers back to the values in the
            /// original <see cref="Dictionary{TKey, TValue}"/>. Therefore, changes to the <see cref="Dictionary{TKey, TValue}"/>
            /// continue to be reflected in the <see cref="Dictionary{TKey, TValue}.ValueCollection"/>.
            /// <para/>
            /// This constructor is an O(1) operation.
            /// </remarks>
            public ValueCollection(Dictionary<TKey, TValue> dictionary)
            {
                if (dictionary is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                this.dictionary = dictionary;
            }

            /// <summary>
            /// Gets the number of elements contained in the <see cref="Dictionary{TKey, TValue}.ValueCollection"/>.
            /// </summary>
            /// <value>
            /// The number of elements contained in the <see cref="Dictionary{TKey, TValue}.ValueCollection"/>.
            /// </value>
            /// <remarks>
            /// Retrieving the value of this property is an O(1) operation.
            /// </remarks>
            public int Count => dictionary.Count;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)dictionary).SyncRoot;

            bool ICollection<TValue>.IsReadOnly => true;

            void ICollection<TValue>.Add(TValue item)
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            void ICollection<TValue>.Clear()
                => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);

            bool ICollection<TValue>.Contains([AllowNull] TValue item) => dictionary.ContainsValue(item);

            void ICollection.CopyTo(Array array, int index)
            {
                if (array is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                if (array.Rank != 1)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                if (array.GetLowerBound(0) != 0)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                if ((uint)index > (uint)array.Length)
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
                if (array.Length - index < dictionary.Count)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

                if (array is TValue[] values)
                {
                    CopyTo(values, index);
                }
                else
                {
                    object[]? objects = array as object[];
                    if (objects == null)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                    }

                    int count = dictionary._count;
                    Entry[]? entries = dictionary._entries;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            if (entries![i].next >= -1) objects[index++] = entries[i].value!;
                        }
                    }
                    catch (ArrayTypeMismatchException)
                    {
                        ThrowHelper.ThrowArgumentException_Argument_IncompatibleArrayType();
                    }
                }
            }

            /// <summary>
            /// Copies the <see cref="Dictionary{TKey,TValue}.ValueCollection"/> elements to an
            /// existing one-dimensional <see cref="Array"/>, starting at the specified array index.
            /// </summary>
            /// <param name="array">The one-dimensional <see cref="Array"/> that is the destination of the elements
            /// copied from <see cref="Dictionary{TKey,TValue}.ValueCollection"/>. The <see cref="Array"/> must have
            /// zero-based indexing.</param>
            /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
            /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
            /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.</exception>
            /// <exception cref="ArgumentException">The number of elements in the source <see cref="Dictionary{TKey,TValue}.ValueCollection"/>
            /// is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.</exception>
            /// <remarks>
            /// The elements are copied to the <see cref="Array"/> in the same order in which the enumerator iterates through
            /// the <see cref="Dictionary{TKey,TValue}.ValueCollection"/>.
            /// <para/>
            /// This method is an O(<c>n</c>) operation, where <c>n</c> is <see cref="Count"/>.
            /// </remarks>
            public void CopyTo(TValue[] array, int index)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (array is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                if ((uint)index > (uint)array.Length)
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
                if (array.Length - index < dictionary.Count)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);

                int count = dictionary._count;
                Entry[]? entries = dictionary._entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries![i].next >= -1) array[index++] = entries[i].value!;
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<TValue>)this).GetEnumerator();

            bool ICollection<TValue>.Remove(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
                return false;
            }

            /// <summary>
            /// Returns an enumerator that iterates through the <see cref="Dictionary{TKey,TValue}.ValueCollection"/>.
            /// </summary>
            /// <returns>A <see cref="Dictionary{TKey,TValue}.ValueCollection.Enumerator"/> for the
            /// <see cref="Dictionary{TKey,TValue}.ValueCollection"/>.</returns>
            /// <remarks>
            /// The <c>foreach</c> statement of the C# language (<c>for each</c> in C++, <c>For Each</c> in Visual Basic)
            /// hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended instead of directly manipulating the enumerator.
            /// <para/>
            /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
            /// <para/>
            /// Initially, the enumerator is positioned before the first element in the collection. At this position, the
            /// <see cref="Enumerator.Current"/> property is undefined. Therefore, you must call the
            /// <see cref="Enumerator.MoveNext()"/> method to advance the enumerator to the first element
            /// of the collection before reading the value of <see cref="Enumerator.Current"/>.
            /// <para/>
            /// The <see cref="Enumerator.Current"/> property returns the same object until
            /// <see cref="Enumerator.MoveNext()"/> is called. <see cref="Enumerator.MoveNext()"/>
            /// sets <see cref="Enumerator.Current"/> to the next element.
            /// <para/>
            /// If <see cref="Enumerator.MoveNext()"/> passes the end of the collection, the enumerator is
            /// positioned after the last element in the collection and <see cref="Enumerator.MoveNext()"/>
            /// returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="Enumerator.MoveNext()"/>
            /// also return <c>false</c>. If the last call to <see cref="Enumerator.MoveNext()"/> returned <c>false</c>,
            /// <see cref="Enumerator.Current"/> is undefined. You cannot set <see cref="Enumerator.Current"/>
            /// to the first element of the collection again; you must create a new enumerator object instead.
            /// <para/>
            /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
            /// such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call
            /// to <see cref="Enumerator.MoveNext()"/> or <see cref="IEnumerator.Reset()"/> throws an
            /// <see cref="InvalidOperationException"/>.
            /// <para/>
            /// The only mutating methods which do not invalidate enumerators are <see cref="Remove(TKey)"/>,
            /// <see cref="Remove(TKey, out TValue)"/> and <see cref="Clear"/>.
            /// <para/>
            /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is
            /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the
            /// collection during the entire enumeration. To allow the collection to be accessed by multiple threads for
            /// reading and writing, you must implement your own synchronization.
            /// <para/>
            /// Default implementations of collections in the <see cref="J2N.Collections.Generic"/> namespace are not synchronized.
            /// <para/>
            /// This method is an O(1) operation.
            /// </remarks>
            public Enumerator GetEnumerator() => new Enumerator(dictionary);

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator() =>
                Count == 0 ? SZGenericArrayEnumerator<TValue>.Empty :
                GetEnumerator();

            #region Nested Structure: Enumerator

            /// <summary>
            /// Enumerates the elements of a <see cref="Dictionary{TKey,TValue}.ValueCollection"/>.
            /// </summary>
            /// <remarks>
            /// The <c>foreach</c> statement of the C# language (<c>for each</c> in C++, <c>For Each</c> in Visual Basic)
            /// hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended instead of directly manipulating the enumerator.
            /// <para/>
            /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
            /// <para/>
            /// Initially, the enumerator is positioned before the first element in the collection. At this position, the
            /// <see cref="Enumerator.Current"/> property is undefined. Therefore, you must call the
            /// <see cref="Enumerator.MoveNext()"/> method to advance the enumerator to the first element
            /// of the collection before reading the value of <see cref="Enumerator.Current"/>.
            /// <para/>
            /// The <see cref="Enumerator.Current"/> property returns the same object until
            /// <see cref="Enumerator.MoveNext()"/> is called. <see cref="Enumerator.MoveNext()"/>
            /// sets <see cref="Enumerator.Current"/> to the next element.
            /// <para/>
            /// If <see cref="Enumerator.MoveNext()"/> passes the end of the collection, the enumerator is
            /// positioned after the last element in the collection and <see cref="Enumerator.MoveNext()"/>
            /// returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="Enumerator.MoveNext()"/>
            /// also return <c>false</c>. If the last call to <see cref="Enumerator.MoveNext()"/> returned <c>false</c>,
            /// <see cref="Enumerator.Current"/> is undefined. You cannot set <see cref="Enumerator.Current"/>
            /// to the first element of the collection again; you must create a new enumerator object instead.
            /// <para/>
            /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
            /// such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call
            /// to <see cref="Enumerator.MoveNext()"/> or <see cref="IEnumerator.Reset()"/> throws an
            /// <see cref="InvalidOperationException"/>.
            /// <para/>
            /// The only mutating methods which do not invalidate enumerators are <see cref="Remove(TKey)"/>,
            /// <see cref="Remove(TKey, out TValue)"/> and <see cref="Clear"/>.
            /// <para/>
            /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is
            /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the
            /// collection during the entire enumeration. To allow the collection to be accessed by multiple threads for
            /// reading and writing, you must implement your own synchronization.
            /// <para/>
            /// Default implementations of collections in the <see cref="J2N.Collections.Generic"/> namespace are not synchronized.
            /// </remarks>
#if FEATURE_SERIALIZABLE
            [Serializable] // J2N TODO: API - remove (BCL doesn't do this)
#endif
            public struct Enumerator : IEnumerator<TValue>, IEnumerator
            {
                private readonly Dictionary<TKey, TValue> dictionary;
                private int index;
                private readonly int version;
                private TValue? currentValue;
                internal Enumerator(Dictionary<TKey, TValue> dictionary)
                {
                    this.dictionary = dictionary;
                    version = dictionary._version;
                    index = 0;
                    currentValue = default;
                }

                /// <summary>
                /// Gets the element at the current position of the enumerator.
                /// </summary>
                /// <value>
                /// The element in the <see cref="Dictionary{TKey,TValue}.ValueCollection"/> at the current position
                /// of the enumerator.
                /// </value>
                /// <remarks>
                /// <see cref="Current"/> is undefined under any of the following conditions:
                /// <list type="bullet">
                ///     <item><description>The enumerator is positioned before the first element of the collection. That happens
                ///         after an enumerator is created or after the <see cref="IEnumerator.Reset"/> method is called.
                ///         The <see cref="MoveNext"/> method must be called to advance the enumerator to the first element
                ///         of the collection before reading the value of the <see cref="Current"/> property.</description></item>
                ///     <item><description>The last call to <see cref="MoveNext"/> returned <c>false</c>, which indicates the
                ///         end of the collection and that the enumerator is positioned after the last element of the
                ///         collection.</description></item>
                ///     <item><description>The enumerator is invalidated due to changes made in the collection, such as adding,
                ///         modifying, or deleting elements.</description></item>
                /// </list>
                /// <para/>
                /// <see cref="Current"/> does not move the position of the enumerator, and consecutive calls to
                /// <see cref="Current"/> return the same object until either <see cref="MoveNext"/> or <see cref="IEnumerator.Reset"/>
                /// is called.
                /// </remarks>
                public TValue Current => currentValue!;

                object? IEnumerator.Current
                {
                    get
                    {
                        if (index == 0 || (index == dictionary._count + 1))
                        {
                            ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                        }

                        return currentValue;
                    }
                }

                /// <summary>
                /// Releases all resources used by the <see cref="Dictionary{TKey, TValue}.ValueCollection.Enumerator"/>.
                /// </summary>
                public void Dispose() { /* Intentionally blank */ }

                /// <summary>
                /// Advances the enumerator to the next element of the <see cref="Dictionary{TKey, TValue}.ValueCollection"/>.
                /// </summary>
                /// <returns><c>true</c> if the enumerator was successfully advanced to the next element;
                /// <c>false</c> if the enumerator has passed the end of the collection.</returns>
                /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
                /// <remarks>
                /// After an enumerator is created, the enumerator is positioned before the first element in the collection,
                /// and the first call to <see cref="MoveNext"/> advances the enumerator to the first element of the collection.
                /// <para/>
                /// If <see cref="MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element
                /// in the collection and <see cref="MoveNext"/> returns <c>false</c>. When the enumerator is at this position,
                /// subsequent calls to <see cref="MoveNext"/> also return <c>false</c>.
                /// <para/>
                /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
                /// such as adding elements or changing the capacity, the enumerator is irrecoverably invalidated and the next
                /// call to <see cref="MoveNext"/> or <see cref="IEnumerator.Reset"/> throws an <see cref="InvalidOperationException"/>.
                /// <para/>
                /// The only mutating methods which do not invalidate enumerators are <see cref="Remove(TKey)"/>,
                /// <see cref="Remove(TKey, out TValue)"/> and <see cref="Clear"/>.
                /// </remarks>
                public bool MoveNext()
                {
                    if (version != dictionary._version)
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();

                    while ((uint)index < (uint)dictionary._count)
                    {
                        ref Entry entry = ref dictionary._entries![index++];

                        if (entry.next >= -1)
                        {
                            currentValue = entry.value;
                            return true;
                        }
                    }
                    index = dictionary._count + 1;
                    currentValue = default;
                    return false;
                }

                void IEnumerator.Reset()
                {
                    if (version != dictionary._version)
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();

                    index = 0;
                    currentValue = default;
                }
            }

            #endregion
        }

        #endregion

        #region Nested Structure: Enumerator

        /// <summary>
        /// Enumerates the elemensts of a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <remarks>
        /// The <c>foreach</c> statement of the C# language (<c>for each</c> in C++, <c>For Each</c> in Visual Basic)
        /// hides the complexity of enumerators. Therefore, using <c>foreach</c> is recommended instead of directly manipulating the enumerator.
        /// <para/>
        /// Enumerators can be used to read the data in the collection, but they cannot be used to modify the underlying collection.
        /// <para/>
        /// Initially, the enumerator is positioned before the first element in the collection. At this position, the
        /// <see cref="Enumerator.Current"/> property is undefined. Therefore, you must call the
        /// <see cref="Enumerator.MoveNext()"/> method to advance the enumerator to the first element
        /// of the collection before reading the value of <see cref="Enumerator.Current"/>.
        /// <para/>
        /// The <see cref="Enumerator.Current"/> property returns the same object until
        /// <see cref="Enumerator.MoveNext()"/> is called. <see cref="Enumerator.MoveNext()"/>
        /// sets <see cref="Enumerator.Current"/> to the next element.
        /// <para/>
        /// If <see cref="Enumerator.MoveNext()"/> passes the end of the collection, the enumerator is
        /// positioned after the last element in the collection and <see cref="Enumerator.MoveNext()"/>
        /// returns <c>false</c>. When the enumerator is at this position, subsequent calls to <see cref="Enumerator.MoveNext()"/>
        /// also return <c>false</c>. If the last call to <see cref="Enumerator.MoveNext()"/> returned <c>false</c>,
        /// <see cref="Enumerator.Current"/> is undefined. You cannot set <see cref="Enumerator.Current"/>
        /// to the first element of the collection again; you must create a new enumerator object instead.
        /// <para/>
        /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
        /// such as adding, modifying, or deleting elements, the enumerator is irrecoverably invalidated and the next call
        /// to <see cref="Enumerator.MoveNext()"/> or <see cref="IEnumerator.Reset()"/> throws an
        /// <see cref="InvalidOperationException"/>.
        /// <para/>
        /// The enumerator does not have exclusive access to the collection; therefore, enumerating through a collection is
        /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration, you can lock the
        /// collection during the entire enumeration. To allow the collection to be accessed by multiple threads for
        /// reading and writing, you must implement your own synchronization.
        /// <para/>
        /// Default implementations of collections in the <see cref="J2N.Collections.Generic"/> namespace are not synchronized.
        /// </remarks>
        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
        {
            private readonly Dictionary<TKey, TValue> dictionary;
            private readonly int version;
            private int index;
            private KeyValuePair<TKey, TValue> current;
            private readonly int getEnumeratorRetType;  // What should Enumerator.Current return?

            internal const int DictEntry = 1;
            internal const int KeyValuePair = 2;

            internal Enumerator(Dictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                this.dictionary = dictionary;
                version = dictionary._version;
                index = 0;
                this.getEnumeratorRetType = getEnumeratorRetType;
                current = default;
            }

            /// <summary>
            /// Gets the element at the current position of the enumerator.
            /// </summary>
            /// <value>The element in the <see cref="Dictionary{TKey, TValue}"/> at the current position of the enumerator.</value>
            public KeyValuePair<TKey, TValue> Current => current;

            object? IEnumerator.Current
            {
                get
                {
                    if (index == 0 || (index == dictionary._count + 1))
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();

                    if (getEnumeratorRetType == DictEntry)
                    {
                        return new DictionaryEntry(current.Key!, current.Value);
                    }

                    return new KeyValuePair<TKey, TValue>(current.Key, current.Value);
                }
            }

            /// <summary>
            /// Releases all resources used by the <see cref="Enumerator"/>.
            /// </summary>
            public void Dispose() { /* Intentionally blank */ }

            /// <summary>
            /// Advances the enumerator to the next element of the <see cref="Dictionary{TKey, TValue}"/>.
            /// </summary>
            /// <returns><c>true</c> if the enumerator was successfully advanced to the next element;
            /// <c>false</c> if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="InvalidOperationException">The collection was modified after the enumerator was created.</exception>
            /// <remarks>
            /// After an enumerator is created, the enumerator is positioned before the first element in the collection, and the
            /// first call to <see cref="MoveNext"/> advances the enumerator to the first element of the collection.
            /// <para/>
            /// If <see cref="MoveNext"/> passes the end of the collection, the enumerator is positioned after the last element
            /// in the collection and <see cref="MoveNext"/> returns <c>false</c>. When the enumerator is at this position,
            /// subsequent calls to <see cref="MoveNext"/> also return <c>false</c>.
            /// <para/>
            /// An enumerator remains valid as long as the collection remains unchanged. If changes are made to the collection,
            /// such as adding elements or changing the capacity, the enumerator is irrecoverably invalidated and the next
            /// call to <see cref="MoveNext"/> or <see cref="IEnumerator.Reset()"/> throws an <see cref="InvalidOperationException"/>.
            /// <para/>
            /// The only mutating methods which do not invalidate enumerators are <see cref="Remove(TKey)"/>,
            /// <see cref="Remove(TKey, out TValue)"/> and <see cref="Clear()"/>.
            /// </remarks>
            /// <seealso cref="Current"/>
            public bool MoveNext()
            {
                if (version != dictionary._version)
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();

                // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
                // dictionary.count+1 could be negative if dictionary.count is int.MaxValue
                while ((uint)index < (uint)dictionary._count)
                {
                    ref Entry entry = ref dictionary._entries![index++];

                    if (entry.next >= -1)
                    {
                        current = new KeyValuePair<TKey, TValue>(entry.key, entry.value);
                        return true;
                    }
                }

                index = dictionary._count + 1;
                current = default;
                return false;
            }

            void IEnumerator.Reset()
            {
                if (version != dictionary._version)
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();

                index = 0;
                current = default;
            }

            #region IDictionaryEnumerator Members

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (index == 0 || (index == dictionary._count + 1))
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen(); ;

                    return new DictionaryEntry(current.Key!, current.Value);
                }
            }

            object? IDictionaryEnumerator.Key
            {
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CS8616, CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
                get
#pragma warning restore CS8616, CS8768 // Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
#pragma warning restore IDE0079 // Remove unnecessary suppression
                {
                    if (index == 0 || (index == dictionary._count + 1))
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();

                    return current.Key;
                }
            }

            object? IDictionaryEnumerator.Value
            {
                get
                {
                    if (index == 0 || (index == dictionary._count + 1))
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();

                    return current.Value;
                }
            }

            #endregion
        }

        #endregion

        #region Nested Class: CollectionsMarshalHelper

        /// <summary>
        /// A helper class containing APIs exposed through <see cref="CollectionMarshal"/>.
        /// These methods are relatively niche and only used in specific scenarios, so adding them in a separate type avoids
        /// the additional overhead on each <see cref="Dictionary{TKey, TValue}"/> instantiation, especially in AOT scenarios.
        /// </summary>
        internal static class CollectionsMarshalHelper
        {
            /// <inheritdoc cref="CollectionMarshal.GetValueRefOrAddDefault{TKey, TValue}(Dictionary{TKey, TValue}, TKey, out bool)"/>
            public static ref TValue? GetValueRefOrAddDefault(Dictionary<TKey, TValue> dictionary, [AllowNull] TKey key, out bool exists)
            {
                // NOTE: this method is mirrored by Dictionary<TKey, TValue>.TryInsert above.
                // If you make any changes here, make sure to keep that version in sync as well.

                // J2N supports null keys

                if (dictionary._buckets == null)
                {
                    dictionary.Initialize(0);
                }
                Debug.Assert(dictionary._buckets != null);

                Entry[]? entries = dictionary._entries;
                Debug.Assert(entries != null, "expected entries to be non-null");

                IEqualityComparer<TKey>? comparer = dictionary._comparer;
                Debug.Assert(comparer is not null || typeof(TKey).IsValueType);
                uint hashCode = (uint)((typeof(TKey).IsValueType && comparer == null) ? key?.GetHashCode() ?? 0 : key is not null ? comparer!.GetHashCode(key) : 0);

                uint collisionCount = 0;
                ref int bucket = ref dictionary.GetBucket(hashCode);
                int i = bucket - 1; // Value in _buckets is 1-based

                if (typeof(TKey).IsValueType && // comparer can only be null for value types; enable JIT to eliminate entire if block for ref types
                    comparer == null)
                {
                    // ValueType: Devirtualize with EqualityComparer<TKey>.Default intrinsic
                    if (key is not null)
                    {
                        while (true)
                        {
                            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                            // Test uint in if rather than loop condition to drop range check for following array access
                            if ((uint)i >= (uint)entries!.Length)
                            {
                                break;
                            }

                            if (entries[i].hashCode == hashCode && EqualityComparer<TKey>.Default.Equals(entries[i].key!, key)) // [!]: checked above
                            {
                                exists = true;

                                return ref entries[i].value!;
                            }

                            i = entries[i].next;

                            collisionCount++;
                            if (collisionCount > (uint)entries.Length)
                            {
                                // The chain of entries forms a loop; which means a concurrent update has happened.
                                // Break out of the loop and throw, rather than looping forever.
                                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                            }
                        }
                    }
                    else
                    {
                        while (true)
                        {
                            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                            // Test uint in if rather than loop condition to drop range check for following array access
                            if ((uint)i >= (uint)entries!.Length)
                            {
                                break;
                            }

                            if (entries[i].hashCode == hashCode && entries[i].key is null)
                            {
                                exists = true;

                                return ref entries[i].value!;
                            }

                            i = entries[i].next;

                            collisionCount++;
                            if (collisionCount > (uint)entries.Length)
                            {
                                // The chain of entries forms a loop; which means a concurrent update has happened.
                                // Break out of the loop and throw, rather than looping forever.
                                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                            }
                        }
                    }
                }
                else
                {
                    if (key is not null)
                    {
                        Debug.Assert(comparer is not null);
                        while (true)
                        {
                            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                            // Test uint in if rather than loop condition to drop range check for following array access
                            if ((uint)i >= (uint)entries!.Length)
                            {
                                break;
                            }

                            if (entries[i].hashCode == hashCode && comparer!.Equals(entries[i].key!, key)) // [comparer!]: asserted above, [key!] checked above
                            {
                                exists = true;

                                return ref entries[i].value!;
                            }

                            i = entries[i].next;

                            collisionCount++;
                            if (collisionCount > (uint)entries.Length)
                            {
                                // The chain of entries forms a loop; which means a concurrent update has happened.
                                // Break out of the loop and throw, rather than looping forever.
                                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                            }
                        }
                    }
                    else
                    {

                        while (true)
                        {
                            // Should be a while loop https://github.com/dotnet/runtime/issues/9422
                            // Test uint in if rather than loop condition to drop range check for following array access
                            if ((uint)i >= (uint)entries!.Length)
                            {
                                break;
                            }

                            if (entries[i].hashCode == hashCode && entries[i].key is null)
                            {
                                exists = true;

                                return ref entries[i].value!;
                            }

                            i = entries[i].next;

                            collisionCount++;
                            if (collisionCount > (uint)entries.Length)
                            {
                                // The chain of entries forms a loop; which means a concurrent update has happened.
                                // Break out of the loop and throw, rather than looping forever.
                                ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                            }
                        }
                    }
                }

                int index;
                if (dictionary._freeCount > 0)
                {
                    index = dictionary._freeList;
                    Debug.Assert((StartOfFreeList - entries[dictionary._freeList].next) >= -1, "shouldn't overflow because `next` cannot underflow");
                    dictionary._freeList = StartOfFreeList - entries[dictionary._freeList].next;
                    dictionary._freeCount--;
                }
                else
                {
                    int count = dictionary._count;
                    if (count == entries.Length)
                    {
                        dictionary.Resize();
                        bucket = ref dictionary.GetBucket(hashCode);
                    }
                    index = count;
                    dictionary._count = count + 1;
                    entries = dictionary._entries;
                }

                ref Entry entry = ref entries![index];
                entry.hashCode = hashCode;
                entry.next = bucket - 1; // Value in _buckets is 1-based
                entry.key = key;
                entry.value = default!;
                bucket = index + 1; // Value in _buckets is 1-based
                dictionary._version++;

                // Value types never rehash

                if (!typeof(TKey).IsValueType && collisionCount > HashHelpers.HashCollisionThreshold && comparer is NonRandomizedStringEqualityComparer)
                {
                    // If we hit the collision threshold we'll need to switch to the comparer which is using randomized string hashing
                    // i.e. EqualityComparer<string>.Default.
                    dictionary.Resize(entries.Length, true);

                    exists = false;

                    // At this point the entries array has been resized, so the current reference we have is no longer valid.
                    // We're forced to do a new lookup and return an updated reference to the new entry instance. This new
                    // lookup is guaranteed to always find a value though and it will never return a null reference here.
                    ref TValue? value = ref dictionary.FindValue(key)!;

                    Debug.Assert(!UnsafeHelpers.IsNullRef(ref value), "the lookup result cannot be a null ref here");

                    return ref value;
                }

                exists = false;

                return ref entry.value!;
            }
        }

        #endregion Nested Class: CollectionsMarshalHelper

        #region Nested Structure: Entry

        private struct Entry
        {
            public uint hashCode;
            /// <summary>
            /// 0-based index of next entry in chain: -1 means end of chain
            /// also encodes whether this entry _itself_ is part of the free list by changing sign and subtracting 3,
            /// so -2 means end of free list, -3 means index 0 but on free list, -4 means index 1 but on free list, etc.
            /// </summary>
            public int next;
            [AllowNull, MaybeNull]
            public TKey key;     // Key of entry
            [AllowNull, MaybeNull]
            public TValue value; // Value of entry
        }

        #endregion Nested Structure: Entry
    }
}
