#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using J2N.Text;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Globalization
{
    internal static partial class Ordinal
    {
        private const int CharStackBufferSize = 256;

        #region CompareString

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CompareString(ICharSequence? x, StringBuilder? y)
            => CompareString(y, x) * -1;

        internal static int CompareString(StringBuilder? x, ICharSequence? y)
        {
            if (x is null) return (y is null || !y.HasValue) ? 0 : -1;
            if (y is null || !y.HasValue) return 1;

            if (y is ISpannable<char> spannable)
            {
                return CompareString(x, spannable.AsSpan());
            }
            if (y is StringBuilderCharSequence sb)
            {
                return CompareString(x, sb.Value);
            }
            if (y is SynchronizedTextBuilderCharSequence stb)
            {
                lock (stb.SyncRoot)
                {
                    return CompareString(x, stb.Value.AsSpan());
                }
            }
            if (y is StringBuffer stringBuffer)
            {
                lock (stringBuffer.SyncRoot)
                {
                    return CompareString(x, stringBuffer.builder);
                }
            }

            int xLength = x.Length;
            int yLength = y.Length;
            int result;
#if FEATURE_STRINGBUILDER_GETCHUNKS
            int yIndex = 0;
            int remaining = Math.Min(xLength, yLength);

            foreach (ReadOnlyMemory<char> chunk in x.GetChunks())
            {
                ReadOnlySpan<char> chunkSpan = chunk.Span;
                int count = Math.Min(remaining, chunkSpan.Length);

                for (int i = 0; i < count; i++, yIndex++)
                {
                    if ((result = chunkSpan[i] - y[yIndex]) != 0)
                        return result;
                }

                remaining -= count;
                if (remaining == 0)
                    break;
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return xLength - yLength;
#else
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> xChars = xLength > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength))
                    : stackalloc char[xLength];
                x.CopyTo(0, xChars, xLength);
#else
                Span<char> xChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength);
                x.CopyTo(0, arrayToReturnToPool, 0, xLength);
#endif
                int count = Math.Min(xLength, yLength);
                for (int i = 0; i < count; i++)
                {
                    if ((result = xChars[i] - y[i]) != 0)
                        return result;
                }

                // At this point, we have compared all the characters in at least one string.
                // The longer string will be larger.
                return xLength - yLength;
            }
            finally
            {
                if (arrayToReturnToPool is not null)
                {
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
                }
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CompareString(ReadOnlySpan<char> x, StringBuilder? y)
            => CompareString(y, x) * -1;

        internal static int CompareString(StringBuilder? x, ReadOnlySpan<char> y)
        {
            if (x is null) return -1;

#if FEATURE_STRINGBUILDER_GETCHUNKS
            int result;
            int yIndex = 0;
            int remaining = Math.Min(x.Length, y.Length);

            foreach (ReadOnlyMemory<char> chunk in x.GetChunks())
            {
                ReadOnlySpan<char> chunkSpan = chunk.Span;
                int count = Math.Min(remaining, chunkSpan.Length);

                if (count > 8)
                {
                    if ((result = chunkSpan.Slice(0, count).SequenceCompareTo(y.Slice(yIndex, count))) != 0)
                        return result;
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        if ((result = chunkSpan[i] - y[yIndex + i]) != 0)
                            return result;
                    }
                }

                yIndex += count;
                remaining -= count;
                if (remaining == 0)
                    break;
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return x.Length - y.Length;
#else
            int xLength = x.Length;
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> xChars = xLength > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength))
                    : stackalloc char[xLength];
                x.CopyTo(0, xChars, xLength);
#else
                Span<char> xChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength);
                x.CopyTo(0, arrayToReturnToPool, 0, xLength);
#endif
                return xChars.Slice(0, xLength).SequenceCompareTo(y);
            }
            finally
            {
                if (arrayToReturnToPool is not null)
                {
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
                }
            }
