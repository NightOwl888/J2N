using System.Collections.Generic;
using System.Diagnostics;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Implements reverse comparison behavior for any comparer as well as
    /// comparer equality checking.
    /// </summary>
    /// <typeparam name="T">The type of element to compare.</typeparam>
    /// <remarks>
    /// This is used by the <see cref="SortedSet{T}"/> and <see cref="SortedDictionary{TKey, TValue}"/>
    /// to implement descending view behavior.
    /// </remarks>
    internal sealed class ReverseComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> _inner;

        private ReverseComparer(IComparer<T> inner)
        {
            Debug.Assert(inner != null);
            _inner = inner!; // [!]: asserted above
        }

        public IComparer<T> InnerComparer => _inner;

        public static ReverseComparer<T> Create(IComparer<T> inner)
            => new(inner);

        public int Compare(T? x, T? y)
            => _inner.Compare(y!, x!);

        public override bool Equals(object? obj)
        {
            if (obj is ReverseComparer<T> reverse)
                return _inner.Equals(reverse._inner);

            return false;
        }

        public override int GetHashCode()
        {
            return _inner.GetHashCode();
        }
    }
}
