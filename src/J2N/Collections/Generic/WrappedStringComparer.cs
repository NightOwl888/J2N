#region Copyright 2019-2025 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// <see cref="WrappedStringComparer"/> is the comparer used by default with the SortedDictionary&lt;string,...&gt;
    /// and SortedSet&lt;string&gt;. We use <see cref="WrappedStringComparer"/> as default comparer to add support for
    /// J2N alternate lookup and other interfaces to the BCL StringComparer.
    /// </summary>
    internal abstract class WrappedStringComparer : IComparer<string?>, IInternalStringComparer
    {
        // SortedDictionary<...>.Comparer and similar methods need to return the original IComparer
        // that was passed in to the ctor. The caller chooses one of these singletons so that the
        // GetUnderlyingComparer method can return the correct value.

        private static readonly WrappedStringComparer WrappedAroundStringComparerOrdinal = new OrdinalComparer(StringComparer.Ordinal);

        private static readonly WrappedStringComparer WrappedAroundStringComparerOrdinalIgnoreCase = new OrdinalIgnoreCaseComparer(StringComparer.OrdinalIgnoreCase);

        private readonly IComparer<string?> _underlyingComparer;

        private WrappedStringComparer(IComparer<string?> underlyingComparer)
        {
            Debug.Assert(underlyingComparer != null);
            _underlyingComparer = underlyingComparer!;
        }

        public abstract int Compare(string? x, string? y);

        // Gets the comparer that should be returned back to the caller when querying the
        // ICollection.Comparer property. Also used for serialization purposes.
        public virtual IComparer<string?> GetUnderlyingComparer() => _underlyingComparer;

        // Equality for the comparer itself.
        public override bool Equals(object? obj)
        {
            // Commonly, both comparers will be the default comparer (and reference-equal). Avoid a virtual method call to Equals() in that case.
            return _underlyingComparer == obj || _underlyingComparer.Equals(obj);
        }

        // Equality for the comparer itself.
        public override int GetHashCode() => _underlyingComparer.GetHashCode();

        private sealed class OrdinalComparer : WrappedStringComparer, ISpanAlternateComparer<char, string?>
        {
            internal OrdinalComparer(IComparer<string?> wrappedComparer) : base(wrappedComparer)
            {
            }
            public override int Compare(string? x, string? y) => string.CompareOrdinal(x, y);

            int ISpanAlternateComparer<char, string?>.Compare(ReadOnlySpan<char> span, string? other)
            {
                if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537

                return System.MemoryExtensions.CompareTo(span, other, StringComparison.Ordinal);
            }

            string? ISpanAlternateComparer<char, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();
        }

        private sealed class OrdinalIgnoreCaseComparer : WrappedStringComparer, ISpanAlternateComparer<char, string?>
        {
            internal OrdinalIgnoreCaseComparer(IComparer<string?> wrappedComparer) : base(wrappedComparer)
            {
            }
            public override int Compare(string? x, string? y) => string.Compare(x, y, StringComparison.OrdinalIgnoreCase);

            int ISpanAlternateComparer<char, string?>.Compare(ReadOnlySpan<char> span, string? other)
            {
                if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537

                return System.MemoryExtensions.CompareTo(span, other, StringComparison.OrdinalIgnoreCase);
            }
            string? ISpanAlternateComparer<char, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();
        }

        private sealed class CultureAwareComparer : WrappedStringComparer, ISpanAlternateComparer<char, string?>
        {
            public static readonly CultureAwareComparer InvariantCulture = new CultureAwareComparer(StringComparer.InvariantCulture);
            public static readonly CultureAwareComparer InvariantCultureIgnoreCase = new CultureAwareComparer(StringComparer.InvariantCultureIgnoreCase);


            internal CultureAwareComparer(IComparer<string?> wrappedComparer) : base(wrappedComparer)
            {
            }

            public override int Compare(string? x, string? y) => _underlyingComparer.Compare(x, y);

            int ISpanAlternateComparer<char, string?>.Compare(ReadOnlySpan<char> span, string? other)
            {
                if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537

#if FEATURE_STRINGCOMPARER_ISWELLKNOWNCULTUREAWARECOMPARER
                if (_underlyingComparer is IEqualityComparer<string?> ec &&
                    StringComparer.IsWellKnownCultureAwareComparer(ec, out CompareInfo? compareInfo, out CompareOptions compareOptions))
                {
                    return compareInfo.Compare(span, other, compareOptions);
                }
#endif
                // Legacy - no choice but to allocate
                return _underlyingComparer.Compare(span.ToString(), other);
            }

            string? ISpanAlternateComparer<char, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();
        }
        
        public static IComparer<string?>? GetStringComparer(object comparer)
        {
            if (ReferenceEquals(comparer, StringComparer.Ordinal))
            {
                return WrappedAroundStringComparerOrdinal;
            }

            if (ReferenceEquals(comparer, StringComparer.OrdinalIgnoreCase))
            {
                return WrappedAroundStringComparerOrdinalIgnoreCase;
            }

            if (StringComparerDescriptor.TryDescribe(comparer, out StringComparerDescriptor descriptor) &&
                TryGetStringComparer(descriptor, out IComparer<string?>? stringComparer))
            {
                return stringComparer;
            }

            return null;
        }

        public static bool TryGetStringComparer(in StringComparerDescriptor descriptor, [MaybeNullWhen(false)] out IComparer<string?>? comparer)
        {
            if (descriptor.Type == StringComparerDescriptor.Classification.Ordinal)
            {
                comparer = WrappedAroundStringComparerOrdinal;
                return true;
            }

            if (descriptor.Type == StringComparerDescriptor.Classification.OrdinalIgnoreCase)
            {
                comparer = WrappedAroundStringComparerOrdinalIgnoreCase;
                return true;
            }

            if (descriptor.Type == StringComparerDescriptor.Classification.InvariantCulture)
            {
                comparer = CultureAwareComparer.InvariantCulture;
                return true;
            }

            if (descriptor.Type == StringComparerDescriptor.Classification.InvariantCultureIgnoreCase)
            {
                comparer = CultureAwareComparer.InvariantCultureIgnoreCase;
                return true;
            }

            if (descriptor.TryCreateStringComparer(out StringComparer? stringComparer))
            {
                // Otherwise, this is culture-aware
                if (descriptor.Type == StringComparerDescriptor.Classification.CurrentCultureIgnoreCase)
                {
                    comparer = new CultureAwareComparer(stringComparer!);
                    return true;
                }

                if (descriptor.Type == StringComparerDescriptor.Classification.CurrentCulture)
                {
                    comparer = new CultureAwareComparer(stringComparer!);
                    return true;
                }
            }

            comparer = null;
            return false;
        }
    }
}
