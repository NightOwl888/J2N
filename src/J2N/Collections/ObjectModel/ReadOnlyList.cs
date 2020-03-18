using J2N.Collections.Generic;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#nullable enable

namespace J2N.Collections.ObjectModel
{
    /// <summary>
    /// Provides the base class for a generic read-only list that is structurally equatable.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
    /// <remarks>
    /// Public static (Shared in Visual Basic) members of this type are thread safe.
    /// Any instance members are not guaranteed to be thread safe.
    /// <para/>
    /// A <see cref="ReadOnlyList{T}"/> can support multiple readers concurrently, as long
    /// as the collection is not modified. Even so, enumerating through a collection is
    /// intrinsically not a thread-safe procedure. To guarantee thread safety during enumeration,
    /// you can lock the collection during the entire enumeration. To allow the collection to be
    /// accessed by multiple threads for reading and writing, you must implement your own synchronization.
    /// </remarks>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class ReadOnlyList<T> : System.Collections.ObjectModel.ReadOnlyCollection<T>, IStructuralEquatable, IStructuralFormattable
    {
#if NET40
        private static readonly bool TIsValueTypeOrStringOrStructuralEquatable = typeof(T).IsValueType || typeof(IStructuralEquatable).IsAssignableFrom(typeof(T)) || typeof(string).Equals(typeof(T));
#else
        private static readonly bool TIsValueTypeOrStringOrStructuralEquatable = typeof(T).GetTypeInfo().IsValueType || typeof(IStructuralEquatable).GetTypeInfo().IsAssignableFrom(typeof(T).GetTypeInfo()) || typeof(string).Equals(typeof(T));
#endif

        private readonly ListEqualityComparer<T> structuralEqualityComparer;
        private readonly IFormatProvider toStringFormatProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList{T}"/> class that is a read-only wrapper around the specified list.
        /// </summary>
        /// <param name="list">The list to wrap.</param>
        /// <see cref="ArgumentNullException"><paramref name="list"/> is <c>null</c>.</see>
        /// <remarks>
        /// To prevent any modifications to <paramref name="list"/>, expose <paramref name="list"/> only through this wrapper.
        /// <para/>
        /// A collection that is read-only is simply a collection with a wrapper that prevents modifying the collection;
        /// therefore, if changes are made to the underlying collection, the read-only collection reflects those changes.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public ReadOnlyList(IList<T> list)
            : this(list, TIsValueTypeOrStringOrStructuralEquatable ? ListEqualityComparer<T>.Default : ListEqualityComparer<T>.Aggressive, StringFormatter.CurrentCulture)
        {
        }

        internal ReadOnlyList(IList<T> list, ListEqualityComparer<T> structuralEqualityComparer, IFormatProvider toStringFormatProvider)
            : base(list)
        {
            this.structuralEqualityComparer = structuralEqualityComparer ?? throw new ArgumentNullException(nameof(structuralEqualityComparer));
            this.toStringFormatProvider = toStringFormatProvider ?? throw new ArgumentNullException(nameof(toStringFormatProvider));
        }

        internal IList<T> List => base.Items; // for testing

#region Structural Equality

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current list
        /// using rules provided by the specified <paramref name="comparer"/>.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to determine
        /// whether the current object and <paramref name="other"/> are structurally equal.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is structurally equal to the current list;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
        public virtual bool Equals(object? other, IEqualityComparer comparer)
            => ListEqualityComparer<T>.Equals(Items, other, comparer);

        /// <summary>
        /// Gets the hash code representing the current list using rules specified by the
        /// provided <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to generate
        /// the hash code.</param>
        /// <returns>A hash code representing the current list.</returns>
        public virtual int GetHashCode(IEqualityComparer comparer)
            => ListEqualityComparer<T>.GetHashCode(this, comparer);

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current list
        /// using rules similar to those in the JDK's AbstactList class. Two lists are considered
        /// equal when they both contain the same objects in the same order.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object implements <see cref="IList{T}"/>
        /// and it contains the same elements in the same order; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Equals(object, IEqualityComparer)"/>
        public override bool Equals(object? obj)
            => Equals(obj, structuralEqualityComparer);

        /// <summary>
        /// Gets the hash code for the current list. The hash code is calculated 
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => GetHashCode(structuralEqualityComparer);

#endregion

#region ToString

        /// <summary>
        /// Returns a string that represents the current list using the specified
        /// <paramref name="format"/> and <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current list.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is not zero.
        /// </exception>
        public virtual string ToString(string? format, IFormatProvider? formatProvider)
            => CollectionUtil.ToString(formatProvider, format, Items);

        /// <summary>
        /// Returns a string that represents the current list using
        /// <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current list.</returns>
        public override string ToString()
            => ToString("{0}", toStringFormatProvider);


        /// <summary>
        /// Returns a string that represents the current list using the specified
        /// <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current list.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="formatProvider"/> is <c>null</c>.</exception>
        public virtual string ToString(IFormatProvider formatProvider)
            => ToString("{0}", formatProvider);

        /// <summary>
        /// Returns a string that represents the current list using the specified
        /// <paramref name="format"/> and <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current list.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="format"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException">
        /// <paramref name="format"/> is invalid.
        /// <para/>
        /// -or-
        /// <para/>
        /// The index of a format item is not zero.
        /// </exception>
        public virtual string ToString(string format)
            => ToString(format, toStringFormatProvider);

#endregion
    }
}
