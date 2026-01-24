using System.Collections;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Represents a sorted collection that may contain duplicates. Note this is just a mock and
    /// the data provided to the constructor must already be sorted according to the provided comparer.
    /// The <see cref="ISortedCollection{T}"/> interface is guaranteed only by implementation, not by
    /// interface contract.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SortedCollection<T> : ISortedCollection<T>
    {
        private readonly IComparer<T> comparer;
        private readonly List<T> list = new List<T>();

        public SortedCollection() : this(null)
        {
        }

        public SortedCollection(IComparer<T>? comparer)
        {
            this.comparer = comparer ?? Comparer<T>.Default;
        }

        public IComparer<T> Comparer => comparer;

        public int Count => list.Count;

        public bool IsReadOnly => false;

        public void Add(T item) => list.Add(item);

        public void Clear() => list.Clear();

        public bool Contains(T item) => list.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => list.GetEnumerator();

        public bool Remove(T item) => list.Remove(item);

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)list).GetEnumerator();
    }
}
