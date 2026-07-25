// Source: https://github.com/dotnet/runtime/blob/v11.0.0-preview.6.26359.118/src/libraries/Common/src/System/Collections/Generic/ValueListBuilder.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Buffers;
using J2N.Runtime.CompilerServices;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace J2N.Collections.Generic
{
    internal ref partial struct ValueListBuilder<T>
    {
        private Span<T> _span;
        private T[]? _arrayFromPool;
        private int _pos;

        public ValueListBuilder(Span<T?> scratchBuffer)
        {
            _span = scratchBuffer!;
        }

        public ValueListBuilder(int capacity)
        {
            Grow(capacity);
        }

        public int Capacity => _span.Length; // J2N: Required for testing, but added for convenience

        public int Length
        {
            get => _pos;
            set
            {
                Debug.Assert(value >= 0);
                Debug.Assert(value <= _span.Length);
                _pos = value;
            }
        }

        public ref T this[int index]
        {
            get
            {
                Debug.Assert(index < _pos);
                return ref _span[index];
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(T item)
        {
            int pos = _pos;

            // Workaround for https://github.com/dotnet/runtime/issues/72004
            Span<T> span = _span;
            if ((uint)pos < (uint)span.Length)
            {
                span[pos] = item;
                _pos = pos + 1;
            }
            else
            {
                AddWithResize(item);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Append(scoped ReadOnlySpan<T> source)
        {
            int pos = _pos;
            Span<T> span = _span;
            if (source.Length == 1 && (uint)pos < (uint)span.Length)
            {
                span[pos] = source[0];
                _pos = pos + 1;
            }
            else
            {
                AppendMultiChar(source);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AppendMultiChar(scoped ReadOnlySpan<T> source)
        {
            if ((uint)(_pos + source.Length) > (uint)_span.Length)
            {
                Grow(source.Length);
            }

            source.CopyTo(_span.Slice(_pos));
            _pos += source.Length;
        }

        public void Insert(int index, scoped ReadOnlySpan<T> source)
        {
            Debug.Assert(index == 0, "Implementation currently only supports index == 0");

            if ((uint)(_pos + source.Length) > (uint)_span.Length)
            {
                Grow(source.Length);
            }

            _span.Slice(0, _pos).CopyTo(_span.Slice(source.Length));
            source.CopyTo(_span);
            _pos += source.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<T> AppendSpan(int length)
        {
            Debug.Assert(length >= 0);

            int pos = _pos;
            Span<T> span = _span;
            if ((uint)(pos + length) <= (uint)span.Length)
            {
                _pos = pos + length;
                return span.Slice(pos, length);
            }
            else
            {
                return AppendSpanWithGrow(length);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private Span<T> AppendSpanWithGrow(int length)
        {
            int pos = _pos;
            Grow(length);
            _pos += length;
            return _span.Slice(pos, length);
        }

        // Hide uncommon path
        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithResize(T item)
        {
            Debug.Assert(_pos == _span.Length);
            int pos = _pos;
            Grow(1);
            _span[pos] = item;
            _pos = pos + 1;
        }

        public ReadOnlySpan<T> AsSpan()
        {
            return _span.Slice(0, _pos);
        }

        public bool TryCopyTo(Span<T> destination, out int itemsWritten)
        {
            if (_span.Slice(0, _pos).TryCopyTo(destination))
            {
                itemsWritten = _pos;
                return true;
            }

            itemsWritten = 0;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            int pos = _pos;
            T[]? toReturn = _arrayFromPool;

            this = default;

            if (toReturn != null)
            {
                if (RuntimeHelper.IsReferenceOrContainsReferences<T>())
                {
                    ArrayPool<T>.Shared.Return(toReturn, pos);
                }
                else
                {
                    ArrayPool<T>.Shared.Return(toReturn);
                }
            }
        }

        /// <summary>
        /// Resize the internal buffer either by doubling current buffer size or
        /// by adding <paramref name="additionalCapacityBeyondPos"/> to
        /// <see cref="_pos"/> whichever is greater.
        /// </summary>
        /// <param name="additionalCapacityBeyondPos">
        /// Number of chars requested beyond current position.
        /// </param>
        /// <remarks>
        /// Note that consuming implementations depend on the list only growing if it's absolutely
        /// required.  If the list is already large enough to hold the additional items be added,
        /// it must not grow. The list is used in a number of places where the reference is checked
        /// and it's expected to match the initial reference provided to the constructor if that
        /// span was sufficiently large.
        /// </remarks>
        private void Grow(int additionalCapacityBeyondPos)
        {
            Debug.Assert(additionalCapacityBeyondPos > 0);
            Debug.Assert(_pos > _span.Length - additionalCapacityBeyondPos, "Grow called incorrectly, no resize is needed.");

            // Double the size of the span.  If it's currently empty, default to size 4,
            // although it'll be increased in Rent to the pool's minimum bucket size.
            int nextCapacity = Math.Max(_span.Length != 0 ? _span.Length * 2 : 4, _pos + additionalCapacityBeyondPos);

            // If the computed doubled capacity exceeds the possible length of an array, then we
            // want to downgrade to either the maximum array length if that's large enough to hold
            // an additional item, or the current length + 1 if it's larger than the max length, in
            // which case it'll result in an OOM when calling Rent below.  In the exceedingly rare
            // case where _span.Length is already int.MaxValue (in which case it couldn't be a managed
            // array), just use that same value again and let it OOM in Rent as well.
            if ((uint)nextCapacity > Arrays.MaxArrayLength)
            {
                nextCapacity = Math.Max(Math.Max(_span.Length + 1, Arrays.MaxArrayLength), _span.Length);
            }

            T[] array = ArrayPool<T>.Shared.Rent(nextCapacity);
            _span.CopyTo(array);

            T[]? toReturn = _arrayFromPool;
            _span = _arrayFromPool = array;
            if (toReturn != null)
            {
                if (RuntimeHelper.IsReferenceOrContainsReferences<T>())
                {
                    ArrayPool<T>.Shared.Return(toReturn, _pos);
                }
                else
                {
                    ArrayPool<T>.Shared.Return(toReturn);
                }
            }
        }
    }
}
