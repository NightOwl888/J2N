// Source: https://github.com/dotnet/runtime/blob/v10.0.0-rc.1.25451.107/src/libraries/System.Memory/tests/ReadOnlySpan/Comparers.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JCG = J2N.Collections.Generic;
#nullable enable

namespace J2N.SpanTests
{
    public static partial class ReadOnlySpanTests
    {
        private static IEnumerable<IEqualityComparer<T>?> GetDefaultEqualityComparers<T>()
        {
            yield return null;

            yield return JCG.EqualityComparer<T>.Default;

            //yield return JCG.EqualityComparer<T>.Create((i, j) => EqualityComparer<T>.Default.Equals(i, j));
            yield return Create<T>((i, j) => JCG.EqualityComparer<T>.Default.Equals(i, j));

            if (typeof(T) == typeof(string))
            {
                yield return (IEqualityComparer<T>)(object)StringComparer.Ordinal;
            }
        }

        private static IEnumerable<IComparer<T>?> GetDefaultComparers<T>()
        {
            yield return null;

            yield return JCG.Comparer<T>.Default;

            yield return Comparer<T>.Create((i, j) => JCG.Comparer<T>.Default.Compare(i, j));

            if (typeof(T) == typeof(string))
            {
                yield return (IComparer<T>)(object)StringComparer.Ordinal;
            }
        }

        private static IEqualityComparer<T> GetFalseEqualityComparer<T>() =>
            Create<T>((i, j) => false);

        private static IEqualityComparer<T> Create<T>(Func<T, T, bool> equals, Func<T, int>? getHashCode = default)
        {
            Debug.Assert(equals is not null);

            getHashCode ??= _ => throw new NotSupportedException();

            return new DelegateEqualityComparer<T>(equals!, getHashCode);
        }

        internal sealed class DelegateEqualityComparer<T> : EqualityComparer<T>
        {
            private readonly Func<T?, T?, bool> _equals;
            private readonly Func<T, int> _getHashCode;

            public DelegateEqualityComparer(Func<T?, T?, bool> equals, Func<T, int> getHashCode)
            {
                _equals = equals;
                _getHashCode = getHashCode;
            }

            public override bool Equals(T? x, T? y) =>
                _equals(x, y);

#pragma warning disable CS8765 // Nullability of parameter doesn't match overridden member
            public override int GetHashCode([DisallowNull] T obj) =>
                _getHashCode(obj);
#pragma warning restore CS8765 // Nullability of parameter doesn't match overridden member

            public override bool Equals(object? obj) =>
                obj is DelegateEqualityComparer<T> other &&
                _equals == other._equals &&
                _getHashCode == other._getHashCode;

            public override int GetHashCode() =>
                HashCode.Combine(_equals.GetHashCode(), _getHashCode.GetHashCode());
        }
    }
}
