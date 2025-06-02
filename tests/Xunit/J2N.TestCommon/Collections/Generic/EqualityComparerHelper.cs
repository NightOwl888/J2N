// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using static J2N.ThrowHelper;

#nullable enable

namespace J2N.Collections.Tests
{
    /// <summary>
    /// Methods from .NET's <see cref="EqualityComparer{T}"/> class that are not available in all J2N target frameworks.
    /// </summary>
    /// <typeparam name="T">The type of item to compare.</typeparam>
    public static class EqualityComparerHelper<T>
    {
        /// <summary>
        /// Creates an <see cref="IEqualityComparer{T}"/> by using the specified delegates as the implementation of the comparer's
        /// <see cref="IEqualityComparer{T}.Equals(T,T)"/> and <see cref="IEqualityComparer{T}.GetHashCode(T)"/> methods.
        /// </summary>
        /// <param name="equals">The delegate to use to implement the <see cref="IEqualityComparer{T}.Equals(T,T)"/> method.</param>
        /// <param name="getHashCode">
        /// The delegate to use to implement the <see cref="IEqualityComparer{T}.GetHashCode(T)"/> method.
        /// If no delegate is supplied, calls to the resulting comparer's <see cref="IEqualityComparer{T}.GetHashCode(T)"/>
        /// will throw <see cref="NotSupportedException"/>.
        /// </param>
        /// <returns>The new comparer.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="equals"/> delegate was null.</exception>
        public static IEqualityComparer<T> Create(Func<T?, T?, bool> equals, Func<T, int>? getHashCode = null)
        {
            if (equals is null)
            {
                ThrowArgumentNullException(ExceptionArgument.equals);
            }

            getHashCode ??= _ => throw new NotSupportedException();

            return new DelegateEqualityComparer(equals, getHashCode);
        }

        internal sealed class DelegateEqualityComparer : IEqualityComparer<T>
        {
            private readonly Func<T?, T?, bool> _equals;
            private readonly Func<T, int> _getHashCode;

            public DelegateEqualityComparer(Func<T?, T?, bool> equals, Func<T, int> getHashCode)
            {
                _equals = equals;
                _getHashCode = getHashCode;
            }

            public bool Equals(T? x, T? y) =>
                _equals(x, y);

            public int GetHashCode(T obj) =>
                _getHashCode(obj);

            public override bool Equals(object? obj) =>
                obj is DelegateEqualityComparer other &&
                _equals == other._equals &&
                _getHashCode == other._getHashCode;

            public override int GetHashCode() =>
                //HashCode.Combine(_equals.GetHashCode(), _getHashCode.GetHashCode());
                _equals.GetHashCode() ^ _getHashCode.GetHashCode();
        }
    }
}
