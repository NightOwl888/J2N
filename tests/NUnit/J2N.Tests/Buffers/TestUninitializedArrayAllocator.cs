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
using System.Runtime.CompilerServices;

namespace J2N.Buffers
{
    [TestFixture]
    public class TestUninitializedArrayAllocator
    {
        /**
         * @tests J2N.Buffers.UninitializedArrayAllocator<T>.Default
         */
        [Test]
        public void Test_Default_ReturnsSingleton()
        {
            var allocator1 = UninitializedArrayAllocator<char>.Default;
            var allocator2 = UninitializedArrayAllocator<char>.Default;

            Assert.That(allocator1, Is.SameAs(allocator2));
        }

        /**
         * @tests J2N.Buffers.UninitializedArrayAllocator<T>.Allocate(int)
         */
        [Test]
        public void Test_Allocate_ReturnsExactLength()
        {
            var allocator = UninitializedArrayAllocator<char>.Default;

            char[] array = allocator.Allocate(128);

            Assert.That(array, Is.Not.Null);
            Assert.That(array.Length, Is.EqualTo(128));
        }

        /**
         * @tests J2N.Buffers.UninitializedArrayAllocator<T>.Allocate(int)
         */
        [Test]
        public void Test_Allocate_ZeroLength()
        {
            var allocator = UninitializedArrayAllocator<char>.Default;

            char[] array = allocator.Allocate(0);

            Assert.That(array, Is.Not.Null);
            Assert.That(array.Length, Is.EqualTo(0));
        }

#if FEATURE_GC_ALLOCATEUNINITIALIZEDARRAY
        /**
         * @tests J2N.Buffers.UninitializedArrayAllocator<T>.GuaranteesClearedArrays
         */
        [Test]
        public void Test_GuaranteesClearedArrays_ReturnsFalse_WhenSupported()
        {
            Assert.That(
                UninitializedArrayAllocator<char>.Default.GuaranteesClearedArrays,
                Is.False);
        }
#else
        /**
         * @tests J2N.Buffers.UninitializedArrayAllocator<T>.GuaranteesClearedArrays
         */
        [Test]
        public void Test_GuaranteesClearedArrays_ReturnsTrue_WhenFallbackingToNewArray()
        {
            Assert.That(
                UninitializedArrayAllocator<char>.Default.GuaranteesClearedArrays,
                Is.True);
        }
#endif

        /**
         * @tests J2N.Buffers.UninitializedArrayAllocator<T>.Return(T[])
         */
        [Test]
        public void Test_Return_DoesNotThrow()
        {
            var allocator = UninitializedArrayAllocator<char>.Default;

            char[] array = allocator.Allocate(64);

            Assert.DoesNotThrow(() => allocator.Return(array));
        }

        /**
         * @tests J2N.Buffers.UninitializedArrayAllocator<T>.Allocate(int)
         */
        [Test]
        public void Test_Allocate_NegativeLength_Throws()
        {
            var allocator = UninitializedArrayAllocator<char>.Default;

            Assert.Throws<OverflowException>(() =>
            {
                allocator.Allocate(-1);
            });
        }

        /**
         * @tests J2N.Buffers.UninitializedArrayAllocator<T>.Allocate(int)
         */
        [Test]
        public void Test_Allocate_MultipleIndependentArrays()
        {
            var allocator = UninitializedArrayAllocator<char>.Default;

            char[] array1 = allocator.Allocate(32);
            char[] array2 = allocator.Allocate(32);

            Assert.That(RuntimeHelpers.GetHashCode(array1),
                Is.Not.EqualTo(RuntimeHelpers.GetHashCode(array2)));
        }
    }
}
