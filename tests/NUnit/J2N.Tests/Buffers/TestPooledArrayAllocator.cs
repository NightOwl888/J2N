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
using System;
using System.Threading.Tasks;

namespace J2N.Buffers
{
    [TestFixture]
    public class TestPooledArrayAllocator
    {
        // --------------------------------------------------------------------
        // Uncleared
        // --------------------------------------------------------------------

        [Test]
        public void Test_Uncleared_ReturnsSingleton()
        {
            var allocator1 = PooledArrayAllocator<char>.Uncleared;
            var allocator2 = PooledArrayAllocator<char>.Uncleared;

            Assert.That(allocator1, Is.SameAs(allocator2));
        }

        [Test]
        public void Test_Uncleared_GuaranteesClearedArrays_ReturnsFalse()
        {
            Assert.That(PooledArrayAllocator<char>.Uncleared.GuaranteesClearedArrays, Is.False);
        }

        [Test]
        public void Test_Uncleared_Allocate_ReturnsAtLeastRequestedLength()
        {
            var allocator = PooledArrayAllocator<char>.Uncleared;

            for (int i = 0; i < 4096; i++)
            {
                char[] array = allocator.Allocate(i);

                Assert.That(array, Is.Not.Null);
                Assert.That(array.Length, Is.GreaterThanOrEqualTo(i));

                allocator.Return(array);
            }
        }

        [Test]
        public void Test_Uncleared_Allocate_ZeroLength()
        {
            var allocator = PooledArrayAllocator<char>.Uncleared;

            char[] array = allocator.Allocate(0);

            Assert.That(array, Is.Not.Null);
            Assert.That(array.Length, Is.GreaterThanOrEqualTo(0));

            allocator.Return(array);
        }

        [Test]
        public void Test_Uncleared_Return_DoesNotThrow()
        {
            var allocator = PooledArrayAllocator<char>.Uncleared;

            char[] array = allocator.Allocate(32);

            Assert.DoesNotThrow(() => allocator.Return(array));
        }

        [Test]
        public void Test_Uncleared_Allocate_NegativeLength_Throws()
        {
            var allocator = PooledArrayAllocator<char>.Uncleared;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                allocator.Allocate(-1);
            });
        }

        [Test]
        public void Test_Uncleared_Allocate_MultipleConcurrentRentReturnOperations()
        {
            var allocator = PooledArrayAllocator<char>.Uncleared;

            Parallel.For(0, Environment.ProcessorCount * 8, _ =>
            {
                for (int i = 1; i <= 1024; i++)
                {
                    char[] array = allocator.Allocate(i);

                    Assert.That(array.Length, Is.GreaterThanOrEqualTo(i));

                    array[0] = 'A';

                    allocator.Return(array);
                }
            });
        }

        [Test]
        public void Test_Uncleared_Allocate_LargeBuffers()
        {
            var allocator = PooledArrayAllocator<char>.Uncleared;

            int[] sizes =
            {
                1024,
                4096,
                16384,
                65536,
                131072
            };

            foreach (int size in sizes)
            {
                char[] array = allocator.Allocate(size);

                Assert.That(array.Length, Is.GreaterThanOrEqualTo(size));

                allocator.Return(array);
            }
        }

        [Test]
        public void Test_Uncleared_Return_Null_Throws()
        {
            var allocator = PooledArrayAllocator<char>.Uncleared;

            Assert.Throws<ArgumentNullException>(() =>
            {
                allocator.Return(null!);
            });
        }

        // --------------------------------------------------------------------
        // Cleared
        // --------------------------------------------------------------------

        [Test]
        public void Test_Cleared_ReturnsSingleton()
        {
            var allocator1 = PooledArrayAllocator<char>.Cleared;
            var allocator2 = PooledArrayAllocator<char>.Cleared;

            Assert.That(allocator1, Is.SameAs(allocator2));
        }

        [Test]
        public void Test_Cleared_GuaranteesClearedArrays_ReturnsTrue()
        {
            Assert.That(PooledArrayAllocator<char>.Cleared.GuaranteesClearedArrays, Is.True);
        }

        [Test]
        public void Test_Cleared_Allocate_ReturnsAtLeastRequestedLength()
        {
            var allocator = PooledArrayAllocator<char>.Cleared;

            for (int i = 0; i < 4096; i++)
            {
                char[] array = allocator.Allocate(i);

                Assert.That(array, Is.Not.Null);
                Assert.That(array.Length, Is.GreaterThanOrEqualTo(i));

                allocator.Return(array);
            }
        }

        [Test]
        public void Test_Cleared_Allocate_ZeroLength()
        {
            var allocator = PooledArrayAllocator<char>.Cleared;

            char[] array = allocator.Allocate(0);

            Assert.That(array, Is.Not.Null);
            Assert.That(array.Length, Is.GreaterThanOrEqualTo(0));

            allocator.Return(array);
        }

        [Test]
        public void Test_Cleared_Return_DoesNotThrow()
        {
            var allocator = PooledArrayAllocator<char>.Cleared;

            char[] array = allocator.Allocate(32);

            Assert.DoesNotThrow(() => allocator.Return(array));
        }

        [Test]
        public void Test_Cleared_Allocate_NegativeLength_Throws()
        {
            var allocator = PooledArrayAllocator<char>.Cleared;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                allocator.Allocate(-1);
            });
        }

        [Test]
        public void Test_Cleared_ReturnedArrayIsClearedBeforeReuse()
        {
            var allocator = PooledArrayAllocator<char>.Cleared;

            char[] array1 = allocator.Allocate(128);

            for (int i = 0; i < array1.Length; i++)
            {
                array1[i] = 'X';
            }

            allocator.Return(array1);

            bool foundClearedArray = false;

            // Pooling is nondeterministic, so try several times.
            for (int attempt = 0; attempt < 64; attempt++)
            {
                char[] array2 = allocator.Allocate(128);

                try
                {
                    bool allCleared = true;

                    for (int i = 0; i < 128; i++)
                    {
                        if (array2[i] != '\0')
                        {
                            allCleared = false;
                            break;
                        }
                    }

                    if (allCleared)
                    {
                        foundClearedArray = true;
                        break;
                    }
                }
                finally
                {
                    allocator.Return(array2);
                }
            }

            Assert.That(foundClearedArray, Is.True,
                "Expected at least one reused array instance to be cleared.");
        }

        [Test]
        public void Test_Cleared_Allocate_MultipleConcurrentRentReturnOperations()
        {
            var allocator = PooledArrayAllocator<char>.Cleared;

            Parallel.For(0, Environment.ProcessorCount * 8, _ =>
            {
                for (int i = 1; i <= 1024; i++)
                {
                    char[] array = allocator.Allocate(i);

                    Assert.That(array.Length, Is.GreaterThanOrEqualTo(i));

                    array[0] = 'A';

                    allocator.Return(array);
                }
            });
        }

        [Test]
        public void Test_Cleared_Allocate_LargeBuffers()
        {
            var allocator = PooledArrayAllocator<char>.Cleared;

            int[] sizes =
            {
                1024,
                4096,
                16384,
                65536,
                131072
            };

            foreach (int size in sizes)
            {
                char[] array = allocator.Allocate(size);

                Assert.That(array.Length, Is.GreaterThanOrEqualTo(size));

                allocator.Return(array);
            }
        }

        [Test]
        public void Test_Cleared_Return_Null_Throws()
        {
            var allocator = PooledArrayAllocator<char>.Cleared;

            Assert.Throws<ArgumentNullException>(() =>
            {
                allocator.Return(null!);
            });
        }
    }
}