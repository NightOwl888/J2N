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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.IO.MemoryMappedFiles
{
    /// <summary>
    /// Wrapper class to box the <see cref="MemoryMappedDirectAccessor"/>
    /// so we have the same reference in all clones.
    /// </summary>
    internal sealed class MemoryMappedDirectAccessorReference : IDisposable
    {
        private readonly MemoryMappedDirectAccessor accessor;

        public MemoryMappedDirectAccessorReference(MemoryMappedDirectAccessor accessor)
        {
            this.accessor = accessor;
        }

        /// <summary>
        /// Gets or sets a byte at the provided <paramref name="index"/>.
        /// </summary>
        /// <param name="index">The zero-based index of the byte to get or set.</param>
        /// <value>The byte at the specified index.</value>
        /// <remarks>No bounds checking is performed.</remarks>
        public byte this[int index]
        {
            get => accessor[index];
            set => accessor.SetByte(index, value);
        }

        /// <summary>
        /// Gets the number of bytes by which the starting position of this segment is offset from the beginning of the memory-mapped file.
        /// </summary>
        public long Offset => accessor.Offset;

        /// <summary>
        /// Gets length of the mapped segment, in bytes.
        /// </summary>
        public int Length => accessor.Length;

        /// <summary>
        /// Creates a new <see cref="Span{Byte}"/> over the memory of this memory mapped view.
        /// </summary>
        /// <returns>The writable span representation of the view.</returns>
        public Span<byte> AsSpan() => accessor.AsSpan();

        /// <summary>
        /// Creates a new <see cref="Span{Byte}"/> over a portion of the memory of this view from a specified
        /// position to the end of the view.
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <returns>The writable span representation of the view.</returns>
        /// <remarks>No bounds checking is performed on <paramref name="start"/>.</remarks>
        public Span<byte> AsSpan(int start) => accessor.AsSpan(start);

        /// <summary>
        /// Creates a new <see cref="Span{Byte}"/> over a portion of the memory of this view from a specified
        /// position and <paramref name="length"/>.
        /// </summary>
        /// <param name="start">The index at which to begin this slice.</param>
        /// <param name="length">The number of bytes in the span.</param>
        /// <returns>The writable span representation of the view.</returns>
        /// <remarks>No bounds checking is performed on <paramref name="start"/> or <paramref name="length"/>.</remarks>
        public Span<byte> AsSpan(int start, int length) => accessor.AsSpan(start, length);


        /// <summary>
        /// Clears all buffers for this view and causes any buffered data to be written to the underlying file.
        /// </summary>
        public void Flush() => accessor.Flush();

        /// <summary>
        /// Releases virtual memory associated with the mapped file segment.
        /// </summary>
        public void Dispose() => accessor.Dispose();
    }
}
