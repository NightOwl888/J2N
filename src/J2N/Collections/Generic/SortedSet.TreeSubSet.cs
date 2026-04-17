// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Dependency of SortedSet, SortedDictionary

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

#if FEATURE_SERIALIZABLE
using System.Runtime.Serialization;
#endif


namespace J2N.Collections.Generic
{
    public partial class SortedSet<T>
    {
        /// <summary>
        /// This class represents a subset view into the tree. Any changes to this view
        /// are reflected in the actual tree. It uses the comparer of the underlying tree.
        /// </summary>
        [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
        [DebuggerDisplay("Count = {Count}")]
        internal sealed class TreeSubSet : SortedSet<T>, ICollectionView
#if FEATURE_SERIALIZABLE
            , ISerializable, IDeserializationCallback
#endif
        {
            private readonly SortedSet<T> _underlying;
            private readonly T? _min;
            private readonly T? _max;
            // keeps track of whether the count variable is up to date
            // up to date -> _countVersion = _underlying.version
            // not up to date -> _countVersion < _underlying.version
            private int _countVersion;
            // these exist for unbounded collections
            // for instance, you could allow this subset to be defined for i > 10. The set will throw if
            // anything <= 10 is added, but there is no upper bound. These features Head(), Tail(), were punted
            // in the spec, and are not available, but the framework is there to make them available at some point.
            private readonly bool _lBoundActive, _uBoundActive;
            private readonly bool _lBoundInclusive, _uBoundInclusive;

            private readonly bool _reverse;
            private IComparer<T>? _reverseComparer;

            internal override IComparer<T> ComparerInternal
            {
                get
                {
                    if (_reverse)
                    {
                        return _reverseComparer ??= ReverseComparer<T>.Create(_underlying.ComparerInternal);
                    }
                    return _underlying.ComparerInternal;
                }
            }

            #region ICollectionView Members

            bool ICollectionView.IsView => true;

            #endregion

            #region Properties for Alternate Lookup

            // J2N: This is state from TreeSubSet exposed to allow range checks in Alternate Lookup

            internal override bool IsReversed => _reverse;

            internal override SortedSet<T> UnderlyingSet => _underlying;

            internal override bool HasLowerBound => _lBoundActive;
            internal override bool HasUpperBound => _uBoundActive;

            internal override bool LowerBoundInclusive => _lBoundInclusive;
            internal override bool UpperBoundInclusive => _uBoundInclusive;

            internal override T? LowerBound => _min;
            internal override T? UpperBound => _max;

            #endregion

            #region Subclass helpers

            /// <inheritdoc/>
            internal override T? LowerValue
            {
                get
                {
                    Debug.Assert(_underlying != null);
                    if (version != _underlying!.version) // [!] asserted above
                        VersionCheck();

                    // J2N: Added caching to the value so we don't have to traverse the tree again unless the set is mutated.
                    if (minVersion == version)
                        return cachedMin;

                    Node? current = root;
                    T? result = default;

                    while (current != null)
                    {
                        int comp = _lBoundActive ? comparer.Compare(_min!, current.Item!) : -1;
                        if (comp > 0 || (comp == 0 && !_lBoundInclusive))
                        {
                            current = current.Right;
                        }
                        else
                        {
                            result = current.Item;
                            if (comp == 0)
                            {
                                if (!_lBoundInclusive)
                                {
                                    current = current.Left;
                                    result = current != null ? current.Item : default;
                                }
                                break;
                            }
                            current = current.Left;
                        }
                    }

                    minVersion = version;
                    cachedMin = result;
                    return result;
                }
            }

            /// <inheritdoc/>
            internal override T? UpperValue
            {
                get
                {
                    Debug.Assert(_underlying != null);
                    if (version != _underlying!.version) // [!] asserted above
                        VersionCheck();

                    // J2N: Added caching to the value so we don't have to traverse the tree again unless the set is mutated.
                    if (maxVersion == version)
                        return cachedMax;

                    Node? current = root;
                    T? result = default;

                    while (current != null)
                    {
                        int comp = _uBoundActive ? comparer.Compare(_max!, current.Item!) : 1;
                        if (comp < 0 || (comp == 0 && !_uBoundInclusive))
                        {
                            current = current.Left;
                        }
                        else
                        {
                            result = current.Item;
                            if (comp == 0)
                            {
                                if (!_uBoundInclusive)
                                {
                                    current = current.Right;
                                    result = current != null ? current.Item : default;
                                }
                                break;
                            }
                            current = current.Right;
                        }
                    }

                    maxVersion = version;
                    cachedMax = result;
                    return result;
                }
            }

            internal override void EnsureTreeOrder(T[] array, int length)
            {
                if (_reverse && length > 1)
                {
                    Array.Reverse(array, 0, length);
                }
            }

            #endregion


            // used to see if the count is out of date
#if DEBUG
            internal override bool versionUpToDate()
            {
                return (version == _underlying.version);
            }
#endif
            public TreeSubSet(SortedSet<T> Underlying, [AllowNull] T Min, bool lowerBoundInclusive, [AllowNull] T Max, bool upperBoundInclusive, bool lowerBoundActive, bool upperBoundActive, bool reverse)
                : base(Underlying.Comparer)
            {
                _underlying = Underlying;
                _min = Min;
                _max = Max;
                _lBoundInclusive = lowerBoundInclusive;
                _uBoundInclusive = upperBoundInclusive;
                _lBoundActive = lowerBoundActive;
                _uBoundActive = upperBoundActive;
                _reverse = reverse;
                root = _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive); // root is first element within range
                count = 0;
                version = -1;
                _countVersion = -1;
            }

            internal override bool AddIfNotPresent(T item)
            {
                if (!IsWithinRange(item))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.item);
                }

