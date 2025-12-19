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
        internal sealed class TreeSubSet : SortedSet<T>
#if FEATURE_SERIALIZABLE
            , ISerializable, IDeserializationCallback
#endif
        {
            private SortedSet<T> _underlying;
            private T? _min;
            private T? _max;
            // keeps track of whether the count variable is up to date
            // up to date -> _countVersion = _underlying.version
            // not up to date -> _countVersion < _underlying.version
            private int _countVersion;
            // these exist for unbounded collections
            // for instance, you could allow this subset to be defined for i > 10. The set will throw if
            // anything <= 10 is added, but there is no upper bound. These features Head(), Tail(), were punted
            // in the spec, and are not available, but the framework is there to make them available at some point.
            private bool _lBoundActive, _uBoundActive;
            private bool _lBoundInclusive, _uBoundInclusive;

            // used to see if the count is out of date



            #region Properties for Alternate Lookup

            // J2N: This is state from TreeSubSet exposed to allow range checks in Alternate Lookup

            internal override SortedSet<T> UnderlyingSet => _underlying;

            internal override bool HasLowerBound => _lBoundActive;
            internal override bool HasUpperBound => _uBoundActive;

            internal override bool LowerBoundInclusive => _lBoundInclusive;
            internal override bool UpperBoundInclusive => _uBoundInclusive;

            internal override T? LowerBound => _min;
            internal override T? UpperBound => _max;

            #endregion

#if DEBUG
            internal override bool versionUpToDate()
            {
                return (version == _underlying.version);
            }
#endif

            public TreeSubSet(SortedSet<T> Underlying, [AllowNull] T Min, bool lowerBoundInclusive, [AllowNull] T Max, bool upperBoundInclusive, bool lowerBoundActive, bool upperBoundActive)
                : base(Underlying.Comparer)
            {
                _underlying = Underlying;
                _min = Min;
                _max = Max;
                _lBoundInclusive = lowerBoundInclusive;
                _uBoundInclusive = upperBoundInclusive;
                _lBoundActive = lowerBoundActive;
                _uBoundActive = upperBoundActive;
                root = _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive); // root is first element within range
                count = 0;
                version = -1;
                _countVersion = -1;
            }

            internal override bool AddIfNotPresent(T item)
            {
                if (!IsWithinRange(item))
                {
                    throw new ArgumentOutOfRangeException(nameof(item));
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

            internal override bool DoRemove(T item)
            {
                if (!IsWithinRange(item))
                {
                    return false;
                }

                bool ret = _underlying.Remove(item);
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
            internal override bool IsWithinRange(T item)
            {
                return !IsTooLow(item) && !IsTooHigh(item);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal override bool IsTooHigh([AllowNull] T item)
            {
                return IsTooHigh(item, _uBoundInclusive);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool IsTooHigh([AllowNull] T item, bool upperBoundInclusive)
            {
                if (_uBoundActive)
                {
                    int c = Comparer.Compare(item!, _max!);
                    if (c > 0 || (c == 0 && !upperBoundInclusive))
                        return true;
                }
                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal override bool IsTooLow([AllowNull] T item)
            {
                return IsTooLow(item, _lBoundInclusive);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private bool IsTooLow([AllowNull] T item, bool lowerBoundInclusive)
            {
                if (_lBoundActive)
                {
                    int c = Comparer.Compare(item!, _min!);
                    if (c < 0 || (c == 0 && !lowerBoundInclusive))
                        return true;
                }
                return false;
            }

            internal override T MinInternal
            {
                get
                {
                    Node? current = root;
                    T? result = default;

                    while (current != null)
                    {
                        int comp = _lBoundActive ? Comparer.Compare(_min!, current.Item!) : -1;
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

                    return result!;
                }
            }

            internal override T MaxInternal
            {
                get
                {
                    Node? current = root;
                    T? result = default;

                    while (current != null)
                    {
                        int comp = _uBoundActive ? Comparer.Compare(_max!, current.Item!) : 1;
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

                    return result!;
                }
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
                    if (current.Left != null && (!_lBoundActive || Comparer.Compare(_min!, current.Item!) < 0))
                    {
                        processQueue.Enqueue(current.Left);
                    }
                    if (current.Right != null && (!_uBoundActive || Comparer.Compare(_max!, current.Item!) > 0))
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
                    if (Comparer.Compare(item, i) == 0)
                        return count;
                }
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif
                return -1;
            }

            /// <summary>
            /// Checks whether this subset is out of date, and updates it if necessary.
            /// <param name="updateCount">Updates the count variable if necessary.</param>
            /// </summary>
            internal override void VersionCheck(bool updateCount = false) => VersionCheckImpl(updateCount);

            private void VersionCheckImpl(bool updateCount)
            {
                Debug.Assert(_underlying != null);
                if (version != _underlying!.version)
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
                return _underlying!.Count;
            }

            // This passes functionality down to the underlying tree, clipping edges if necessary
            // There's nothing gained by having a nested subset. May as well draw it from the base
            // Cannot increase the bounds of the subset, can only decrease it
            public override SortedSet<T> GetViewBetween([AllowNull] T lowerValue, [AllowNull] T upperValue)
            {
                if (IsTooLow(lowerValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(lowerValue));
                }
                if (IsTooHigh(upperValue))
                {
                    throw new ArgumentOutOfRangeException(nameof(upperValue));
                }
                return (TreeSubSet)_underlying.GetViewBetween(lowerValue, upperValue);
            }

            // This passes functionality down to the underlying tree, clipping edges if necessary
            // There's nothing gained by having a nested subset. May as well draw it from the base
            // Cannot increase the bounds of the subset, can only decrease it
            public override SortedSet<T> GetViewBetween([AllowNull] T lowerValue, bool lowerValueInclusive, [AllowNull] T upperValue, bool upperValueInclusive)
            {
                if (IsTooLow(lowerValue, lowerValueInclusive))
                {
                    throw new ArgumentOutOfRangeException(nameof(lowerValue));
                }
                if (IsTooHigh(upperValue, upperValueInclusive))
                {
                    throw new ArgumentOutOfRangeException(nameof(upperValue));
                }
                return (TreeSubSet)_underlying.GetViewBetween(lowerValue, lowerValueInclusive, upperValue, upperValueInclusive);
            }

#if DEBUG
            internal override void IntersectWithEnumerable(IEnumerable<T> other)
            {
                base.IntersectWithEnumerable(other);
                Debug.Assert(versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
            }
#endif

            internal override bool DoTryGetPredecessor(T item, [MaybeNullWhen(false)] out T result)
            {
                VersionCheck();
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif

                // If item is at or below lower bound, no strict predecessor exists
                if (_lBoundActive)
                {
                    int c = Comparer.Compare(item!, _min!);
                    if (c <= 0)
                    {
                        result = default!;
                        return false;
                    }
                }

                Node? current = root;
                Node? match = null;

                while (current != null)
                {
                    int cmp = Comparer.Compare(item, current.Item);

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

            internal override bool DoTryGetSuccessor(T item, [MaybeNullWhen(false)] out T result)
            {
                VersionCheck();
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif

                // If item is at or above upper bound, no strict successor exists
                if (_uBoundActive)
                {
                    int c = Comparer.Compare(item!, _max!);
                    if (c >= 0)
                    {
                        result = default!;
                        return false;
                    }
                }

                Node? current = root;
                Node? match = null;

                while (current != null)
                {
                    int cmp = Comparer.Compare(item, current.Item);

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

            internal override bool DoTryGetFloor(T item, [MaybeNullWhen(false)] out T result)
            {
                VersionCheck();
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif

                Node? current = root;
                Node? candidate = null;

                while (current != null)
                {
                    int cmp = Comparer.Compare(item, current.Item);

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

            internal override bool DoTryGetCeiling(T item, [MaybeNullWhen(false)] out T result)
            {
                VersionCheck();
#if DEBUG
                Debug.Assert(this.versionUpToDate() && root == _underlying.FindRange(_min, _max, _lBoundInclusive, _uBoundInclusive, _lBoundActive, _uBoundActive));
#endif

                Node? current = root;
                Node? candidate = null;

                while (current != null)
                {
                    int cmp = Comparer.Compare(item, current.Item);

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
