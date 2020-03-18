#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#if NET40

// https://github.com/dotnet/corefx/blob/48363ac826ccf66fbe31a5dcb1dc2aab9a7dd768/src/Common/src/CoreLib/System/Collections/Generic/IReadOnlyCollection.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Collections.Generic
{
    // Provides a read-only, covariant view of a generic list.
    public interface IReadOnlyCollection<out T> : IEnumerable<T>
    {
        int Count { get; }
    }
}

#endif