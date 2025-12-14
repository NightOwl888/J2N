using System;
using System.Collections.Generic;
using System.Diagnostics;

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

        private static readonly WrappedStringComparer WrappedAroundDefaultComparer = new OrdinalComparer(Comparer<string?>.Default);

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
            public static readonly CultureAwareComparer InvariantCulture = new CultureAwareComparer(StringComparer.InvariantCulture, StringComparison.InvariantCulture);
            public static readonly CultureAwareComparer InvariantCultureIgnoreCase = new CultureAwareComparer(StringComparer.InvariantCultureIgnoreCase, StringComparison.InvariantCultureIgnoreCase);

            private readonly StringComparison _comparisonType;
            internal CultureAwareComparer(IComparer<string?> wrappedComparer, StringComparison comparisonType) : base(wrappedComparer)
            {
                _comparisonType = comparisonType;
            }
            public override int Compare(string? x, string? y) => string.Compare(x, y, _comparisonType);

            int ISpanAlternateComparer<char, string?>.Compare(ReadOnlySpan<char> span, string? other)
            {
                if (other is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537

                return System.MemoryExtensions.CompareTo(span, other, _comparisonType);
            }
            string? ISpanAlternateComparer<char, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();
        }
        
        public static IComparer<string?>? GetStringComparer(object comparer)
        {
            if (ReferenceEquals(comparer, Comparer<string>.Default))
            {
                return WrappedAroundDefaultComparer;
            }

            if (ReferenceEquals(comparer, StringComparer.Ordinal))
            {
                return WrappedAroundStringComparerOrdinal;
            }

            if (ReferenceEquals(comparer, StringComparer.OrdinalIgnoreCase))
            {
                return WrappedAroundStringComparerOrdinalIgnoreCase;
            }

            if (ReferenceEquals(comparer, StringComparer.InvariantCulture))
            {
                return CultureAwareComparer.InvariantCulture;
            }

            if (ReferenceEquals(comparer, StringComparer.InvariantCultureIgnoreCase))
            {
                return CultureAwareComparer.InvariantCultureIgnoreCase;
            }

            // These are not cached, since they read the culture of the current thread upon creation. So,
            // we need to actively check whether the culture matches the one of the current thread rather than
            // checking against a specific instance.
            if (StringComparer.CurrentCulture.Equals(comparer))
            {
                return new CultureAwareComparer((IComparer<string?>)comparer, StringComparison.CurrentCulture);
            }

            if (StringComparer.CurrentCultureIgnoreCase.Equals(comparer))
            {
                return new CultureAwareComparer((IComparer<string?>)comparer, StringComparison.CurrentCultureIgnoreCase);
            }

            return null;
        }
    }
}