#endif
        }

        internal static int CompareString(StringBuilder? x, StringBuilder? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return (y is null) ? 0 : -1;
            if (y is null) return 1;

#if FEATURE_STRINGBUILDER_GETCHUNKS
            StringBuilder.ChunkEnumerator xChunks = x.GetChunks();
            StringBuilder.ChunkEnumerator yChunks = y.GetChunks();
            ReadOnlySpan<char> xSpan = default;
            ReadOnlySpan<char> ySpan = default;
            int result = 0;
            int xIndex = 0;
            int yIndex = 0;
            bool hasX = xChunks.MoveNext();
            bool hasY = yChunks.MoveNext();

            if (hasX)
                xSpan = xChunks.Current.Span;

            if (hasY)
                ySpan = yChunks.Current.Span;

            while (hasX && hasY)
            {
                int count = Math.Min(xSpan.Length - xIndex, ySpan.Length - yIndex);

                if (count > 8)
                {
                    if ((result = xSpan.Slice(xIndex, count).SequenceCompareTo(ySpan.Slice(yIndex, count))) != 0)
                    {
                        return result;
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        if ((result = xSpan[xIndex + i] - ySpan[yIndex + i]) != 0)
                            return result;
                    }
                }

                xIndex += count;
                yIndex += count;

                if (xIndex == xSpan.Length)
                {
                    hasX = xChunks.MoveNext();
                    if (hasX)
                    {
                        xSpan = xChunks.Current.Span;
                        xIndex = 0;
                    }
                }

                if (yIndex == ySpan.Length)
                {
                    hasY = yChunks.MoveNext();
                    if (hasY)
                    {
                        ySpan = yChunks.Current.Span;
                        yIndex = 0;
                    }
                }
            }

            // At this point, we have compared all the characters in at least one string.
            // The longer string will be larger.
            return x.Length - y.Length;
#else
            int xLength = x.Length;
            int yLength = y.Length;
            char[]? xArrayToReturnToPool = null;
            char[]? yArrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> xChars = xLength > CharStackBufferSize
                    ? (xArrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength))
                    : stackalloc char[xLength];
                Span<char> yChars = yLength > CharStackBufferSize
                    ? (yArrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength))
                    : stackalloc char[yLength];
                x.CopyTo(0, xChars, xLength);
                y.CopyTo(0, yChars, yLength);
#else
                Span<char> xChars = xArrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength);
                Span<char> yChars = yArrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength);
                x.CopyTo(0, xArrayToReturnToPool, 0, xLength);
                y.CopyTo(0, yArrayToReturnToPool, 0, yLength);
#endif
                return xChars.Slice(0, xLength).SequenceCompareTo(yChars.Slice(0, yLength));
            }
            finally
            {
                if (xArrayToReturnToPool is not null)
                {
                    ArrayPool<char>.Shared.Return(xArrayToReturnToPool);
                }
                if (yArrayToReturnToPool is not null)
                {
                    ArrayPool<char>.Shared.Return(yArrayToReturnToPool);
                }
            }
#endif
        }

        #endregion CompareString

        #region Equal

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Equal(ICharSequence? x, StringBuilder? y)
            => Equal(y, x);

        internal static bool Equal(StringBuilder? x, ICharSequence? y)
        {
            if (x is null) return y is null || !y.HasValue;
            if (y is null || !y.HasValue) return false;

            if (y is ISpannable<char> spannable)
            {
                return Equal(x, spannable.AsSpan());
            }
            if (y is StringBuilderCharSequence sb)
            {
                return Equal(x, sb.Value);
            }
            if (y is SynchronizedTextBuilderCharSequence stb)
            {
                lock (stb.SyncRoot)
                {
                    return Equal(x, stb.Value.AsSpan());
                }
            }
            if (y is StringBuffer stringBuffer)
            {
                lock (stringBuffer.SyncRoot)
                {
                    return Equal(x, stringBuffer.builder);
                }
            }

            int xLength = x.Length;
            int yLength = y.Length;
            if (xLength != yLength) return false;
            if (xLength == 0) return true;

#if FEATURE_STRINGBUILDER_GETCHUNKS
            int offset = 0;
            foreach (ReadOnlyMemory<char> xChunk in x.GetChunks())
            {
                ReadOnlySpan<char> xSpan = xChunk.Span;

                for (int i = 0; i < xChunk.Length; i++)
                {
                    if (xSpan[i] != y[offset + i])
                        return false;
                }

                offset += xChunk.Length;
            }
            Debug.Assert(offset == xLength);
            return true;
#else
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> xChars = xLength > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength))
                    : stackalloc char[xLength];
                x.CopyTo(0, xChars, xLength);
#else
                Span<char> xChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength);
                x.CopyTo(0, arrayToReturnToPool, 0, xLength);
#endif
                for (int i = 0; i < xLength; i++)
                {
                    if (xChars[i] != y[i])
                        return false;
                }

                return true;
            }
            finally
            {
                if (arrayToReturnToPool is not null)
                {
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
                }
            }
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Equal(ReadOnlySpan<char> x, StringBuilder? y)
            => Equal(y, x);

        internal static bool Equal(StringBuilder? x, ReadOnlySpan<char> y)
        {
            if (x is null)
                return false;

            int xLength = x.Length;
            int yLength = y.Length;
            if (xLength != yLength) return false;
            if (xLength == 0) return true;

#if FEATURE_STRINGBUILDER_GETCHUNKS
            int offset = 0;
            foreach (ReadOnlyMemory<char> xChunk in x.GetChunks())
            {
                int xChunkLength = xChunk.Length;
                ReadOnlySpan<char> yChunk = y.Slice(offset, xChunkLength);

                if (xChunkLength > 8)
                {
                    if (!xChunk.Span.SequenceEqual(yChunk))
                        return false;
                }
                else
                {
                    ReadOnlySpan<char> xChunkSpan = xChunk.Span;
                    for (int i = 0; i < xChunkLength; i++)
                    {
                        if (xChunkSpan[i] != yChunk[i])
                            return false;
                    }
                }

                offset += xChunk.Length;
            }
            Debug.Assert(offset == xLength);
            return true;
#else
            char[]? arrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> xChars = xLength > CharStackBufferSize
                    ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength))
                    : stackalloc char[xLength];
                x.CopyTo(0, xChars, xLength);
