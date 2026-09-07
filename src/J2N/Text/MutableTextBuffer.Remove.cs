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

using J2N.CodeGeneration;
using System.Runtime.CompilerServices;

namespace J2N.Text
{
    internal partial class MutableTextBuffer
    {
        /// <summary>
        /// Deletes a sequence of characters specified by <paramref name="startIndex"/> and <paramref name="count"/>.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Delete{TBuilder}(TBuilder, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void DeleteInternal(int startIndex, int count) // Coverage for the JDK
        {
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            int pos = m_Position;
            if ((uint)startIndex + (uint)count > pos)
                count = pos - startIndex;
            if (count > 0)
                RemoveCore(startIndex, count);
        }

        /// <summary>
        /// Removes the character at the specified index from this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.RemoveAt{TBuilder}(TBuilder, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void RemoveAtInternal(int index) // Coverage for the JDK (deleteCharAt)
        {
            if (index < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);

            int currentLength = Length;
            if ((uint)index >= (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(index, ExceptionArgument.index);
            }

            RemoveCore(index, 1);
        }

        /// <summary>
        /// Removes the specified range of characters from this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Remove{TBuilder}(TBuilder, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationExtensionImplementation]
        internal void RemoveInternal(int startIndex, int length)
        {
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);

            if (length > m_Position - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(length, ExceptionArgument.length);
            }

            RemoveCore(startIndex, length);
        }
    }
}
