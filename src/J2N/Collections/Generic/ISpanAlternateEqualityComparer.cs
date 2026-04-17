// Based on: https://github.com/dotnet/runtime/blob/v10.0.1/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/IAlternateEqualityComparer.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Implemented by an <see cref="IEqualityComparer{T}"/> to support comparing
    /// a <see cref="ReadOnlySpan{T}"/> instance of type <typeparamref name="TAlternateSpan"/>
    /// with a <typeparamref name="T"/> instance.
    /// </summary>
    /// <typeparam name="TAlternateSpan">The type of <see cref="ReadOnlySpan{T}"/> to compare.</typeparam>
    /// <typeparam name="T">The type to compare.</typeparam>
    public interface ISpanAlternateEqualityComparer<TAlternateSpan, T>
    {
        /// <summary>Determines whether the specified <paramref name="alternate"/> equals the specified <paramref name="other"/>.</summary>
        /// <param name="alternate">The <see cref="ReadOnlySpan{T}"/> instance of type <typeparamref name="TAlternateSpan"/> to compare.</param>
        /// <param name="other">The instance of type <typeparamref name="T"/> to compare.</param>
        /// <returns><see langword="true"/> if the specified instances are equal; otherwise, <see langword="false"/>.</returns>
        bool Equals(ReadOnlySpan<TAlternateSpan> alternate, T other);

        /// <summary>Returns a hash code for the specified alternate instance.</summary>
        /// <param name="alternate">The <see cref="ReadOnlySpan{T}"/> instance of type <typeparamref name="TAlternateSpan"/>
        /// for which to get a hash code.</param>
        /// <returns>A hash code for the specified instance.</returns>
        /// <remarks>
        /// This interface is intended to be implemented on a type that also implements <see cref="IEqualityComparer{T}"/>.
        /// The result of this method should return the same hash code as would invoking the <see cref="IEqualityComparer{T}.GetHashCode"/>
        /// method on any <typeparamref name="T"/> for which <see cref="Equals(ReadOnlySpan{TAlternateSpan}, T)"/>
        /// returns <see langword="true"/>.
        /// </remarks>
        int GetHashCode(ReadOnlySpan<TAlternateSpan> alternate);

        /// <summary>
        /// Creates a <typeparamref name="T"/> that is considered by <see cref="Equals(ReadOnlySpan{TAlternateSpan}, T)"/> to be equal
        /// to the specified <paramref name="alternate"/>.
        /// </summary>
        /// <param name="alternate">The <see cref="ReadOnlySpan{T}"/> instance of type <typeparamref name="TAlternateSpan"/>
        /// for which an equal <typeparamref name="T"/> is required.</param>
        /// <returns>A <typeparamref name="T"/> considered equal to the specified <paramref name="alternate"/>.</returns>
        T Create(ReadOnlySpan<TAlternateSpan> alternate);
    }
}
