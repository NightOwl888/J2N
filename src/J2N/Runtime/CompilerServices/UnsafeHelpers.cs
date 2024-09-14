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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Runtime.CompilerServices
{
    internal static class UnsafeHelpers
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T NullRef<T>()
#if FEATURE_UNSAFE_NULLREF
            => ref Unsafe.NullRef<T>();
#else
        {
            unsafe
            {
                return ref Unsafe.AsRef<T>(null);
            }
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNullRef<T>(ref T source)
#if FEATURE_UNSAFE_ISNULLREF
            => Unsafe.IsNullRef<T>(ref source);
#else
        {
            unsafe
            {
                return Unsafe.AsPointer(ref source) == Unsafe.AsPointer(ref NullRef<T>());
            }
        }
#endif
    }
}