#else
                Span<char> xChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength);
                x.CopyTo(0, arrayToReturnToPool, 0, xLength);
#endif
                if (xLength > 8)
                {
                    return xChars.Slice(0, xLength).SequenceEqual(y);
                }
                else
                {
                    for (int i = 0; i < yLength; i++)
                    {
                        if (xChars[i] != y[i])
                            return false;
                    }

                    return true;
                }
            }
            finally
            {
                if (arrayToReturnToPool is not null)
                {
                    ArrayPool<char>.Shared.Return(arrayToReturnToPool);
                }
            }
#endif
        }

        internal static bool Equal(StringBuilder? x, StringBuilder? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null) return y is null;
            if (y is null) return false;

            int xLength = x.Length;
            int yLength = y.Length;
            if (xLength != yLength) return false;
            if (xLength == 0) return true;

#if FEATURE_STRINGBUILDER_GETCHUNKS
            StringBuilder.ChunkEnumerator xChunks = x.GetChunks();
            StringBuilder.ChunkEnumerator yChunks = y.GetChunks();
            ReadOnlySpan<char> xSpan = default;
            ReadOnlySpan<char> ySpan = default;
            int xIndex = 0;
            int yIndex = 0;
            bool hasX = xChunks.MoveNext();
            bool hasY = yChunks.MoveNext();

            if (hasX)
                xSpan = xChunks.Current.Span;

            if (hasY)
                ySpan = yChunks.Current.Span;

            while (hasX && hasY)
            {
                int count = Math.Min(xSpan.Length - xIndex, ySpan.Length - yIndex);

                if (count > 8)
                {
                    if (!xSpan.Slice(xIndex, count).SequenceEqual(ySpan.Slice(yIndex, count)))
                    {
                        return false;
                    }
                }
                else
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (xSpan[xIndex + i] != ySpan[yIndex + i])
                            return false;
                    }
                }

                xIndex += count;
                yIndex += count;

                if (xIndex == xSpan.Length)
                {
                    hasX = xChunks.MoveNext();
                    if (hasX)
                    {
                        xSpan = xChunks.Current.Span;
                        xIndex = 0;
                    }
                }

                if (yIndex == ySpan.Length)
                {
                    hasY = yChunks.MoveNext();
                    if (hasY)
                    {
                        ySpan = yChunks.Current.Span;
                        yIndex = 0;
                    }
                }
            }

            Debug.Assert(!hasX);
            Debug.Assert(!hasY);
            return true;
#else
            char[]? xArrayToReturnToPool = null;
            char[]? yArrayToReturnToPool = null;
            try
            {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                Span<char> xChars = xLength > CharStackBufferSize
                    ? (xArrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength))
                    : stackalloc char[xLength];
                Span<char> yChars = yLength > CharStackBufferSize
                    ? (yArrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength))
                    : stackalloc char[yLength];
                x.CopyTo(0, xChars, xLength);
                y.CopyTo(0, yChars, yLength);
#else
                Span<char> xChars = xArrayToReturnToPool = ArrayPool<char>.Shared.Rent(xLength);
                Span<char> yChars = yArrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength);
                x.CopyTo(0, xArrayToReturnToPool, 0, xLength);
                y.CopyTo(0, yArrayToReturnToPool, 0, yLength);
#endif
                if (xLength > 8)
                {
                    return xChars.Slice(0, xLength).SequenceEqual(yChars.Slice(0, yLength));
                }
                else
                {
                    for (int i = 0; i < yLength; i++)
                    {
                        if (xChars[i] != yChars[i])
                            return false;
                    }

                    return true;
                }
            }
            finally
            {
                if (xArrayToReturnToPool is not null)
                {
                    ArrayPool<char>.Shared.Return(xArrayToReturnToPool);
                }
                if (yArrayToReturnToPool is not null)
                {
                    ArrayPool<char>.Shared.Return(yArrayToReturnToPool);
                }
            }
#endif
        }

        #endregion Equal
    }
}
