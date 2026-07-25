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
    /// Provides a contract for generating and releasing array storage.
    /// </summary>
    /// <typeparam name="T">The type of array element.</typeparam>
    public interface IArrayAllocator<T>
    {
        /// <summary>
        /// Gets a value indicating whether the allocator gurantees array clearing or requires
        /// clearing unused elements for safe public exposure.
        /// </summary>
        bool GuaranteesClearedArrays { get; }

        /// <summary>
        /// Retrieves a buffer that is at least the requested length.
        /// </summary>
        /// <param name="minimumLength">The minimum length of the array.</param>
        /// <returns>An array of type <typeparamref name="T"/> that is at least <paramref name="minimumLength"/> in length.</returns>
        /// <exception cref="OutOfMemoryException">The allocator cannot provide an array whose length is at least
        /// <paramref name="minimumLength"/>.</exception>
        /// /// <remarks>
        /// Implementations must either:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             Return an array whose length is at least <paramref name="minimumLength"/>.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             Throw an exception if such an array cannot be provided.
        ///         </description>
        ///     </item>
        /// </list>
        /// Returning an array whose length is less than <paramref name="minimumLength"/> violates the contract.
        /// </remarks>
        T[] Allocate(int minimumLength);

        /// <summary>
        /// Returns an array that was previously obtained using the <see cref="Allocate(int)"/>
        /// method on the same instance. This transfers ownership of the array from the caller
        /// back to the underlying implementation.
        /// </summary>
        /// <param name="array">A buffer that was previously obtained using the <see cref="Allocate(int)"/>.</param>
        /// <remarks>
        /// Once a buffer has been returned, the caller gives up all ownership of the buffer and must not use it.
        /// The reference returned from <see cref="Allocate(int)"/> must only be returned using the
        /// <see cref="Return(T[])"/> method once.
        /// </remarks>
        void Return(T[] array);
    }
}
