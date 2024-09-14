#region Copyright 2019-2024 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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
using System.Buffers;
using System.Security.Cryptography;

namespace J2N.Security.Cryptography
{
    internal static class RandomHelpers
    {
        // Mimics Interop.GetRandomBytes(byte* buffer, int size) using RandomNumberGenerator
        internal static unsafe void GetRandomBytes(byte* buffer, int size)
        {
#if FEATURE_RANDOMNUMBERGENERATOR_FILL_SPAN
            Span<byte> span = new Span<byte>(buffer, size);
            RandomNumberGenerator.Fill(span);
#elif FEATURE_RANDOMNUMBERGENERATOR_GETBYTES_OFFSET_COUNT
            byte[]? rentedArray = null;

            try
            {
                // Rent a buffer from the ArrayPool
                rentedArray = ArrayPool<byte>.Shared.Rent(size);

                // Fill the rented buffer with random bytes
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(rentedArray, 0, (int)size);
                }

                // Efficiently copy the random bytes to the provided buffer
                fixed (byte* source = rentedArray)
                {
                    Buffer.MemoryCopy(source, buffer, size, size);
                }
            }
            finally
            {
                if (rentedArray != null)
                {
                    // Return the array to the pool
                    ArrayPool<byte>.Shared.Return(rentedArray);
                }
            }
#else
            byte[] tempArray = new byte[size];

            // Fill the tempArray with random bytes
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tempArray);
            }

            // Manually copy each byte to the buffer
            for (int i = 0; i < size; i++)
            {
                buffer[i] = tempArray[i];
            }
#endif
        }
    }
}
