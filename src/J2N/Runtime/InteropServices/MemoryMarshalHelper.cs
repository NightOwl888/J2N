#region Copyright 2019-2024 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

using J2N.Runtime.CompilerServices;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace J2N.Runtime.InteropServices
{
    internal class MemoryMarshalHelper
    {

#pragma warning disable IDE0060 // https://github.com/dotnet/roslyn-analyzers/issues/6228
        /// <summary>
        /// Returns a reference to the 0th element of the Span. If the Span is empty, returns a reference to fake non-null pointer. Such a reference can be used
        /// for pinning but must never be dereferenced. This is useful for interop with methods that do not accept null pointers for zero-sized buffers.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe ref T GetNonNullPinnableReference<T>(Span<T> span) => ref (span.Length != 0) ? ref MemoryMarshal.GetReference(span) : ref Unsafe.AsRef<T>((void*)1);

        /// <summary>
        /// Returns a reference to the 0th element of the ReadOnlySpan. If the ReadOnlySpan is empty, returns a reference to fake non-null pointer. Such a reference
        /// can be used for pinning but must never be dereferenced. This is useful for interop with methods that do not accept null pointers for zero-sized buffers.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static unsafe ref T GetNonNullPinnableReference<T>(ReadOnlySpan<T> span) => ref (span.Length != 0) ? ref MemoryMarshal.GetReference(span) : ref Unsafe.AsRef<T>((void*)1);
#pragma warning restore IDE0060 // https://github.com/dotnet/roslyn-analyzers/issues/6228
    }
}
