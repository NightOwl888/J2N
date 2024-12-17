using J2N.Collections.Generic;
using J2N.Collections.Generic.Extensions;
using J2N.Collections.ObjectModel;
using J2N.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using JCG = J2N.Collections.Generic;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Wraps an <see cref="IList{T}"/> as a sublist. However, since we can make no assumptions as to the internals of the
    /// passed in implementation, we make a best effort to throw an exception when the Count property of the list has changed
    /// but have undefined behavior it the state is mutated in another way.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    internal class SubList<T> : IList<T>, IStructuralEquatable, IStructuralFormattable
    {
        internal IList<T> parent; // Internal for testing
        private readonly int parentOffset; // Tracks the diff between current offset and parent's offset (for calls to parent)
        private int size; // Locally keeps track of the length of this SubList (including edits)
        private int version; // Tracks changes to the current SubList (for enumerators)
        private int parentCount; // Best effort to track changes to parent (blow up if the Count changes, but it is expensive to determine if an unknown IList<T> has mutated beyond that)

        public SubList(IList<T> parent, int index, int count)
        {
            this.parent = parent;
            version = 0;
            parentOffset = index;
            size = count;
            parentCount = parent.Count;
        }

        #region IList<T> Members

        public T this[int index]
        {
            get
            {
                CoModificationCheck();
                if ((uint)index >= (uint)size)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_Index);

                return parent[index + parentOffset];
            }
            set
            {
                CoModificationCheck();
                if ((uint)index >= (uint)size)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_Index);

                parent[index + parentOffset] = value;
            }
        }

        public int Count => size;

        public bool IsReadOnly => parent.IsReadOnly;

        public void Add(T item)
        {
            CoModificationCheck();
            version++;
            parent.Insert(parentOffset + size, item);
            size++;
            parentCount = parent.Count;
        }

        public void Clear()
        {
            CoModificationCheck();
            version++;
            parent.RemoveAll(parentOffset, size, (value) => true); // J2N TODO: Using RemoveRange(startIndex, count) would be more efficient
            size = 0;
            parentCount = parent.Count;
        }

        public bool Contains(T item)
        {
            CoModificationCheck();
            return size != 0 && IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int index)
        {
            CoModificationCheck();
            if (array is null)
                throw new ArgumentNullException(nameof(array));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
            if (array.Length - index < size)
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

            if (size == 0)
                return;

            int limit = parentOffset + size;
            for (int dest = index, source = parentOffset; source < limit; dest++, source++)
                array[dest] = parent[source];
        }

        public Enumerator GetEnumerator()
        {
            CoModificationCheck();
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
            => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public int IndexOf(T item)
        {
            CoModificationCheck();

            // Unwrap ReadOnlyList to expose its inner type
            IList<T> localList = parent is ReadOnlyList<T> readOnlyList ? readOnlyList.List : parent;

            // Fast path: call into IndexOf(T, int, int) overload if it is available.
            // We can't use IndexOf(T) because there may be duplicate items outside of the
            // range we are searching.
            if (localList is SCG.List<T> scgList)
            {
                int result = scgList.IndexOf(item, parentOffset, size) - parentOffset;
                return result >= 0 ? result : -1;
            }
            else if (localList is JCG.List<T> jcgList)
            {
                int result = jcgList.IndexOf(item, parentOffset, size) - parentOffset;
                return result >= 0 ? result : -1;
            }

            return IndexOfSlow(item, parentOffset, size);
        }

        private int IndexOfSlow(T item, int startIndex, int count)
        {
            int endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (EqualityComparer<T>.Default.Equals(parent[i], item)) return i - parentOffset;
            }
            return -1;
        }

        public void Insert(int index, T item)
        {
            CoModificationCheck();
            // Note that insertions at the end are legal.
            if ((uint)index > (uint)size)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_ListInsert);

            parent.Insert(index + parentOffset, item);
            size++;
            version++;
            parentCount = parent.Count;
        }

        public bool Remove(T item)
        {
            // Delegate error checking to IndexOf
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            CoModificationCheck();
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
            if ((uint)index >= (uint)size)
                throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_Index);

            size--;
            parent.RemoveAt(index + parentOffset);
            version++;
            parentCount = parent.Count;
        }

        private void CoModificationCheck()
        {
            if (parent.Count != parentCount)
                throw new InvalidOperationException(SR.InvalidOperation_ViewFailedVersion);
        }

        #endregion  IList<T> Members

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
        {
            CoModificationCheck();
            return ListEqualityComparer<T>.Equals(this, other, comparer);
        }

        /// <summary>
        /// Gets the hash code representing the current list using rules specified by the
        /// provided <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IEqualityComparer"/> implementation to use to generate
        /// the hash code.</param>
        /// <returns>A hash code representing the current list.</returns>
        public virtual int GetHashCode(IEqualityComparer comparer)
        {
            CoModificationCheck();
            return ListEqualityComparer<T>.GetHashCode(this, comparer);
        }

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
            => Equals(obj, ListEqualityComparer<T>.Default);

        /// <summary>
        /// Gets the hash code for the current list. The hash code is calculated 
        /// by taking each nested element's hash code into account.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
            => GetHashCode(ListEqualityComparer<T>.Default);

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
        {
            CoModificationCheck();
            return CollectionUtil.ToString(formatProvider, format, this);
        }

        /// <summary>
        /// Returns a string that represents the current list using
        /// <see cref="StringFormatter.CurrentCulture"/>.
        /// <para/>
        /// The presentation has a specific format. It is enclosed by square
        /// brackets ("[]"). Elements are separated by ', ' (comma and space).
        /// </summary>
        /// <returns>A string that represents the current list.</returns>
        public override string ToString()
            => ToString("{0}", StringFormatter.CurrentCulture);


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
            => ToString(format, StringFormatter.CurrentCulture);

        #endregion

        #region Nested Structure: Enumerator

        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly SubList<T> list;
            private int index;
            private readonly int version;
            [AllowNull, MaybeNull] private T current;

            internal Enumerator(SubList<T> list)
            {
                this.list = list;
                index = list.parentOffset;
                version = list.version;
                current = default!;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                SubList<T> localList = list;
                localList.CoModificationCheck();
                if (version == localList.version && ((uint)index < ((uint)localList.size + (uint)localList.parentOffset)))
                {
                    current = localList.parent[index];
                    index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                SubList<T> localList = list;
                if (version != localList.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                index = localList.size + localList.parentOffset + 1;
                current = default!;
                return false;
            }

            public T Current => current;

            object? IEnumerator.Current
            {
                get
                {
                    SubList<T> localList = list;
                    int offset = localList.parentOffset;
                    if (index == offset || index == localList.size + offset + 1)
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                list.CoModificationCheck();
                if (version != list.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                index = list.parentOffset;
                current = default!;
            }
        }

        #endregion
    }
}
