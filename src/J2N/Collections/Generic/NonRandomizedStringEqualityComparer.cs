// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace J2N.Collections.Generic
{
    /// <summary>
    /// NonRandomizedStringEqualityComparer is the comparer used by default with the Dictionary&lt;string,...&gt;
    /// We use NonRandomizedStringEqualityComparer as default comparer as it doesnt use the randomized string hashing which
    /// keeps the performance not affected till we hit collision threshold and then we switch to the comparer which is using
    /// randomized string hashing.
    /// </summary>
    // J2N: Although, this is public in .NET, we don't need to maintain compatibility with .NET Core 2.0 so this is internal.
    // This has never been exposed in the J2N serialization blob.
    internal class NonRandomizedStringEqualityComparer : IEqualityComparer<string?>, IInternalStringEqualityComparer, ISerializable
    {
        // Dictionary<...>.Comparer and similar methods need to return the original IEqualityComparer
        // that was passed in to the ctor. The caller chooses one of these singletons so that the
        // GetUnderlyingEqualityComparer method can return the correct value.

        private static readonly NonRandomizedStringEqualityComparer WrappedAroundDefaultComparer = new OrdinalComparer(EqualityComparer<string?>.Default);

        private static readonly NonRandomizedStringEqualityComparer WrappedAroundStringComparerOrdinal = new OrdinalComparer(StringComparer.Ordinal);

        private static readonly NonRandomizedStringEqualityComparer WrappedAroundStringComparerOrdinalIgnoreCase = new OrdinalIgnoreCaseComparer(StringComparer.OrdinalIgnoreCase);

        private readonly IEqualityComparer<string?> _underlyingComparer;

        private NonRandomizedStringEqualityComparer(IEqualityComparer<string?> underlyingComparer)
        {
            Debug.Assert(underlyingComparer != null);
            _underlyingComparer = underlyingComparer!;
        }

        // This is used by the serialization engine.
        [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected NonRandomizedStringEqualityComparer(SerializationInfo information, StreamingContext context)
            : this(EqualityComparer<string?>.Default)
        {
        }

        public virtual bool Equals(string? x, string? y)
        {
            // This instance may have been deserialized into a class that doesn't guarantee
            // these parameters are non-null. Can't short-circuit the null checks.

            return string.Equals(x, y);
        }

        public virtual int GetHashCode(string? obj)
        {
            // This instance may have been deserialized into a class that doesn't guarantee
            // these parameters are non-null. Can't short-circuit the null checks.

            //return obj?.GetNonRandomizedHashCode() ?? 0;
            if (obj is null)
                return 0;
            return StringHelper.GetNonRandomizedHashCode(obj);
        }

        internal virtual RandomizedStringEqualityComparer GetRandomizedEqualityComparer()
        {
            return RandomizedStringEqualityComparer.Create(_underlyingComparer, ignoreCase: false);
        }

        // Gets the comparer that should be returned back to the caller when querying the
        // ICollection.Comparer property. Also used for serialization purposes.
        public virtual IEqualityComparer<string?> GetUnderlyingEqualityComparer() => _underlyingComparer;

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // We are doing this to stay compatible with .NET Framework.
            // Our own collection types will never call this (since this type is a wrapper),
            // but perhaps third-party collection types could try serializing an instance
            // of this.
            //System.Collections.Generic.EqualityComparer<string>.

            //info.SetType(typeof(System.Collections.Generic.GenericEqualityComparer<string>));
            info.SetType(typeof(EqualityComparer<string>));
        }

        private sealed class OrdinalComparer : NonRandomizedStringEqualityComparer, ISpanAlternateEqualityComparer<char, string?>
#if FEATURE_IALTERNATEEQUALITYCOMPARER
            , IAlternateEqualityComparer<ReadOnlySpan<char>, string?>
#endif
        {
            internal OrdinalComparer(IEqualityComparer<string?> wrappedComparer) : base(wrappedComparer)
            {
            }

            public override bool Equals(string? x, string? y) => string.Equals(x, y);

            public override int GetHashCode(string? obj)
            {
                //Debug.Assert(obj != null, "This implementation is only called from first-party collection types that guarantee non-null parameters.");
                if (obj is null)
                    return 0;

#if NET || NETFRAMEWORK
                return StringHelper.GetNonRandomizedHashCode(obj);
#else
                // J2N: There is no guarantee that any future .NET platform will use null-terminated
                // strings, so we call the span overload to be safe.
                return StringHelper.GetNonRandomizedHashCode(obj.AsSpan());
#endif
            }

#if FEATURE_IALTERNATEEQUALITYCOMPARER
            int IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.GetHashCode(ReadOnlySpan<char> span) =>
                StringHelper.GetNonRandomizedHashCode(span);

            bool IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Equals(ReadOnlySpan<char> span, string? target)
            {
                // See explanation in StringEqualityComparer.Equals.
                if (span.IsEmpty && target is null)
                {
                    return false;
                }

                return span.SequenceEqual(target);
            }

            string IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();
#endif

            int ISpanAlternateEqualityComparer<char, string?>.GetHashCode(ReadOnlySpan<char> span) =>
                StringHelper.GetNonRandomizedHashCode(span);

            bool ISpanAlternateEqualityComparer<char, string?>.Equals(ReadOnlySpan<char> span, string? target)
            {
                if (span.IsEmpty && target is null)
                {
                    return false;
                }

                return span.SequenceEqual(target);
            }

            string ISpanAlternateEqualityComparer<char, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();
        }

        private sealed class OrdinalIgnoreCaseComparer : NonRandomizedStringEqualityComparer, ISpanAlternateEqualityComparer<char, string?>
#if FEATURE_IALTERNATEEQUALITYCOMPARER
           , IAlternateEqualityComparer<ReadOnlySpan<char>, string?>
#endif
        {
            internal OrdinalIgnoreCaseComparer(IEqualityComparer<string?> wrappedComparer)
                : base(wrappedComparer)
            {
            }

            public override bool Equals(string? x, string? y) => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

            public override int GetHashCode(string? obj)
            {
                //Debug.Assert(obj != null, "This implementation is only called from first-party collection types that guarantee non-null parameters.");

                if (obj is null)
                    return 0;

#if NET || NETFRAMEWORK
                return StringHelper.GetNonRandomizedHashCodeOrdinalIgnoreCase(obj);
#else
                // J2N: There is no guarantee that any future .NET platform will use null-terminated
                // strings, so we call the span overload to be safe.
                return StringHelper.GetNonRandomizedHashCodeOrdinalIgnoreCase(obj.AsSpan());
#endif
            }
#if FEATURE_IALTERNATEEQUALITYCOMPARER
            int IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.GetHashCode(ReadOnlySpan<char> span) =>
                StringHelper.GetNonRandomizedHashCodeOrdinalIgnoreCase(span);

            bool IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Equals(ReadOnlySpan<char> span, string? target)
            {
                // See explanation in StringEqualityComparer.Equals.
                if (span.IsEmpty && target is null)
                {
                    return false;
                }

                return System.MemoryExtensions.Equals(span, target, StringComparison.OrdinalIgnoreCase);
            }

            string IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();
#endif

            int ISpanAlternateEqualityComparer<char, string?>.GetHashCode(ReadOnlySpan<char> span) =>
                StringHelper.GetNonRandomizedHashCodeOrdinalIgnoreCase(span);

            bool ISpanAlternateEqualityComparer<char, string?>.Equals(ReadOnlySpan<char> span, string? target)
            {
                if (span.IsEmpty && target is null)
                {
                    return false;
                }

                return System.MemoryExtensions.Equals(span, target, StringComparison.OrdinalIgnoreCase);
            }

            string ISpanAlternateEqualityComparer<char, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();

            internal override RandomizedStringEqualityComparer GetRandomizedEqualityComparer()
            {
                return RandomizedStringEqualityComparer.Create(_underlyingComparer, ignoreCase: true);
            }
        }

        // NOTE: We don't implement NonRandomizedStringEqualityComparer here because there is no corresponding RandomizedStringEqualityComparer to switch to.
        private sealed class CultureAwareComparer : IEqualityComparer<string?>, IInternalStringEqualityComparer, ISpanAlternateEqualityComparer<char, string?>
#if FEATURE_IALTERNATEEQUALITYCOMPARER
           , IAlternateEqualityComparer<ReadOnlySpan<char>, string?>
#endif
        {
            public static readonly CultureAwareComparer InvariantCulture = new CultureAwareComparer(StringComparer.InvariantCulture);
            public static readonly CultureAwareComparer InvariantCultureIgnoreCase = new CultureAwareComparer(StringComparer.InvariantCultureIgnoreCase);

            private readonly IEqualityComparer<string?> _underlyingComparer;
#if FEATURE_IALTERNATEEQUALITYCOMPARER
            private readonly IAlternateEqualityComparer<ReadOnlySpan<char>, string?>? _alternateComparer;
#endif
            internal CultureAwareComparer(IEqualityComparer<string?> wrappedComparer)
            {
                _underlyingComparer = wrappedComparer;
#if FEATURE_IALTERNATEEQUALITYCOMPARER
                _alternateComparer = wrappedComparer as IAlternateEqualityComparer<ReadOnlySpan<char>, string?>;
#endif
            }

            public IEqualityComparer<string?> GetUnderlyingEqualityComparer() => _underlyingComparer;

            public bool Equals(string? x, string? y) => _underlyingComparer.Equals(x, y);

            public int GetHashCode(string? obj)
            {
                if (obj is null)
                    return 0;

                // J2N: Ensure this is in sync across all target frameworks for a given comparison type.
                // If we call the BCL method for one GetHashCode() method, we should do it for all in this class.
                // The BCL seed will always be the same for a given process, so the hash codes will be consistent,
                // however, the J2N seed will likely be different.

                return _underlyingComparer.GetHashCode(obj);
            }

#if FEATURE_IALTERNATEEQUALITYCOMPARER

            int IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.GetHashCode(ReadOnlySpan<char> span)
            {
                // J2N: Ensure this is in sync across all target frameworks for a given comparison type.
                // If we call the BCL method for one GetHashCode() method, we should do it for all in this class.
                // The BCL seed will always be the same for a given process, so the hash codes will be consistent,
                // however, the J2N seed will likely be different.

                if (_alternateComparer is not null)
                    return _alternateComparer.GetHashCode(span);

                // Legacy - no choice but to allocate (this will never be hit)
                return _underlyingComparer.GetHashCode(span.ToString());
            }

            bool IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Equals(ReadOnlySpan<char> span, string? target)
            {
                // See explanation in StringEqualityComparer.Equals.
                if (span.IsEmpty && target is null)
                {
                    return false;
                }

                if (_alternateComparer is not null)
                    return _alternateComparer.Equals(span, target);

                // Legacy - no choice but to allocate (this will never be hit)
                return _underlyingComparer.Equals(span.ToString(), target);
            }

            string? IAlternateEqualityComparer<ReadOnlySpan<char>, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();
#endif

            int ISpanAlternateEqualityComparer<char, string?>.GetHashCode(ReadOnlySpan<char> span)
            {
                // J2N: Ensure this is in sync across all target frameworks for a given comparison type.
                // If we call the BCL method for one GetHashCode() method, we should do it for all in this class.
                // The BCL seed will always be the same for a given process, so the hash codes will be consistent,
                // however, the J2N seed will likely be different.
#if FEATURE_IALTERNATEEQUALITYCOMPARER
                if (_alternateComparer is not null)
                    return _alternateComparer.GetHashCode(span);
#endif

#if FEATURE_STRINGCOMPARER_ISWELLKNOWNCULTUREAWARECOMPARER
                if (_underlyingComparer is IEqualityComparer<string?> ec &&
                    StringComparer.IsWellKnownCultureAwareComparer(ec, out CompareInfo? compareInfo, out CompareOptions compareOptions))
                {
                    return compareInfo.GetHashCode(span, compareOptions);
                }
#endif

                // Legacy - no choice but to allocate
                return _underlyingComparer.GetHashCode(span.ToString());
            }

            bool ISpanAlternateEqualityComparer<char, string?>.Equals(ReadOnlySpan<char> span, string? target)
            {
                // See explanation in StringEqualityComparer.Equals.
                if (span.IsEmpty && target is null)
                {
                    return false;
                }

#if FEATURE_IALTERNATEEQUALITYCOMPARER
                if (_alternateComparer is not null)
                    return _alternateComparer.Equals(span, target);
#endif

#if FEATURE_STRINGCOMPARER_ISWELLKNOWNCULTUREAWARECOMPARER
                if (_underlyingComparer is IEqualityComparer<string?> ec &&
                    StringComparer.IsWellKnownCultureAwareComparer(ec, out CompareInfo? compareInfo, out CompareOptions compareOptions))
                {
                    // NOTE: For culture-aware comparisons, Compare() == 0 is the only
                    // correct definition of equality. There is no Equals-only alternative.
                    return compareInfo.Compare(span, target, compareOptions) == 0;
                }
#endif

                // Legacy - no choice but to allocate one side. This is cheaper than
                // calling System.MemoryExtensions.Equals() which will allocate both sides.
                return _underlyingComparer.Equals(span.ToString(), target);
            }

            string? ISpanAlternateEqualityComparer<char, string?>.Create(ReadOnlySpan<char> span) =>
                span.ToString();
        }

        public static IEqualityComparer<string>? GetStringComparer(object comparer)
        {
            // Special-case EqualityComparer<string>.Default, StringComparer.Ordinal, and StringComparer.OrdinalIgnoreCase.
            // We use a non-randomized comparer for improved perf, falling back to a randomized comparer if the
            // hash buckets become unbalanced.

            if (ReferenceEquals(comparer, EqualityComparer<string>.Default))
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

            if (StringComparerDescriptor.TryDescribe(comparer, out StringComparerDescriptor descriptor) &&
                TryGetStringComparer(in descriptor, out IEqualityComparer<string?>? stringComparer))
            {
                return stringComparer;
            }

            return null;
        }

        public static bool TryGetStringComparer(in StringComparerDescriptor descriptor, [MaybeNullWhen(false)] out IEqualityComparer<string?>? comparer)
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