                bool ret = _underlying.AddIfNotPresent(item);
                VersionCheck();
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif

                return ret;
            }

            public override bool Contains(T item)
            {
                VersionCheck();
#if DEBUG
                Debug.Assert(versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif
                return base.Contains(item);
            }

            internal override bool DoRemove(T item, [MaybeNullWhen(false)] out T removed)
            {
                if (!IsWithinRange(item))
                {
                    removed = default;
                    return false;
                }

                bool ret = _underlying.DoRemove(item, out removed);
                VersionCheck();
#if DEBUG
                Debug.Assert(versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif
                return ret;
            }

            public override void Clear()
            {
                if (Count == 0)
                {
                    return;
                }

                List<T> toRemove = new List<T>();
                BreadthFirstTreeWalk(n => { toRemove.Add(n.Item); return true; });
                while (toRemove.Count != 0)
                {
                    _underlying.Remove(toRemove[toRemove.Count - 1]);
                    toRemove.RemoveAt(toRemove.Count - 1);
                }

                root = null;
                count = 0;
                version = _underlying.version;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal override bool IsWithinRange([AllowNull] T item)
            {
                // Check whether too low
                if (_lBoundActive)
                {
                    int c = comparer.Compare(item!, _min!);
                    if (c < 0 || (c == 0 && !_lBoundInclusive))
                        return false;
                }

                // Check whether too high
                if (_uBoundActive)
                {
                    int c = comparer.Compare(item!, _max!);
                    if (c > 0 || (c == 0 && !_uBoundInclusive))
                        return false;
                }

                return true;
            }

            private bool IsWithinRange([AllowNull] T item, bool inclusive)
                => inclusive ? IsWithinRange(item) : IsWithinClosedRange(item);


            private bool IsWithinClosedRange([AllowNull] T item)
            {
                if (_lBoundActive)
                {
                    if (comparer.Compare(item!, _min!) < 0)
                        return false;
                }

                if (_uBoundActive)
                {
                    if (comparer.Compare(item!, _max!) > 0)
                        return false;
                }

                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal override bool IsTooHigh([AllowNull] T item)
            {
                if (_uBoundActive)
                {
                    int c = comparer.Compare(item!, _max!);
                    if (c > 0 || (c == 0 && !_uBoundInclusive))
                        return true;
                }
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal override bool IsTooLow([AllowNull] T item)
            {
                if (_lBoundActive)
                {
                    int c = comparer.Compare(item!, _min!);
                    if (c < 0 || (c == 0 && !_lBoundInclusive))
                        return true;
                }
                return false;
            }

            /// <inheritdoc/>
            internal override T? MinInternal => _reverse ? UpperValue : LowerValue;

            /// <inheritdoc/>
            internal override T? MaxInternal => _reverse ? LowerValue : UpperValue;

            internal override bool DoTryGetFirst([MaybeNullWhen(false)] out T result)
            {
                VersionCheck(updateCount: true);

                return base.DoTryGetFirst(out result);
            }

            internal override bool DoTryGetLast([MaybeNullWhen(false)] out T result)
            {
                VersionCheck(updateCount: true);

                return base.DoTryGetLast(out result);
            }

            internal override bool DoRemoveFirst([MaybeNullWhen(false)] out T value)
            {
                VersionCheck(updateCount: true);

                return base.DoRemoveFirst(out value);
            }

            internal override bool DoRemoveLast([MaybeNullWhen(false)] out T value)
            {
                VersionCheck(updateCount: true);

                return base.DoRemoveLast(out value);
            }

            internal override bool InOrderTreeWalk(TreeWalkPredicate<T> action)
            {
                VersionCheck();

                if (root == null)
                {
                    return true;
                }

                // The maximum height of a red-black tree is 2*lg(n+1).
                // See page 264 of "Introduction to algorithms" by Thomas H. Cormen
                Stack<Node> stack = new Stack<Node>(2 * (int)SortedSet<T>.Log2(count + 1)); // this is not exactly right if count is out of date, but the stack can grow
                Node? current = root;
                while (current != null)
                {
                    if (IsWithinRange(current.Item))
                    {
                        stack.Push(current);
                        current = current.Left;
                    }
                    else if (IsTooLow(current.Item))
                    {
                        current = current.Right;
                    }
                    else
                    {
                        current = current.Left;
                    }
                }

                while (stack.Count != 0)
                {
                    current = stack.Pop();
                    if (!action(current))
                    {
                        return false;
                    }

                    Node? node = current.Right;
                    while (node != null)
                    {
                        if (IsWithinRange(node.Item))
                        {
                            stack.Push(node);
                            node = node.Left;
                        }
                        else if (IsTooLow(node.Item))
                        {
                            node = node.Right;
                        }
                        else
                        {
                            node = node.Left;
                        }
                    }
                }
                return true;
            }

            internal override bool BreadthFirstTreeWalk(TreeWalkPredicate<T> action)
            {
                VersionCheck();

                if (root == null)
                {
                    return true;
                }

                Queue<Node> processQueue = new Queue<Node>();
                processQueue.Enqueue(root);
                Node current;

                while (processQueue.Count != 0)
                {
                    current = processQueue.Dequeue();
                    if (IsWithinRange(current.Item) && !action(current))
                    {
                        return false;
                    }
                    if (current.Left != null && (!_lBoundActive || comparer.Compare(_min!, current.Item!) < 0))
                    {
                        processQueue.Enqueue(current.Left);
                    }
                    if (current.Right != null && (!_uBoundActive || comparer.Compare(_max!, current.Item!) > 0))
                    {
                        processQueue.Enqueue(current.Right);
                    }
                }
                return true;
            }

            internal override SortedSet<T>.Node? FindNode(T item)
            {
                if (!IsWithinRange(item))
                {
                    return null;
                }

                VersionCheck();
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif
                return base.FindNode(item);
            }

            // this does indexing in an inefficient way compared to the actual sortedset, but it saves a
            // lot of space
            internal override int InternalIndexOf(T item)
            {
                int count = -1;
                foreach (T i in this)
                {
                    count++;
                    if (comparer.Compare(item, i) == 0)
                        return count;
                }
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif
                return -1;
            }

            // J2N: We need to override for views to ensure the underlying set version is updated
            internal override void UpdateVersion()
            {
                Debug.Assert(_underlying != null);
                _underlying!.UpdateVersion(); // [!] asserted above
                base.UpdateVersion();
            }

            /// <summary>
            /// Checks whether this subset is out of date, and updates it if necessary.
            /// <param name="updateCount">Updates the count variable if necessary.</param>
            /// </summary>
            internal override void VersionCheck(bool updateCount = false) => VersionCheckImpl(updateCount);

            private void VersionCheckImpl(bool updateCount)
            {
                Debug.Assert(_underlying != null);
                if (version != _underlying!.version) // [!] asserted above
                {
                    root = _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive);
                    version = _underlying.version;
                }

                if (updateCount && _countVersion != _underlying.version)
                {
                    count = 0;
                    InOrderTreeWalk(n => { count++; return true; });
                    _countVersion = _underlying.version;
                }
            }

            /// <summary>
            /// Returns the number of elements <c>count</c> of the parent set.
            /// </summary>
            internal override int TotalCount()
            {
                Debug.Assert(_underlying != null);
                return _underlying!.Count; // [!] asserted above
            }

            // This passes functionality down to the underlying tree, clipping edges and reversing
            // argument order if necessary. There's nothing gained by having a nested subset. May
            // as well draw it from the base. Cannot increase the bounds of the subset, can only decrease it.
            internal override SortedSet<T> DoGetView([AllowNull] T fromItem, bool fromInclusive, ExceptionArgument fromArgumentName, [AllowNull] T toItem, bool toInclusive, ExceptionArgument toArgumentName)
            {
                T? lower = _reverse ? toItem : fromItem;
                T? upper = _reverse ? fromItem : toItem;
                bool lowerInclusive = _reverse ? toInclusive : fromInclusive;
                bool upperInclusive = _reverse ? fromInclusive : toInclusive;
                ExceptionArgument lowerArgumentName = _reverse ? toArgumentName : fromArgumentName;
                ExceptionArgument upperArgumentName = _reverse ? fromArgumentName : toArgumentName;

                if (!IsWithinRange(lower, lowerInclusive))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(lowerArgumentName);
                }
                if (!IsWithinRange(upper, upperInclusive))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(upperArgumentName);
                }

                return base.DoGetView(lower, lowerInclusive, lowerArgumentName, upper, upperInclusive, upperArgumentName);
            }

            // This passes functionality down to the underlying tree, clipping edges if necessary
            // There's nothing gained by having a nested subset. May as well draw it from the base
            // Cannot increase the bounds of the subset, can only decrease it
            internal override SortedSet<T> DoGetViewBefore([AllowNull] T toItem, bool inclusive, ExceptionArgument toArgumentName)
            {
                if (!IsWithinRange(toItem, inclusive))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(toArgumentName);
                }

                return !_reverse
                    ? GetViewBeforeCore(toItem, inclusive)
                    : GetViewAfterCore(toItem, inclusive);
            }

            // This passes functionality down to the underlying tree, clipping edges if necessary
            // There's nothing gained by having a nested subset. May as well draw it from the base
            // Cannot increase the bounds of the subset, can only decrease it
            internal override SortedSet<T> DoGetViewAfter([AllowNull] T fromItem, bool inclusive, ExceptionArgument fromArgumentName)
            {
                if (!IsWithinRange(fromItem, inclusive))
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(fromArgumentName);
                }

                return !_reverse
                    ? GetViewAfterCore(fromItem, inclusive)
                    : GetViewBeforeCore(fromItem, inclusive);
            }

