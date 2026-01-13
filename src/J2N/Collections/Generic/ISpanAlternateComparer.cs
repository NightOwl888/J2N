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

using System;
using System.Collections.Generic;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Implemented by an <see cref="IComparer{T}"/> to support comparing
    /// a <see cref="ReadOnlySpan{T}"/> instance of type <typeparamref name="TAlternateSpan"/>
    /// with a <typeparamref name="T"/> instance.
    /// </summary>
    /// <typeparam name="TAlternateSpan">The type of <see cref="ReadOnlySpan{T}"/> to compare.</typeparam>
    /// <typeparam name="T">The type to compare.</typeparam>
    public interface ISpanAlternateComparer<TAlternateSpan, T>
    {
        /// <summary>
        /// Performs a comparison between a <see cref="ReadOnlySpan{T}"/> instance of type <typeparamref name="TAlternateSpan"/>
        /// and a <typeparamref name="T"/> instance, returning a value indicating whether the <paramref name="alternate"/> is less than, equal to,
        /// or greater than the <paramref name="other"/>.
        /// </summary>
        /// <param name="alternate">The <see cref="ReadOnlySpan{T}"/> instance of type <typeparamref name="TAlternateSpan"/> to compare.</param>
        /// <param name="other">The instance of type <typeparamref name="T"/> to compare.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="alternate"/> and <paramref name="other"/>,
        /// as shown in the following table.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Value</term>
        ///         <term>Meaning</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <description><paramref name="alternate"/> is less than <paramref name="other"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <description><paramref name="alternate"/> is equal to <paramref name="other"/>.</description>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <description><paramref name="alternate"/> is greater than <paramref name="other"/>, or <paramref name="other"/> is <c>null</c>.</description>
        ///     </item>
        /// </list>
        /// <para/>
        /// <b>Notes to Implementers</b>
        /// <para/>
        /// Comparing <c>null</c> with any reference type is allowed and does not generate an exception. A <c>null</c> reference is
        /// considered to be less than any reference that is not <c>null</c>.
        /// </returns>
        int Compare(ReadOnlySpan<TAlternateSpan> alternate, T? other);

        /// <summary>
        /// Creates a <typeparamref name="T"/> that is considered by <see cref="Compare(ReadOnlySpan{TAlternateSpan}, T)"/> to be equal
        /// to the specified <paramref name="alternate"/>.
        /// </summary>
        /// <param name="alternate">The <see cref="ReadOnlySpan{T}"/> instance of type <typeparamref name="TAlternateSpan"/>
        /// for which an equal <typeparamref name="T"/> is required.</param>
        /// <returns>A <typeparamref name="T"/> considered equal to the specified <paramref name="alternate"/>.</returns>
        T Create(ReadOnlySpan<TAlternateSpan> alternate);
    }
}
