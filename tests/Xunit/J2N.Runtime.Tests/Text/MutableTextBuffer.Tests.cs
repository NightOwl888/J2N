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

using J2N.Buffers;
using System;
using System.Text;
#nullable enable

namespace J2N.Text.Tests
{
    public partial class MutableTextBuffer_Tests : StringBuilder_Tests
    {
        internal static readonly IArrayAllocator<char> DefaultAllocator =
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
            UninitializedArrayAllocator<char>.Default;
#else
            ArrayAllocator<char>.Default;
#endif

        private protected override MutableTextBuffer MutableTextBufferFactory(MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize();
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(int capacity, int maxCapacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(capacity, maxCapacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value, int startIndex, int length, int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, startIndex, length, capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value, int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(ReadOnlySpan<char> value, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(ReadOnlySpan<char> value, int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(StringBuilder? value, int startIndex, int length, int capacity, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, startIndex, length, capacity);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(ICharSequence? value, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(DefaultAllocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value);
        }

        private protected override MutableTextBuffer MutableTextBufferFactory(string? value, int capacity, IArrayAllocator<char> allocator, MutableTextBufferTestOptions? options = null)
        {
            return new MutableTextBuffer(allocator)
            {
                UseInvariantDefaults = options?.UseInvariantDefaults ?? false,
                ClearExposedBuffers = options?.ClearExposedBuffers ?? true,
            }.Initialize(value, capacity);
        }

    }
}
