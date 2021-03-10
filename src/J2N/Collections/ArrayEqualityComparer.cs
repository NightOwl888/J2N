using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
#nullable enable

namespace J2N.Collections
{
    /// <summary>
    /// Provides comparers that use structural equality rules for arrays similar to those in Java.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public abstract class ArrayEqualityComparer<T> : System.Collections.Generic.EqualityComparer<T>
    {
        /// <summary>
        /// Hidden default property that doesn't apply to this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new static IEqualityComparer<T>? Default { get; }

        /// <summary>
        /// Gets a structural equality comparer for the specified generic array type with comparison rules similar
        /// to the JDK's Arrays class.
        /// <para/>
        /// This provides a high-performance array comparison that is faster than the
        /// <see cref="System.Collections.StructuralComparisons.StructuralEqualityComparer"/>.
        /// </summary>
        public static IEqualityComparer<T[]> OneDimensional { get; } = OneDimensionalArrayEqualityComparer<T>.Default;
    }
}