#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
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

using System.Runtime.CompilerServices;


namespace J2N
{
    /// <summary>
    /// Extensions to the <see cref="System.Single"/> class.
    /// </summary>
    public static class SingleExtensions
    {
        private const float NegativeZero = -0.0f;

        /// <summary>
        /// Gets a value indicating whether the current <see cref="float"/> has the value negative zero (<c>-0.0f</c>).
        /// While negative zero is supported by the <see cref="float"/> datatype in .NET, comparisons and string formatting ignore
        /// this feature. This method allows a simple way to check whether the current <see cref="float"/> has the value negative zero.
        /// </summary>
        /// <param name="f">This <see cref="float"/>.</param>
        /// <returns><c>true</c> if the current value represents negative zero; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsNegativeZero(this float f)
        {
            return (f == 0 && BitConversion.SingleToRawInt32Bits(f) == BitConversion.SingleToRawInt32Bits(NegativeZero));
        }
    }
}
