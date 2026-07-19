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

using NUnit.Framework;

namespace J2N.Buffers
{

    [TestFixture]
    public class TestIArrayAllocatorContract
    {
        private static readonly IArrayAllocator<char>[] allocators =
        {
            ArrayAllocator<char>.Default,
            PooledArrayAllocator<char>.Cleared,
            PooledArrayAllocator<char>.Uncleared,
#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
            UninitializedArrayAllocator<char>.Default,
#endif
        };

        /**
         * @tests J2N.Buffers.IArrayAllocator<T>.Allocate(int)
         */
        [Test]
        public void Test_AllAllocators_ReturnAtLeastRequestedLength()
        {
            foreach (IArrayAllocator<char> allocator in allocators)
            {
                for (int length = 0; length <= 4096; length++)
                {
                    char[] array = allocator.Allocate(length);

                    try
                    {
                        Assert.That(array.Length,
                            Is.GreaterThanOrEqualTo(length));
                    }
                    finally
                    {
                        allocator.Return(array);
                    }
                }
            }
        }

        /**
         * @tests J2N.Buffers.IArrayAllocator<T>.Allocate(int)
         */
        [Test]
        public void Test_AllAllocators_AllowReadWriteOperations()
        {
            foreach (IArrayAllocator<char> allocator in allocators)
            {
                char[] array = allocator.Allocate(256);

                try
                {
                    for (int i = 0; i < 256; i++)
                    {
                        array[i] = (char)(i % 128);
                    }

                    for (int i = 0; i < 256; i++)
                    {
                        Assert.That(array[i], Is.EqualTo((char)(i % 128)));
                    }
                }
                finally
                {
                    allocator.Return(array);
                }
            }
        }

        /**
         * @tests J2N.Buffers.IArrayAllocator<T>.Return(T[])
         */
        [Test]
        public void Test_AllAllocators_SupportMultipleRentReturnCycles()
        {
            foreach (IArrayAllocator<char> allocator in allocators)
            {
                for (int cycle = 0; cycle < 1000; cycle++)
                {
                    char[] array = allocator.Allocate(128);

                    array[0] = 'Z';

                    allocator.Return(array);
                }
            }
        }
    }
}
