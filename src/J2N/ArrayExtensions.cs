#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

namespace J2N
{
    /// <summary>
    /// Extensions to <see cref="System.Array"/> types.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        /// Assigns the specified value to each element of the specified array.
        /// </summary>
        /// <typeparam name="T">The type array element.</typeparam>
        /// <param name="array">The array to be filled.</param>
        /// <param name="value">The value to be set to all of the elements of the array.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        public static void Fill<T>(this T[] array, T value)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));

            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }

        /// <summary>
        /// Assigns the specified value to each element of the specified array.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the <paramref name="length"/> parameter
        /// is a length, not an exclusive end index as would be the case in Java. To translate from Java,
        /// use <c>endIndex - beginIndex</c> to resolve the value for <paramref name="length"/>.
        /// </summary>
        /// <typeparam name="T">The type array element.</typeparam>
        /// <param name="array">The array to be filled.</param>
        /// <param name="startIndex">The index of the first element (inclusive) to be filled with the specified <paramref name="value"/>.</param>
        /// <param name="length">The number of elements to set to the specified <paramref name="value"/>.</param>
        /// <param name="value">The value to be set to the specified elements of the array.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> or <paramref name="length"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> > <paramref name="length"/> indicates a position outside of the bounds of the <paramref name="array"/>.
        /// </exception>
        public static void Fill<T>(this T[] array, int startIndex, int length, T value)
        {
            if (array is null)
                throw new ArgumentNullException(nameof(array));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            if (startIndex > array.Length - length)
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);

            int end = startIndex + length;
            for (int i = startIndex; i < end; i++)
                array[i] = value;
        }
    }
}
