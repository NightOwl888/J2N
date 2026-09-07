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
using System.Buffers;

namespace J2N.Buffers
{
    /// <summary>
    /// Extensions to <see cref="ArrayPool{T}"/>
    /// </summary>
    internal static class ArrayPoolExtensions
    {
        /// <summary>
        /// Returns to the pool an array that was previously obtained via <see cref="ArrayPool{T}.Rent"/> on the same
        /// <see cref="ArrayPool{T}"/> instance. This method is a no-op if <paramref name="array"/> is <c>null</c>.
        /// </summary>
        /// <param name="pool">This <see cref="ArrayPool{T}"/>.</param>
        /// <param name="array">
        /// The buffer previously obtained from <see cref="ArrayPool{T}.Rent"/> to return to the pool. If <c>null</c>,
        /// no operation will take place.
        /// </param>
        /// <param name="clearArray">
        /// If <c>true</c> and if the pool will store the buffer to enable subsequent reuse, <see cref="ReturnIfNotNull"/>
        /// will clear <paramref name="array"/> of its contents so that a subsequent consumer via <see cref="ArrayPool{T}.Rent"/>
        /// will not see the previous consumer's content.  If <c>false</c> or if the pool will release the buffer,
        /// the array's contents are left unchanged.
        /// </param>
        /// <remarks>
        /// Once a buffer has been returned to the pool, the caller gives up all ownership of the buffer
        /// and must not use it. The reference returned from a given call to <see cref="ArrayPool{T}.Rent"/> must only be
        /// returned via <see cref="ReturnIfNotNull"/> once.  The default <see cref="ArrayPool{T}"/>
        /// may hold onto the returned buffer in order to rent it again, or it may release the returned buffer
        /// if it's determined that the pool already has enough buffers stored.
        /// </remarks>
        public static void ReturnIfNotNull<T>(this ArrayPool<T> pool, T[]? array, bool clearArray = false)
        {
            if (array != null)
            {
                pool.Return(array, clearArray);
            }
        }

        /// <summary>
        /// Returns an array and clears up to the specified length.
        /// </summary>
        // From: https://github.com/dotnet/runtime/blob/v10.0.10/src/libraries/System.Private.CoreLib/src/System/Buffers/ArrayPool.cs#L101-L105
        internal static void Return<T>(this ArrayPool<T> pool, T[] array, int lengthToClear)
        {
            array.AsSpan(0, lengthToClear).Clear();
            pool.Return(array);
        }
    }
}
