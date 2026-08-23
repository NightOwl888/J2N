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

namespace J2N
{
    /// <summary>
    /// Contract that specifies this type is capable of copying its contents to an array.
    /// </summary>
    /// <typeparam name="T">The type of array element.</typeparam>
    internal interface ICopyable<T>
    {
        /// <summary>
        /// Gets the length of this instance.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Copies the elements from a specified segment of this instance to a specified
        /// segment of a destination array.
        /// </summary>
        /// <param name="sourceIndex">The starting position in this instance where elements will be copied from.
        /// The index is zero-based.</param>
        /// <param name="destination">The array where elements will be copied.</param>
        /// <param name="destinationIndex">The starting position in <paramref name="destination"/> where elements
        /// will be copied. The index is zero-based.</param>
        /// <param name="count">The number of elements to be copied.</param>
        void CopyTo(int sourceIndex, T[] destination, int destinationIndex, int count);
    }
}
