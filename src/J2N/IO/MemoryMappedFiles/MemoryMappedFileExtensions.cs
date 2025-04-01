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
using System.IO.MemoryMappedFiles;

namespace J2N.IO.MemoryMappedFiles
{
    /// <summary>
    /// Extensions to the <see cref="MemoryMappedFile"/> class.
    /// </summary>
    public static class MemoryMappedFileExtensions
    {
        /// <summary>
        /// Creates a <see cref="MemoryMappedViewByteBuffer"/> that maps to a view of the 
        /// memory-mapped file.
        /// </summary>
        /// <param name="memoryMappedFile">This <see cref="MemoryMappedFile"/>.</param>
        /// <returns>A randomly accessible block of memory, as a <see cref="MemoryMappedViewByteBuffer"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="memoryMappedFile"/> is <c>null</c>.</exception>
        /// <exception cref="UnauthorizedAccessException">Access to the memory-mapped file is unauthorized.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        public static MemoryMappedViewByteBuffer CreateViewByteBuffer(this MemoryMappedFile memoryMappedFile)
        {
            return CreateViewByteBuffer(memoryMappedFile, 0, 0, MemoryMappedFileAccess.ReadWrite, 0, 0);
        }

        /// <summary>
        /// Creates a <see cref="MemoryMappedViewByteBuffer"/> that maps to a view of the 
        /// memory-mapped file, and that has the specified <paramref name="offset"/> and <paramref name="size"/>.
        /// </summary>
        /// <param name="memoryMappedFile">This <see cref="MemoryMappedFile"/>.</param>
        /// <param name="offset">The byte at which to start the view.</param>
        /// <param name="size">The size of the view. Specify <c>0</c> (zero) to create a view that
        /// starts at <paramref name="offset"/> and ends approximately at the end of the memory-mapped file.</param>
        /// <returns>A randomly accessible block of memory, as a <see cref="MemoryMappedViewByteBuffer"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="memoryMappedFile"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="size"/> is a negative value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="size"/> is greater than the logical address space.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">Access to the memory-mapped file is unauthorized.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        public static MemoryMappedViewByteBuffer CreateViewByteBuffer(this MemoryMappedFile memoryMappedFile, long offset, int size)
        {
            return CreateViewByteBuffer(memoryMappedFile, offset, size, MemoryMappedFileAccess.ReadWrite, 0, size);
        }

        /// <summary>
        /// Creates a <see cref="MemoryMappedViewByteBuffer"/> that maps to a view of the 
        /// memory-mapped file, and that has the specified <paramref name="offset"/> and <paramref name="size"/>.
        /// </summary>
        /// <param name="memoryMappedFile">This <see cref="MemoryMappedFile"/>.</param>
        /// <param name="offset">The byte at which to start the view.</param>
        /// <param name="size">The size of the view. Specify <c>0</c> (zero) to create a view that
        /// starts at <paramref name="offset"/> and ends approximately at the end of the memory-mapped file.</param>
        /// <param name="access">One of the enumeration values that specifies the type of access allowed to the memory-mapped file. The default is <see cref="MemoryMappedFileAccess.ReadWrite"/>.</param>
        /// <returns>A randomly accessible block of memory, as a <see cref="MemoryMappedViewByteBuffer"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="memoryMappedFile"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="size"/> is a negative value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="size"/> is greater than the logical address space.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="size"/> is greater than <see cref="int.MaxValue"/>.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException"><paramref name="access"/> is invalid for the memory-mapped file.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        public static MemoryMappedViewByteBuffer CreateViewByteBuffer(this MemoryMappedFile memoryMappedFile, long offset, int size, MemoryMappedFileAccess access)
        {
            return CreateViewByteBuffer(memoryMappedFile, offset, size, access, 0, size);
        }