            private SortedSet<T> GetViewBeforeCore([AllowNull] T toItem, bool inclusive)
            {
                T? upper;
                bool upperInclusive;

                // Fast path - no upper bound, no equality possible
                if (!_uBoundActive)
                {
                    upper = toItem;
                    upperInclusive = inclusive;
                }
                else
                {
                    // Compute comparison ONCE
                    int cmp = comparer.Compare(toItem!, _max!);
                    if (cmp < 0)
                    {
                        // Override with new value
                        upper = toItem;
                        upperInclusive = inclusive;
                    }
                    else if (cmp > 0)
                    {
                        // Clipped by upper bound
                        upper = _max;
                        upperInclusive = _uBoundInclusive;
                    }
                    else // cmp == 0
                    {
                        // Rare equality case
                        upper = _max;
                        upperInclusive = _uBoundInclusive && inclusive;
                    }
                }

                return new TreeSubSet(_underlying, _min, _lBoundInclusive, upper, upperInclusive, _lBoundActive, true, _reverse);
            }

            private SortedSet<T> GetViewAfterCore([AllowNull] T fromItem, bool inclusive)
            {
                T? lower;
                bool lowerInclusive;

                // Fast path - no lower bound, no equality possible
                if (!_lBoundActive)
                {
                    lower = fromItem;
                    lowerInclusive = inclusive;
                }
                else
                {
                    // Compute comparison ONCE
                    int cmp = comparer.Compare(fromItem!, _min!);
                    if (cmp > 0)
                    {
                        // Override with new value
                        lower = fromItem;
                        lowerInclusive = inclusive;
                    }
                    else if (cmp < 0)
                    {
                        // Clipped by lower bound
                        lower = _min;
                        lowerInclusive = _lBoundInclusive;
                    }
                    else // cmp == 0
                    {
                        // Rare equality case
                        lower = _min;
                        lowerInclusive = _lBoundInclusive && inclusive;
                    }
                }

                return new TreeSubSet(_underlying, lower, lowerInclusive, _max, _uBoundInclusive, true, _uBoundActive, _reverse);
            }

#if DEBUG
            internal override void IntersectWithEnumerable(IEnumerable<T> other)
            {
                base.IntersectWithEnumerable(other);
                Debug.Assert(versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
            }
#endif

            internal override void SymmetricExceptWithValue(T item)
            {
                // J2N: We perform the range check here to bypass the range checks in
                // Add/Remove/FindNode. This relies on TreeSubSet invariants and must
                // stay up to date with the underlying tree semantics.

                // We only check the range once
                if (!IsWithinRange(item))
                {
                    return;
                }

                /////////////////////////////////////
                // Contains (FindNode) (without range check)
                /////////////////////////////////////

                VersionCheck();
#if DEBUG
                Debug.Assert(versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif
                if (base.FindNode(item) != null)
                {
                    /////////////////////////////////////
                    // Remove (without range check)
                    /////////////////////////////////////
                    _underlying.DoRemove(item, out _);
                    VersionCheck();
#if DEBUG
                    Debug.Assert(versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif
                }
                else
                {
                    /////////////////////////////////////
                    // Add (without range check)
                    /////////////////////////////////////
                    _underlying.AddIfNotPresent(item);
                    VersionCheck();
#if DEBUG
                    Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif
                }
            }

            internal override bool DoTryGetPredecessor([AllowNull] T item, [MaybeNullWhen(false)] out T result)
                => _reverse ? TryGetSuccessorCore(item, out result) : TryGetPredecessorCore(item, out result);

            internal bool TryGetPredecessorCore([AllowNull] T item, [MaybeNullWhen(false)] out T result)
            {
                VersionCheck();
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif

                // If item is at or below lower bound, no strict predecessor exists
                if (IsTooLow(item!))
                {
                    result = default!;
                    return false;
                }

                Node? current = root;
                Node? match = null;

                while (current != null)
                {
                    int cmp = comparer.Compare(item!, current.Item);

                    if (cmp > 0)
                    {
                        match = current;
                        current = current.Right;
                    }
                    else
                    {
                        current = current.Left;
                    }
                }

                // Final safety check: candidate must be within view
                if (match == null || IsTooLow(match.Item))
                {
                    result = default!;
                    return false;
                }

                result = match.Item;
                return true;
            }

            internal override bool DoTryGetSuccessor([AllowNull] T item, [MaybeNullWhen(false)] out T result)
                => _reverse ? TryGetPredecessorCore(item, out result) : TryGetSuccessorCore(item, out result);

            internal bool TryGetSuccessorCore([AllowNull] T item, [MaybeNullWhen(false)] out T result)
            {
                VersionCheck();
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif

                // If item is at or above upper bound, no strict successor exists
                if (IsTooHigh(item!))
                {
                    result = default!;
                    return false;
                }

                Node? current = root;
                Node? match = null;

                while (current != null)
                {
                    int cmp = comparer.Compare(item!, current.Item);

                    if (cmp < 0)
                    {
                        match = current;
                        current = current.Left;
                    }
                    else
                    {
                        current = current.Right;
                    }
                }

                // Final safety check
                if (match == null || IsTooHigh(match.Item))
                {
                    result = default!;
                    return false;
                }

                result = match.Item;
                return true;
            }

            internal override bool DoTryGetFloor([AllowNull] T item, [MaybeNullWhen(false)] out T result)
                => _reverse ? TryGetCeilingCore(item, out result) : TryGetFloorCore(item, out result);

            internal bool TryGetFloorCore([AllowNull] T item, [MaybeNullWhen(false)] out T result)
            {
                VersionCheck();
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif

                Node? current = root;
                Node? candidate = null;

                while (current != null)
                {
                    int cmp = comparer.Compare(item!, current.Item);

                    if (cmp < 0)
                    {
                        current = current.Left;
                    }
                    else
                    {
                        candidate = current;
                        current = current.Right;
                    }
                }

                if (candidate == null || IsTooLow(candidate.Item))
                {
                    result = default!;
                    return false;
                }

                result = candidate.Item;
                return true;
            }

            internal override bool DoTryGetCeiling([AllowNull] T item, [MaybeNullWhen(false)] out T result)
                => _reverse ? TryGetFloorCore(item, out result) : TryGetCeilingCore(item, out result);

            internal bool TryGetCeilingCore([AllowNull] T item, [MaybeNullWhen(false)] out T result)
            {
                VersionCheck();
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif

                Node? current = root;
                Node? candidate = null;

                while (current != null)
                {
                    int cmp = comparer.Compare(item!, current.Item);

                    if (cmp > 0)
                    {
                        current = current.Right;
                    }
                    else
                    {
                        candidate = current;
                        current = current.Left;
                    }
                }

                if (candidate == null || IsTooHigh(candidate.Item))
                {
                    result = default!;
                    return false;
                }

                result = candidate.Item;
                return true;
            }

#if FEATURE_SERIALIZABLE

            [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context) => GetObjectData(info, context);

            [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            protected override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_SerializationDeprecated);
            }

            void IDeserializationCallback.OnDeserialization(object? sender) => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_SerializationDeprecated);

            protected override void OnDeserialization(object? sender) => ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_SerializationDeprecated);
#endif
        }
    }
}
