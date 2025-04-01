#region Copyright 2019-2025 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace J2N.IO.MemoryMappedFiles
{
    /// <summary>
    /// An unsafe accessor to provide direct access to the memory mapped data.
    /// </summary>
    internal unsafe struct MemoryMappedDirectAccessor
    {
        private /*readonly*/ MemoryMappedViewAccessor accessor;
        private /*readonly*/ SafeBuffer bufferHandle;
        private /*readonly*/ byte* pointer;
        private /*readonly*/ long viewOffset;
        private /*readonly*/ int length;

        public MemoryMappedDirectAccessor(MemoryMappedViewAccessor accessor, long viewOffset)
        {
            Debug.Assert(accessor != null);

            this.accessor = accessor!;
            this.viewOffset = viewOffset;
            this.length = (int)this.accessor.Capacity;
            bufferHandle = this.accessor.SafeMemoryMappedViewHandle;
            pointer = default;
            bufferHandle.AcquirePointer(ref pointer);
            // Apply the offset once so future indexing is simpler
            pointer += viewOffset;
        }

        /// <summary>
        /// Gets a byte at the provided <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the byte to get.</param>
        /// <returns>The byte at the specified index</returns>
        /// <remarks>No bounds checking is performed.</remarks>
        public byte this[int index] => pointer[viewOffset + index];

        /// <summary>
        /// Sets a byte at the provided <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the byte to set.</param>
        /// <param name="value">The byte value.</param>
        /// <remarks>No bounds checking is performed.</remarks>
        public void SetByte(int index, byte value)
        {
            pointer[viewOffset + index] = value;
        }

        /// <summary>
        /// Creates a new <see cref="Span{Byte}"/> over the memory of this memory mapped view.
        /// </summary>
        /// <returns>The writable span representation of the view.</returns>
        public readonly Span<byte> AsSpan() => new Span<byte>(pointer, (int)accessor.Capacity);

        /// <summary>
        /// Creates a new <see cref="Span{Byte}"/> over a portion of the memory of this view from a specified
        /// position to the end of the view.
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <returns>The writable span representation of the view.</returns>
        /// <remarks>No bounds checking is performed on <paramref name="start"/>.</remarks>
        public readonly Span<byte> AsSpan(int start) => new Span<byte>(pointer + start, (int)accessor.Capacity - start);

        /// <summary>
        /// Creates a new <see cref="Span{Byte}"/> over a portion of the memory of this view from a specified
        /// position and <paramref name="length"/>.
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The number of bytes in the span.</param>
        /// <returns>The writable span representation of the view.</returns>
        /// <remarks>No bounds checking is performed on <paramref name="start"/> or <paramref name="length"/>.</remarks>
        public readonly Span<byte> AsSpan(int start, int length) => new Span<byte>(pointer + start, length);

        /// <summary>
        /// Gets the number of bytes by which the starting position of this segment is offset from the beginning of the memory-mapped file.
        /// </summary>
        public readonly long Offset => viewOffset;

        /// <summary>
        /// Gets length of the mapped segment, in bytes.
        /// </summary>
        public readonly int Length => length;

        /// <summary>
        /// Clears all buffers for this view and causes any buffered data to be written to the underlying file.
        /// </summary>
        public void Flush() => accessor.Flush();

        /// <summary>
        /// Releases virtual memory associated with the mapped file segment.
        /// </summary>
        public void Dispose()
        {
            if (pointer is not null)
            {
                bufferHandle.ReleasePointer();
                pointer = default;
            }
            accessor.Dispose();
        }
    }
}
