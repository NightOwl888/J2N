using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace J2N.Runtime.CompilerServices
{
    /// <summary>
    /// An equality comparer that compares objects for reference equality.
    /// <para/>
    /// The comparison is made by calling <see cref="object.ReferenceEquals(object, object)"/>
    /// rather than by calling the <see cref="Object.Equals(object)"/> method.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare. Must be a reference type.</typeparam>
    public sealed class IdentityEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
        where T : class
    {
        /// <summary>
        /// Gets the default instance of the
        /// <see cref="IdentityEqualityComparer{T}"/> class.
        /// </summary>
        /// <value>A <see cref="IdentityEqualityComparer{T}"/> instance.</value>
        public static IdentityEqualityComparer<T> Default { get; } = new IdentityEqualityComparer<T>();

        private IdentityEqualityComparer() { } // Singleton instance only

        /// <inheritdoc />
        public bool Equals(T? left, T? right)
        {
            return object.ReferenceEquals(left, right);
        }

        /// <inheritdoc />
        public int GetHashCode(T value)
        {
            return RuntimeHelpers.GetHashCode(value);
        }

        /// <inheritdoc />
        public new bool Equals(object? left, object? right)
        {
            return object.ReferenceEquals(left, right);
        }

        /// <inheritdoc />
        public int GetHashCode(object value)
        {
            return RuntimeHelpers.GetHashCode(value);
        }
    }
}
