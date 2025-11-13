// Source: https://github.com/dotnet/runtime/blob/v10.0.0-rc.2.25502.107/src/libraries/System.Private.CoreLib/src/System/Globalization/SurrogateCasing.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace J2N.Globalization
{
    internal static class SurrogateCasing
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ToUpper(char h, char l, out char hr, out char lr)
        {
            Debug.Assert(char.IsHighSurrogate(h));
            Debug.Assert(char.IsLowSurrogate(l));

            //UnicodeUtility.GetUtf16SurrogatesFromSupplementaryPlaneScalar(CharUnicodeInfo.ToUpper(UnicodeUtility.GetScalarFromUtf16SurrogatePair(h, l)), out hr, out lr);

            Span<char> buffer = stackalloc char[2] { h, l };
            Span<char> result = stackalloc char[2];
            System.MemoryExtensions.ToUpperInvariant(buffer, result);
            hr = result[0];
            lr = result[1];

            Debug.Assert(char.IsHighSurrogate(hr));
            Debug.Assert(char.IsLowSurrogate(lr));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void ToLower(char h, char l, out char hr, out char lr)
        {
            Debug.Assert(char.IsHighSurrogate(h));
            Debug.Assert(char.IsLowSurrogate(l));

            //UnicodeUtility.GetUtf16SurrogatesFromSupplementaryPlaneScalar(CharUnicodeInfo.ToLower(UnicodeUtility.GetScalarFromUtf16SurrogatePair(h, l)), out hr, out lr);

            Span<char> buffer = stackalloc char[2] { h, l };
            Span<char> result = stackalloc char[2];
            System.MemoryExtensions.ToLowerInvariant(buffer, result);
            hr = result[0];
            lr = result[1];

            Debug.Assert(char.IsHighSurrogate(hr));
            Debug.Assert(char.IsLowSurrogate(lr));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Equal(char h1, char l1, char h2, char l2)
        {
            ToUpper(h1, l1, out char hr1, out char lr1);
            ToUpper(h2, l2, out char hr2, out char lr2);

            return hr1 == hr2 && lr1 == lr2;
        }
    }
}
