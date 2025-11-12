// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

        private sealed class OrdinalComparer : NonRandomizedStringEqualityComparer
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
                
                return StringHelper.GetNonRandomizedHashCode(obj);
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
        }

        private sealed class OrdinalIgnoreCaseComparer : NonRandomizedStringEqualityComparer
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

                return StringHelper.GetNonRandomizedHashCodeOrdinalIgnoreCase(obj);
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

            internal override RandomizedStringEqualityComparer GetRandomizedEqualityComparer()
            {
                return RandomizedStringEqualityComparer.Create(_underlyingComparer, ignoreCase: true);
            }
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

            return null;
        }
    }
}
