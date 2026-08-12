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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace J2N.Globalization
{
    internal static partial class Ordinal
    {
        #region CompareString

        internal static int CompareString(ICharSequence? x, StringBuffer? y)
        {
            if (x is null || !x.HasValue) return (y is null) ? 0 : -1;
            if (y is null) return 1;

            if (x is ISpannable<char> spannable)
            {
                lock (y.SyncRoot)
                {
                    return CompareString(spannable.AsSpan(), y.builder);
                }
            }
            if (x is SynchronizedTextBuilderCharSequence synchronizedTextBuilderCharSequence)
                return CompareString(synchronizedTextBuilderCharSequence.Value, y);
            if (x is StringBuffer stringBuffer)
                return CompareString(stringBuffer, y);
            if (x is StringBuilderCharSequence stringBuilderCharSequence)
            {
                lock (y.SyncRoot)
                {
                    return CompareString(stringBuilderCharSequence.Value, y.builder);
                }
            }

            lock (y.SyncRoot)
            {
                return CompareString(x, y.builder);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CompareString(StringBuffer? x, ICharSequence? y)
            => CompareString(y, x) * -1;

        internal static int CompareString(ICharSequence? x, SynchronizedTextBuilder? y)
        {
            if (x is null || !x.HasValue) return (y is null) ? 0 : -1;
            if (y is null) return 1;

            if (x is ISpannable<char> spannable)
            {
                lock (y.SyncRoot)
                {
                    return spannable.AsSpan().SequenceCompareTo(y.buffer.AsSpan());
                }
            }
            if (x is SynchronizedTextBuilderCharSequence synchronizedTextBuilderCharSequence)
                return CompareString(synchronizedTextBuilderCharSequence.Value, y);
            if (x is StringBuffer stringBuffer)
                return CompareString(stringBuffer, y);
            if (x is StringBuilderCharSequence stringBuilderCharSequence)
            {
                lock (y.SyncRoot)
                {
                    return CompareString(stringBuilderCharSequence.Value, y.AsSpan());
                }
            }

            lock (y.SyncRoot)
            {
                int length = Math.Min(x.Length, y.buffer.Length);
                int result;
                for (int i = 0; i < length; i++)
                {
                    if ((result = x[i] - y.buffer[i]) != 0)
                        return result;
                }

                // At this point, we have compared all the characters in at least one string.
                // The longer string will be larger.
                return x.Length - y.buffer.Length;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CompareString(SynchronizedTextBuilder? x, ICharSequence? y)
            => CompareString(y, x) * -1;

        internal static int CompareString(StringBuffer? x, StringBuffer? y)
        {
            if (x == y) return 0;
            if (x is null) return (y is null) ? 0 : -1;
            if (y is null) return 1;

            bool lockTaken = false;
            try
            {
                Monitor.Enter(y.SyncRoot, ref lockTaken);
                int yLength = y.builder.Length;
                char[]? arrayToReturnToPool = null;
                try
                {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                    Span<char> yChars = yLength > CharStackBufferSize
                        ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength))
                        : stackalloc char[yLength];
#else
                    Span<char> yChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength);
#endif
                    try
                    {
                        // This line does not lock the other implementation
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                        y.builder.CopyTo(0, yChars, yLength);
#else
                        y.builder.CopyTo(0, arrayToReturnToPool, 0, yLength);
#endif
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            Monitor.Exit(y.SyncRoot);
                            lockTaken = false;
                        }
                    }

                    // Other lock is released, now lock us and do the comparison
                    lock (x.SyncRoot)
                    {
                        return CompareString(x.builder, yChars.Slice(0, yLength));
                    }
                }
                finally
                {
                    if (arrayToReturnToPool is not null)
                    {
                        ArrayPool<char>.Shared.Return(arrayToReturnToPool);
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(y.SyncRoot);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int CompareString(StringBuffer? x, SynchronizedTextBuilder? y)
            => CompareString(y, x) * -1;

        internal static int CompareString(SynchronizedTextBuilder? x, StringBuffer? y)
        {
            if (x is null) return (y is null) ? 0 : -1;
            if (y is null) return 1;

            bool lockTaken = false;
            try
            {
                Monitor.Enter(y.SyncRoot, ref lockTaken);
                int yLength = y.builder.Length;
                char[]? arrayToReturnToPool = null;
                try
                {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                    Span<char> yChars = yLength > CharStackBufferSize
                        ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength))
                        : stackalloc char[yLength];
#else
                    Span<char> yChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength);
#endif
                    try
                    {
                        // This line does not lock the other implementation
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                        y.builder.CopyTo(0, yChars, yLength);
#else
                        y.builder.CopyTo(0, arrayToReturnToPool, 0, yLength);
#endif
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            Monitor.Exit(y.SyncRoot);
                            lockTaken = false;
                        }
                    }

                    // Other lock is released, now lock us and do the comparison
                    lock (x.SyncRoot)
                    {
                        return x.AsSpan().SequenceCompareTo(yChars.Slice(0, yLength));
                    }
                }
                finally
                {
                    if (arrayToReturnToPool is not null)
                    {
                        ArrayPool<char>.Shared.Return(arrayToReturnToPool);
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(y.SyncRoot);
                }
            }
        }

        internal static int CompareString(SynchronizedTextBuilder? x, SynchronizedTextBuilder? y)
        {
            if (x == y) return 0;
            if (x is null) return (y is null) ? 0 : -1;
            if (y is null) return 1;

            bool lockTaken = false;
            try
            {
                Monitor.Enter(y.SyncRoot, ref lockTaken);
                int yLength = y.buffer.Length;
                char[]? arrayToReturnToPool = null;
                try
                {
                    Span<char> yChars = yLength > CharStackBufferSize
                        ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength))
                        : stackalloc char[yLength];
                    try
                    {
                        // This line does not lock the other implementation
                        y.buffer.CopyTo(0, yChars, yLength);
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            Monitor.Exit(y.SyncRoot);
                            lockTaken = false;
                        }
                    }

                    // Other lock is released, now lock us and do the comparison
                    lock (x.SyncRoot)
                    {
                        return x.AsSpan().SequenceCompareTo(yChars.Slice(0, yLength));
                    }
                }
                finally
                {
                    if (arrayToReturnToPool is not null)
                    {
                        ArrayPool<char>.Shared.Return(arrayToReturnToPool);
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(y.SyncRoot);
                }
            }
        }

        #endregion CompareToOrdinal

        #region Equal

        internal static bool Equal(ICharSequence? x, StringBuffer? y)
        {
            if (x is null || !x.HasValue) return y is null;
            if (y is null) return false;

            if (x is ISpannable<char> spannable)
            {
                lock (y.SyncRoot)
                {
                    return Equal(spannable.AsSpan(), y.builder);
                }
            }
            if (x is SynchronizedTextBuilderCharSequence synchronizedTextBuilderCharSequence)
                return Equal(synchronizedTextBuilderCharSequence.Value, y);
            if (x is StringBuffer stringBuffer)
                return Equal(stringBuffer, y);
            if (x is StringBuilderCharSequence stringBuilderCharSequence)
            {
                lock (y.SyncRoot)
                {
                    return Equal(stringBuilderCharSequence.Value, y.builder);
                }
            }

            lock (y.SyncRoot)
            {
                return Equal(x, y.builder);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Equal(StringBuffer? x, ICharSequence? y)
            => Equal(y, x);

        internal static bool Equal(ICharSequence? x, SynchronizedTextBuilder? y)
        {
            if (x is null || !x.HasValue) return y is null;
            if (y is null) return false;

            if (x is ISpannable<char> spannable)
            {
                lock (y.SyncRoot)
                {
                    return spannable.AsSpan().SequenceEqual(y.buffer.AsSpan());
                }
            }
            if (x is SynchronizedTextBuilderCharSequence synchronizedTextBuilderCharSequence)
                return Equal(synchronizedTextBuilderCharSequence.Value, y);
            if (x is StringBuffer stringBuffer)
                return Equal(stringBuffer, y);
            if (x is StringBuilderCharSequence stringBuilderCharSequence)
            {
                lock (y.SyncRoot)
                {
                    return Equal(stringBuilderCharSequence.Value, y.buffer.AsSpan());
                }
            }

            lock (y.SyncRoot)
            {
                int xLength = x.Length;
                int yLength = y.buffer.Length;
                if (xLength != yLength) return false;
                if (xLength == 0) return true;

                for (int i = 0; i < xLength; i++)
                {
                    if (x[i] != y.buffer[i])
                        return false;
                }

                return true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Equal(SynchronizedTextBuilder? x, ICharSequence? y)
            => Equal(y, x);

        internal static bool Equal(StringBuffer? x, StringBuffer? y)
        {
            if (x == y) return true;
            if (x is null) return y is null;
            if (y is null) return false;

            bool lockTaken = false;
            try
            {
                Monitor.Enter(y.SyncRoot, ref lockTaken);
                int yLength = y.builder.Length;

                if (yLength == 0)
                {
                    // This line unlocks the other implementation
                    if (lockTaken)
                    {
                        Monitor.Exit(y.SyncRoot);
                        lockTaken = false;
                    }

                    lock (x.SyncRoot)
                    {
                        return x.builder.Length == 0;
                    }
                }

                char[]? arrayToReturnToPool = null;
                try
                {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                    Span<char> yChars = yLength > CharStackBufferSize
                        ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength))
                        : stackalloc char[yLength];
#else
                    Span<char> yChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength);
#endif
                    try
                    {
                        // This line does not lock the other implementation
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                        y.builder.CopyTo(0, yChars, yLength);
#else
                        y.builder.CopyTo(0, arrayToReturnToPool, 0, yLength);
#endif
                    }
                    finally
                    {
                        if (lockTaken)
                        {
                            Monitor.Exit(y.SyncRoot);
                            lockTaken = false;
                        }
                    }

                    // Other lock is released, now lock us and do the comparison
                    lock (x.SyncRoot)
                    {
                        return Equal(x.builder, yChars.Slice(0, yLength));
                    }
                }
                finally
                {
                    if (arrayToReturnToPool is not null)
                    {
                        ArrayPool<char>.Shared.Return(arrayToReturnToPool);
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(y.SyncRoot);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Equal(StringBuffer? x, SynchronizedTextBuilder? y)
            => Equal(y, x);

        internal static bool Equal(SynchronizedTextBuilder? x, StringBuffer? y)
        {
            if (x is null) return y is null;
            if (y is null) return false;

            bool lockTaken = false;
            try
            {
                Monitor.Enter(y.SyncRoot, ref lockTaken);
                int yLength = y.builder.Length;

                if (yLength == 0)
                {
                    // This line unlocks the other implementation
                    if (lockTaken)
                    {
                        Monitor.Exit(y.SyncRoot);
                        lockTaken = false;
                    }

                    lock (x.SyncRoot)
                    {
                        return x.buffer.Length == 0;
                    }
                }

                char[]? arrayToReturnToPool = null;
                try
                {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                    Span<char> yChars = yLength > CharStackBufferSize
                        ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength))
                        : stackalloc char[yLength];
#else
                    Span<char> yChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength);
#endif
                    try
                    {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN
                        y.builder.CopyTo(0, yChars, yLength);
#else
                        y.builder.CopyTo(0, arrayToReturnToPool, 0, yLength);
#endif
                    }
                    finally
                    {
                        // This line unlocks the other implementation
                        if (lockTaken)
                        {
                            Monitor.Exit(y.SyncRoot);
                            lockTaken = false;
                        }
                    }

                    // Other lock is released, now lock us and do the comparison
                    lock (x.SyncRoot)
                    {
                        int xLength = x.buffer.Length;
                        if (xLength != yLength) return false;
                        if (xLength > 8)
                        {
                            return x.AsSpan().SequenceEqual(yChars.Slice(0, yLength));
                        }
                        else
                        {
                            for (int i = 0; i < yLength; i++)
                            {
                                if (x.buffer[i] != yChars[i])
                                    return false;
                            }

                            return true;
                        }
                    }
                }
                finally
                {
                    if (arrayToReturnToPool is not null)
                    {
                        ArrayPool<char>.Shared.Return(arrayToReturnToPool);
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(y.SyncRoot);
                }
            }
        }

        internal static bool Equal(SynchronizedTextBuilder? x, SynchronizedTextBuilder? y)
        {
            if (x == y) return true;
            if (x is null) return y is null;
            if (y is null) return false;

            bool lockTaken = false;
            try
            {
                Monitor.Enter(y.SyncRoot, ref lockTaken);
                int yLength = y.buffer.Length;

                if (yLength == 0)
                {
                    // This line unlocks the other implementation
                    if (lockTaken)
                    {
                        Monitor.Exit(y.SyncRoot);
                        lockTaken = false;
                    }

                    lock (x.SyncRoot)
                    {
                        return x.buffer.Length == 0;
                    }
                }

                char[]? arrayToReturnToPool = null;
                try
                {
                    Span<char> yChars = yLength > CharStackBufferSize
                        ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(yLength))
                        : stackalloc char[yLength];
                    try
                    {
                        y.buffer.CopyTo(0, yChars, yLength);
                    }
                    finally
                    {
                        // This line unlocks the other implementation
                        if (lockTaken)
                        {
                            Monitor.Exit(y.SyncRoot);
                            lockTaken = false;
                        }
                    }

                    // Other lock is released, now lock us and do the comparison
                    lock (x.SyncRoot)
                    {
                        int xLength = x.buffer.Length;
                        if (xLength != yLength) return false;
                        if (xLength > 8)
                        {
                            return x.AsSpan().SequenceEqual(yChars.Slice(0, yLength));
                        }
                        else
                        {
                            for (int i = 0; i < yLength; i++)
                            {
                                if (x.buffer[i] != yChars[i])
                                    return false;
                            }

                            return true;
                        }
                    }
                }
                finally
                {
                    if (arrayToReturnToPool is not null)
                    {
                        ArrayPool<char>.Shared.Return(arrayToReturnToPool);
                    }
                }
            }
            finally
            {
                if (lockTaken)
                {
                    Monitor.Exit(y.SyncRoot);
                }
            }
        }

        #endregion Equal
    }
}
