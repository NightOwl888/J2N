// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !FEATURE_STATIC_EXCEPTION_HELPERS

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace J2N
{
    /// <summary>
    /// This class contains methods that are static helper methods on exception classes, like <c>ArgumentNullException.ThrowIfNull</c>,
    /// for older targets that don't have them. These methods are intended to be used via <c>using static J2N.StaticThrowHelper;</c>,
    /// conditionally compiled, where you use i.e. <c>using static System.ArgumentNullException;</c> instead for those platforms
    /// that have them.
    /// </summary>
    internal class StaticThrowHelper
    {
        public static void ThrowIfNegative(int value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value < 0)
                ThrowNegative(value, paramName);
        }

        public static void ThrowIfLessThan(int value, int other, [CallerArgumentExpression(nameof(value))] string? paramName = null)
        {
            if (value.CompareTo(other) < 0)
                ThrowLess(value, other, paramName);
        }

        public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
            {
                ThrowNull(paramName);
            }
        }

        [DoesNotReturn]
        private static void ThrowNegative<T>(T value, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, SR.Format(SR.ArgumentOutOfRange_Generic_MustBeNonNegative, paramName, value));

        [DoesNotReturn]
        internal static void ThrowNull(string? paramName) =>
            throw new ArgumentNullException(paramName);

        [DoesNotReturn]
        private static void ThrowLess<T>(T value, T other, string? paramName) =>
            throw new ArgumentOutOfRangeException(paramName, value, SR.Format(SR.ArgumentOutOfRange_Generic_MustBeGreaterOrEqual, paramName, value, other));
    }
}

#endif
