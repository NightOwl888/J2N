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

using System.Buffers;
using System.Diagnostics;

namespace J2N.Buffers
{
    /// <summary>
    /// An allocator that uses a <see cref="ArrayPool{T}"/> to manage
    /// array instances using pooling.
    /// </summary>
    /// <typeparam name="T">The type of array element.</typeparam>
    internal sealed class PooledArrayAllocator<T> : IArrayAllocator<T>
    {
        private readonly ArrayPool<T> pool;
        private readonly bool clearArrays;

        private PooledArrayAllocator(ArrayPool<T> pool, bool clearArrays)
        {
            Debug.Assert(pool is not null);
            this.pool = pool!; // [!] asserted above
            this.clearArrays = clearArrays;
        }

        /// <summary>
        /// A holder for the uncleared array pool allocator instance. Note that if the <see cref="Uncleared"/>
        /// property is never called, this does not get instantiated.
        /// </summary>
        private static class UnclearedArrayPoolHolder
        {
            public static readonly PooledArrayAllocator<T> Instance = new(ArrayPool<T>.Create(), clearArrays: false);
        }

        /// <summary>
        /// A holder for the cleared array pool allocator instance. Note that if the <see cref="Cleared"/>
        /// property is never called, this does not get instantiated.
        /// </summary>
        private static class ClearedArrayPoolHolder
        {
            public static readonly PooledArrayAllocator<T> Instance = new(ArrayPool<T>.Create(), clearArrays: true);
        }

        /// <summary>
        /// Gets an instance of <see cref="PooledArrayAllocator{T}"/> that does not
        /// clear arrays as it returns them to the pool.
        /// </summary>
        /// <remarks>
        /// The underlying pool is shared, but is not the same instance as <see cref="ArrayPool{T}.Shared"/>
        /// and may have different performance characteristics. However, since it is a separate pool,
        /// sensitive data that is exposed when returned to this pool is not available to callers of
        /// <see cref="ArrayPool{T}.Shared"/>.
        /// </remarks>
        public static PooledArrayAllocator<T> Uncleared => UnclearedArrayPoolHolder.Instance;

        /// <summary>
        /// Gets an instance of <see cref="PooledArrayAllocator{T}"/> that clears
        /// arrays as it returns them to the pool.
        /// </summary>
        /// <remarks>
        /// The underlying pool is shared, but is not the same instance as <see cref="ArrayPool{T}.Shared"/>
        /// and may have different performance characteristics.
        /// </remarks>
        public static PooledArrayAllocator<T> Cleared => ClearedArrayPoolHolder.Instance;

        /// <inheritdoc/>
        public bool GuaranteesClearedArrays => clearArrays;

        /// <inheritdoc/>
        public T[] Allocate(int minimumLength)
            => pool.Rent(minimumLength);

        /// <inheritdoc/>
        public void Return(T[] array)
            => pool.Return(array, clearArrays);
    }
}
