using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
#nullable enable

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Represents a set of values.
    /// <para/>
    /// <see cref="HashSet{T}"/> adds the following features to <see cref="System.Collections.Generic.HashSet{T}"/>:
    /// <list type="bullet">
    ///     <item><description>
    ///         Overrides the <see cref="Equals(object)"/> and <see cref="GetHashCode()"/> methods to compare collections
    ///         using structural equality by default. Also, <see cref="IStructuralEquatable"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    ///     <item><description>
    ///         Overrides the <see cref="ToString()"/> method to list the contents of the set
    ///         by default. Also, <see cref="IFormatProvider"/> is implemented so the
    ///         default behavior can be overridden.
    ///     </description></item>
    ///     <item><description>
    ///         Uses <see cref="EqualityComparer{T}.Default"/> by default, which provides some specialized equality comparisons
    ///         for specific types to match the behavior of Java.
    ///     </description></item>
    /// </list>
    /// <para/>
    /// Usage Note: This class is intended to be a direct replacement for <see cref="System.Collections.Generic.HashSet{T}"/> in order
    /// to provide default structural equality and formatting behavior similar to Java. Note that the <see cref="ToString()"/>
    /// method uses the current culture by default to behave like other components in .NET. To exactly match Java's culture-neutral behavior,
    /// call <c>ToString(StringFormatter.InvariantCulture)</c>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the set.</typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    [DebuggerDisplay("Count = {Count}")]
    public class HashSet<T> : System.Collections.Generic.HashSet<T>, IStructuralEquatable, IStructuralFormattable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that is empty
        /// and uses <see cref="EqualityComparer{T}.Default"/> for the set type.
        /// </summary>
        /// <remarks>
        /// The capacity of a <see cref="HashSet{T}"/> object is the number of elements that the object can hold.
        /// A <see cref="HashSet{T}"/> object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public HashSet() : this(EqualityComparer<T>.Default) { }

#if FEATURE_HASHSET_CAPACITY
        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that is empty, but has reserved
        /// space for <paramref name="capacity"/> items and uses <see cref="EqualityComparer{T}.Default"/> for the set type.
        /// </summary>
        /// <param name="capacity">The initial size of the <see cref="HashSet{T}"/>.</param>
        /// <remarks>
        /// Since resizes are relatively expensive (require rehashing), this attempts to minimize the need
        /// to resize by setting the initial capacity based on the value of the <paramref name="capacity"/>.
        /// </remarks>
        public HashSet(int capacity) : this(capacity, EqualityComparer<T>.Default) { }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that uses <see cref="EqualityComparer{T}.Default"/>
        /// for the set type, contains elements copied from the specified collection, and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The capacity of a <see cref="HashSet{T}"/> object is the number of elements that the object can hold.
        /// A <see cref="HashSet{T}"/> object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// If <paramref name="collection"/> contains duplicates, the set will contain one of each unique element.No exception will
        /// be thrown. Therefore, the size of the resulting set is not identical to the size of <paramref name="collection"/>.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in the <paramref name="collection"/> parameter.
        /// </remarks>
        public HashSet(IEnumerable<T> collection) : this(collection, EqualityComparer<T>.Default) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that is empty and uses the
        /// specified equality comparer for the set type.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the
        /// set, or <c>null</c> to use <see cref="EqualityComparer{T}.Default"/> for the set type.</param>
        /// <remarks>
        /// The capacity of a <see cref="HashSet{T}"/> object is the number of elements that the object can hold.
        /// A <see cref="HashSet{T}"/> object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// This constructor is an O(1) operation.
        /// </remarks>
        public HashSet(IEqualityComparer<T>? comparer) : base(comparer ?? EqualityComparer<T>.Default) { }

