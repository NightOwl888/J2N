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

using System;

namespace J2N.Buffers
{
    /// <summary>
    /// An allocator that serves uninitialized arrays if supported by the current platform.
    /// </summary>
    /// <typeparam name="T">The type of array element.</typeparam>
    internal sealed class UninitializedArrayAllocator<T> : IArrayAllocator<T>
    {
        private UninitializedArrayAllocator() { }

        public static UninitializedArrayAllocator<T> Default { get; } = new();

#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
        /// <inheritdoc/>
        public bool GuaranteesClearedArrays => false;

        /// <inheritdoc/>
        public T[] Allocate(int minimumLength)
            => GC.AllocateUninitializedArray<T>(minimumLength);

        /// <inheritdoc/>
        public void Return(T[] array)
        {
            // Intentionally blank
        }
#else
        /// <inheritdoc/>
        public bool GuaranteesClearedArrays => true;

        /// <inheritdoc/>
        public T[] Allocate(int minimumLength)
            => new T[minimumLength];

        /// <inheritdoc/>
        public void Return(T[] array)
        {
            // Intentionally blank
        }
#endif
    }
}
