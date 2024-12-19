using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
#if FEATURE_SERIALIZABLE
using System.Runtime.Serialization;
#endif

namespace J2N.Collections.Generic
{
    public partial class List<T>
    {
        /// <summary>
        /// This class represents a subset view into the list. Any changes to this view
        /// are reflected in the actual list. It uses the comparer of the underlying list.
        /// </summary>
        [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
        [DebuggerDisplay("Count = {Count}")]
        internal class SubList : List<T>
        {
            internal List<T> parent; // Internal for testing
            private readonly int parentOffset; // Tracks the diff between current offset and parent's offset (for calls to parent)
            private readonly int offset; // Keeps track of the total offset of the current SubList (calls not overridden in List<T> use this)
            private int size; // Locally keeps track of the length of this SubList (including edits)

            public SubList(List<T> list, int startIndex, int count)
            {
                parent = list;
                _items = parent._items;
                _size = parent._size;
                _version = parent._version;
                parentOffset = startIndex;
                offset = startIndex + parent.Offset;
                size =  count;
            }

            internal override int Offset => offset;

            internal override int Size => size;

            // This is the version of the original ancestor that spawned this tree of SubLists
            internal override int AncestralVersion => parent.AncestralVersion;

            internal override bool IsReadOnly => parent.IsReadOnly;

            internal override void DoSet(int index, T value)
            {
                CoModificationCheck();
                if ((uint)index >= (uint)size)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_Index);

                parent.DoSet(index + parentOffset, value);
                _version = parent._version;
            }

            internal override void DoAdd(T item)
            {
                CoModificationCheck();
                parent.DoInsert(parentOffset + size, item);
                _version = parent._version;
                _items = parent._items;
                size++;
                _size = parent._size;
            }

            internal override bool DoSetCapacity(int value)
            {
                CoModificationCheck();
                bool reallocated = parent.DoSetCapacity(value);
                if (reallocated)
                {
                    _items = parent._items; // Our items changed to a new array, we need to update the reference.
                    _version = parent._version;
                }
                return reallocated;
            }

            internal override void DoClear()
            {
                CoModificationCheck();
                parent.DoRemoveRange(parentOffset, size);
                _version = parent._version;
                size = 0;
                _size = parent._size;
            }

            internal override void DoInsert(int index, T item)
            {
                CoModificationCheck();
                // Note that insertions at the end are legal.
                if ((uint)index > (uint)size)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_ListInsert);

                parent.DoInsert(index + parentOffset, item);
                _version = parent._version;
                if (_items != parent._items)
                    _items = parent._items; // Our items changed to a new array, we need to update the reference.
                size++;
                _size = parent._size;
            }

            internal override int DoInsertRange(int index, IEnumerable<T> collection)
            {
                CoModificationCheck();
                ThrowHelper.ThrowIfNull(collection, ExceptionArgument.collection);
                // Note that insertions at the end are legal.
                if ((uint)index > (uint)size)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_ListInsert);

                int originalParentSize = parent._size;
                try
                {
                    int added = parent.DoInsertRange(index + parentOffset, collection);
                    _version = parent._version;
                    if (_items != parent._items)
                        _items = parent._items; // Our items changed to a new array, we need to update the reference.
                    size += added;
                    _size = parent._size;
                    return added;
                }
                catch
                {
                    // Rare: When appending using the enumerator, we might get an InvalidOperationException. But we still need to
                    // update our state to match the parent.
                    int added = parent._size - originalParentSize;
                    _version = parent._version;
                    if (_items != parent._items)
                        _items = parent._items; // Our items changed to a new array, we need to update the reference.
                    size += added;
                    _size = parent._size;
                    throw;
                }
            }

            internal override bool DoRemove(T item)
            {
                CoModificationCheck();
                int index = parent.IndexOf(item, parentOffset, size);
                if (index > -1)
                {
                    parent.RemoveAt(index);
                    _version = parent._version;
                    size--;
                    _size = parent._size;
                    return true;
                }
                return false;
            }

            internal override int DoRemoveAll(int startIndex, int count, Predicate<T> match)
            {
                CoModificationCheck();
                if ((uint)startIndex > (uint)size)
                    throw new ArgumentOutOfRangeException(nameof(startIndex), startIndex, SR.ArgumentOutOfRange_Index);
                if (count < 0 || startIndex > size - count)
                    throw new ArgumentOutOfRangeException(nameof(count), count, SR.ArgumentOutOfRange_Count);

                int removed = parent.DoRemoveAll(startIndex + parentOffset, count, match);
                _version = parent._version;
                size -= removed;
                _size = parent._size;
                return removed;
            }

            internal override void DoRemoveAt(int index)
            {
                CoModificationCheck();
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if ((uint)index >= (uint)size)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_ListInsert);

                parent.DoRemoveAt(index + parentOffset);
                _version = parent._version;
                size--;
                _size = parent._size;
            }

            internal override void DoRemoveRange(int index, int count)
            {
                CoModificationCheck();
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (count < 0)
                    throw new ArgumentOutOfRangeException(nameof(count), count, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (size - index < count)
                    throw new ArgumentException(SR.Argument_InvalidOffLen);

                parent.DoRemoveRange(index + parentOffset, count);
                _version = parent._version;
                size -= count;
                _size = parent._size;
            }

            internal override void DoReverse(int index, int count)
            {
                CoModificationCheck();
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (count < 0)
                    throw new ArgumentOutOfRangeException(nameof(count), count, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (size - index < count)
                    throw new ArgumentException(SR.Argument_InvalidOffLen);

                parent.DoReverse(index + parentOffset, count);
                _version = parent._version;
            }

            internal override void DoSort(int index, int count, Comparison<T> comparison)
            {
                CoModificationCheck();
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (size - index < count)
                    throw new ArgumentException(SR.Argument_InvalidOffLen);

                parent.DoSort(index + parentOffset, count, comparison);
                _version = parent._version;
            }

            internal override void DoSort(int index, int count, IComparer<T>? comparer)
            {
                CoModificationCheck();
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (count < 0)
                    throw new ArgumentOutOfRangeException(nameof(count), count, SR.ArgumentOutOfRange_NeedNonNegNum);
                if (size - index < count)
                    throw new ArgumentException(SR.Argument_InvalidOffLen);

                parent.DoSort(index + parentOffset, count, comparer);
                _version = parent._version;
            }

            internal override void CoModificationCheck()
            {
                if (AncestralVersion != _version)
                    throw new InvalidOperationException(SR.InvalidOperation_ViewFailedVersion);
            }
        }
    }
}