#if FEATURE_HASHSET_CAPACITY
        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that uses the specified equality comparer
        /// for the set type, and has sufficient capacity to accommodate <paramref name="capacity"/> elements.
        /// </summary>
        /// <param name="capacity">The initial size of the <see cref="HashSet{T}"/>.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the
        /// set, or <c>null</c> to use <see cref="EqualityComparer{T}.Default"/> for the set type.</param>
        /// <remarks>Since resizes are relatively expensive (require rehashing), this attempts to minimize the need to resize
        /// by setting the initial capacity based on the value of the <paramref name="capacity"/>.</remarks>
        public HashSet(int capacity, IEqualityComparer<T>? comparer)  : base(capacity, comparer ?? EqualityComparer<T>.Default) { }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class that uses the specified equality comparer for the set type,
        /// contains elements copied from the specified collection, and has sufficient capacity to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new set.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing values in the
        /// set, or <c>null</c> to use <see cref="EqualityComparer{T}.Default"/> for the set type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is <c>null</c>.</exception>
        /// <remarks>
        /// The capacity of a <see cref="HashSet{T}"/> object is the number of elements that the object can hold. A <see cref="HashSet{T}"/>
        /// object's capacity automatically increases as elements are added to the object.
        /// <para/>
        /// If <paramref name="collection"/> contains duplicates, the set will contain one of each unique element. No exception will be thrown. Therefore,
        /// the size of the resulting set is not identical to the size of <paramref name="collection"/>.
        /// <para/>
        /// This constructor is an O(n) operation, where n is the number of elements in the <paramref name="collection"/> parameter.
        /// </remarks>
        public HashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer) : base(collection, comparer ?? EqualityComparer<T>.Default) { }


#if FEATURE_SERIALIZABLE

        /// <summary>
        /// Initializes a new instance of the <see cref="HashSet{T}"/> class with serialized data.
        /// </summary>
        /// <param name="info">A <see cref="System.Runtime.Serialization.SerializationInfo"/> object that contains
        /// the information required to serialize the <see cref="HashSet{T}"/> object.</param>
        /// <param name="context">A <see cref="System.Runtime.Serialization.StreamingContext"/> structure that contains
        /// the source and destination of the serialized stream associated with the <see cref="HashSet{T}"/> object.</param>
        protected HashSet(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif

        #endregion

        #region Structural Equality

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current set
        /// using rules provided by the specified <paramref name="comparer"/>.
        /// </summary>
        /// <param name="other">The object to compare with the current object.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to determine
        /// whether the current object and <paramref name="other"/> are structurally equal.</param>
        /// <returns><c>true</c> if <paramref name="other"/> is structurally equal to the current set;
        /// otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="comparer"/> is <c>null</c>.</exception>
        public virtual bool Equals(object? other, IEqualityComparer comparer)
            => SetEqualityComparer<T>.Equals(this, other, comparer);

        /// <summary>
        /// Gets the hash code representing the current set using rules specified by the
        /// provided <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to generate
        /// the hash code.</param>
        /// <returns>A hash code representing the current set.</returns>
        public virtual int GetHashCode(IEqualityComparer comparer)
            => SetEqualityComparer<T>.GetHashCode(this, comparer);

        /// <summary>
        /// Determines whether the specified object is structurally equal to the current set
        /// using rules similar to those in the JDK's AbstactSet class. Two sets are considered
        /// equal when they both contain the same objects (in any order).
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object implements <see cref="ISet{T}"/>
        /// and it contains the same elements; otherwise, <c>false</c>.</returns>
        /// <seealso cref="Equals(object, IEqualityComparer)"/>
        public override bool Equals(object? obj)
            => Equals(obj, SetEqualityComparer<T>.Default);

        /// <summary>
        /// Gets the hash code for the current list. The hash code is calculated 
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        /// <seealso cref="GetHashCode(IEqualityComparer)"/>
        public override int GetHashCode()
            => GetHashCode(SetEqualityComparer<T>.Default);

        #endregion

        #region ToString

        /// <summary>
        /// Returns a string that represents the current set using the specified
        /// <paramref name="format"/> and <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
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
        /// Returns a string that represents the current set using
        /// <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
        public override string ToString()
            => ToString("{0}", StringFormatter.CurrentCulture);

        /// <summary>
        /// Returns a string that represents the current set using the specified
        /// <paramref name="formatProvider"/>.
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="formatProvider"/> is <c>null</c>.</exception>
        public virtual string ToString(IFormatProvider? formatProvider)
            => ToString("{0}", formatProvider);

        /// <summary>
        /// Returns a string that represents the current set using the specified
        /// <paramref name="format"/> and <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current set.</returns>
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

        #endregion
    }
}
