using System;
using System.Collections.Generic;
using System.Diagnostics;

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
            private readonly List<T> origin;
            internal List<T> parent; // Internal for testing
            private readonly int parentOffset; // Tracks the diff between current offset and parent's offset (for calls to parent)
            private readonly int offset; // Keeps track of the total offset of the current SubList (calls not overridden in List<T> use this)
            private int size; // Locally keeps track of the length of this SubList (including edits)

            public SubList(List<T> list, int startIndex, int count)
            {
                origin = list.Origin;
                parent = list;
                _items = origin._items;
                _size = parent._size;
                _version = parent._version;
                parentOffset = startIndex;
                offset = startIndex + parent.Offset;
                size =  count;
            }

            internal override int Offset => offset;

            internal override int Size => size;

            // This is the eldest ancestor that spawned this tree of SubLists
            internal override List<T> Origin => origin;

            internal override bool IsReadOnly => parent.IsReadOnly;

            internal override void DoSet(int index, T value)
            {
                CoModificationCheck();
                if ((uint)index >= (uint)size)
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(index);

                parent.DoSet(index + parentOffset, value);
                _version = parent._version;
            }

            internal override void DoAdd(T item)
            {
                CoModificationCheck();
                parent.DoInsert(parentOffset + size, item);
                _version = parent._version;
                if (_items != origin._items)
                    _items = origin._items; // Our items changed to a new array, we need to update the reference.
                size++;
                _size = parent._size;
            }

            internal override bool DoSetCapacity(int value)
            {
                CoModificationCheck();
                bool reallocated = parent.DoSetCapacity(value);
                if (reallocated)
                {
                    _items = origin._items; // Our items changed to a new array, we need to update the reference.
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
                    ThrowHelper.ThrowArgumentOutOfRangeException(index, ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_ListInsert);

                parent.DoInsert(index + parentOffset, item);
                _version = parent._version;
                if (_items != origin._items)
                    _items = origin._items; // Our items changed to a new array, we need to update the reference.
                size++;
                _size = parent._size;
            }

            internal override void DoAddRange(IEnumerable<T> collection)
            {
                CoModificationCheck();
                if (collection is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);

                int originalParentSize = parent._size;
                int added = 0;
                try
                {
                    // NOTE: It seems like we should be calling parent.DoAddRange here, but we need to insert at the end of the sublist,
                    // which is at parentOffset + size, not at the end of the parent.
                    added = parent.DoInsertRange(parentOffset + size, collection);
                }
                catch
                {
                    // Rare: When appending using the enumerator, we might get an InvalidOperationException. But we still need to
                    // update our state to match the parent.
                    added = parent._size - originalParentSize;
                    throw;
                }
                finally
                {
                    _version = parent._version;
                    if (_items != origin._items)
                        _items = origin._items; // Our items changed to a new array, we need to update the reference.
                    size += added;
                    _size = parent._size;
                }
            }

            internal override void DoAddRange(ReadOnlySpan<T> source)
            {
                CoModificationCheck();

                int originalParentSize = parent._size;
                int added = 0;
                try
                {
                    // NOTE: It seems like we should be calling parent.DoAddRange here, but we need to insert at the end of the sublist,
                    // which is at parentOffset + size, not at the end of the parent.
                    added = parent.DoInsertRange(parentOffset + size, source);
                }
                catch
                {
                    // Rare: When appending using the enumerator, we might get an InvalidOperationException. But we still need to
                    // update our state to match the parent.
                    added = parent._size - originalParentSize;
                    throw;
                }
                finally
                {
                    _version = parent._version;
                    if (_items != origin._items)
                        _items = origin._items; // Our items changed to a new array, we need to update the reference.
                    size += added;
                    _size = parent._size;
                }
            }

            internal override int DoInsertRange(int index, IEnumerable<T> collection)
            {
                CoModificationCheck();
                if (collection is null)
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
                // Note that insertions at the end are legal.
                if ((uint)index > (uint)size)
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);

                int originalParentSize = parent._size;
                int added = 0;
                try
                {
                    added = parent.DoInsertRange(index + parentOffset, collection);
                }
                catch
                {
                    // Rare: When appending using the enumerator, we might get an InvalidOperationException. But we still need to
                    // update our state to match the parent.
                    added = parent._size - originalParentSize;
                    throw;
                }
                finally
                {
                    _version = parent._version;
                    if (_items != origin._items)
                        _items = origin._items; // Our items changed to a new array, we need to update the reference.
                    size += added;
                    _size = parent._size;
                }
                return added;
            }

            internal override int DoInsertRange(int index, ReadOnlySpan<T> source)
            {
                CoModificationCheck();
                // Note that insertions at the end are legal.
                if ((uint)index > (uint)size)
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(index);

                int originalParentSize = parent._size;
                int added = 0;
                try
                {
                    added = parent.DoInsertRange(index + parentOffset, source);
                }
                catch
                {
                    // Rare: When appending using the enumerator, we might get an InvalidOperationException. But we still need to
                    // update our state to match the parent.
                    added = parent._size - originalParentSize;
                    throw;
                }
                finally
                {
                    _version = parent._version;
                    if (_items != origin._items)
                        _items = origin._items; // Our items changed to a new array, we need to update the reference.
                    size += added;
                    _size = parent._size;
                }
                return added;
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
                    ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_IndexMustBeLess(startIndex);
                if (count < 0 || startIndex > size - count)
                    ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count(count);

                int removed = parent.DoRemoveAll(startIndex + parentOffset, count, match);
                _version = parent._version;
                size -= removed;
                _size = parent._size;
                return removed;
            }

            internal override void DoRemoveAt(int index)
            {
                CoModificationCheck();
                if ((uint)index >= (uint)size)
                    ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(index);

                parent.DoRemoveAt(index + parentOffset);
                _version = parent._version;
                size--;
                _size = parent._size;
            }

            internal override void DoRemoveRange(int index, int count)
            {
                CoModificationCheck();
                if (index < 0)
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
                if (count < 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                if (size - index < count)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

                parent.DoRemoveRange(index + parentOffset, count);
                _version = parent._version;
                size -= count;
                _size = parent._size;
            }

            internal override void DoReverse(int index, int count)
            {
                CoModificationCheck();
                if (index < 0)
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
                if (count < 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                if (size - index < count)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

                parent.DoReverse(index + parentOffset, count);
                _version = parent._version;
            }

            internal override void DoSort(int index, int count, Comparison<T> comparison)
            {
                CoModificationCheck();
                if (index < 0)
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
                if (count < 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                if (size - index < count)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

                parent.DoSort(index + parentOffset, count, comparison);
                _version = parent._version;
            }

            internal override void DoSort(int index, int count, IComparer<T>? comparer)
            {
                CoModificationCheck();
                if (index < 0)
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException(index);
                if (count < 0)
                    ThrowHelper.ThrowArgumentOutOfRangeException(count, ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
                if (size - index < count)
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);

                parent.DoSort(index + parentOffset, count, comparer);
                _version = parent._version;
            }

            internal override void CoModificationCheck()
            {
                if (_items != origin._items)
                    _items = origin._items; // Our items changed to a new array, we need to update the reference.

                if (origin._version != _version)
                {
                    ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_ViewFailedVersion);
                }
            }
        }
    }
}