        /// <summary>
        /// Creates a <see cref="MemoryMappedViewByteBuffer"/> that maps to a view of the 
        /// memory-mapped file, and that has the specified <paramref name="offset"/> and <paramref name="size"/>.
        /// </summary>
        /// <param name="memoryMappedFile">This <see cref="MemoryMappedFile"/>.</param>
        /// <param name="offset">The byte at which to start the view.</param>
        /// <param name="size">The size of the view. Specify <c>0</c> (zero) to create a view that
        /// starts at <paramref name="offset"/> and ends approximately at the end of the memory-mapped file.</param>
        /// <returns>A randomly accessible block of memory, as a <see cref="MemoryMappedViewByteBuffer"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="memoryMappedFile"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="size"/> is a negative value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="size"/> is greater than the logical address space.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="size"/> is greater than <see cref="int.MaxValue"/>.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">Access to the memory-mapped file is unauthorized.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        public static MemoryMappedViewByteBuffer CreateViewByteBuffer(this MemoryMappedFile memoryMappedFile, long offset, long size) // J2N TODO: API - deprecate this overload because supporting ByteBuffer larger than int.MaxValue is not possible with Span<T>
        {
            if (size > int.MaxValue)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.size, ExceptionResource.ArgumentOutOfRange_SizeMustBeLessThanOrEqualToInt32MaxValue);

            return CreateViewByteBuffer(memoryMappedFile, offset, size, MemoryMappedFileAccess.ReadWrite, 0, (int)size);
        }

        /// <summary>
        /// Creates a <see cref="MemoryMappedViewByteBuffer"/> that maps to a view of the 
        /// memory-mapped file, and that has the specified <paramref name="offset"/> and <paramref name="size"/>.
        /// </summary>
        /// <param name="memoryMappedFile">This <see cref="MemoryMappedFile"/>.</param>
        /// <param name="offset">The byte at which to start the view.</param>
        /// <param name="size">The size of the view. Specify <c>0</c> (zero) to create a view that
        /// starts at <paramref name="offset"/> and ends approximately at the end of the memory-mapped file.</param>
        /// <param name="access">One of the enumeration values that specifies the type of access allowed to the memory-mapped file. The default is <see cref="MemoryMappedFileAccess.ReadWrite"/>.</param>
        /// <returns>A randomly accessible block of memory, as a <see cref="MemoryMappedViewByteBuffer"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="memoryMappedFile"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> or <paramref name="size"/> is a negative value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="size"/> is greater than the logical address space.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="size"/> is greater than <see cref="int.MaxValue"/>.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException"><paramref name="access"/> is invalid for the memory-mapped file.</exception>
        /// <exception cref="System.IO.IOException">An I/O error occurred.</exception>
        public static MemoryMappedViewByteBuffer CreateViewByteBuffer(this MemoryMappedFile memoryMappedFile, long offset, long size, MemoryMappedFileAccess access) // J2N TODO: API - deprecate this overload because supporting ByteBuffer larger than int.MaxValue is not possible with Span<T>
        {
            if (size > int.MaxValue)
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.size, ExceptionResource.ArgumentOutOfRange_SizeMustBeLessThanOrEqualToInt32MaxValue);

            return CreateViewByteBuffer(memoryMappedFile, offset, size, access, 0, (int)size);
        }

        internal static MemoryMappedViewByteBuffer CreateViewByteBuffer(this MemoryMappedFile memoryMappedFile, long offset, long size, MemoryMappedFileAccess access, int bufferOffset, int bufferSize)
        {
            if (memoryMappedFile is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.memoryMappedFile);

            switch (access)
            {
                case MemoryMappedFileAccess.Read:
                case MemoryMappedFileAccess.ReadExecute:
                    var readOnlyAccessor = new MemoryMappedDirectAccessorReference(new MemoryMappedDirectAccessor(memoryMappedFile.CreateViewAccessor(offset, size, access), offset));
                    return new ReadOnlyMemoryMappedViewByteBuffer(readOnlyAccessor, bufferSize, bufferOffset);
                case MemoryMappedFileAccess.ReadWrite:
                case MemoryMappedFileAccess.ReadWriteExecute:
                case MemoryMappedFileAccess.Write:
                case MemoryMappedFileAccess.CopyOnWrite:
                    var readWriteAccessor = new MemoryMappedDirectAccessorReference(new MemoryMappedDirectAccessor(memoryMappedFile.CreateViewAccessor(offset, size, access), offset));
                    return new ReadWriteMemoryMappedViewByteBuffer(readWriteAccessor, bufferSize, bufferOffset);
                default:
                    throw new ArgumentOutOfRangeException(nameof(access));
            }
        }
    }
}
