#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#if NET40

using System;

// https://github.com/dotnet/corefx/blob/48363ac826ccf66fbe31a5dcb1dc2aab9a7dd768/src/Common/src/CoreLib/System/Runtime/CompilerServices/MethodImplOptions.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace J2N.Compatibility
{
    // This Enum matchs the miImpl flags defined in corhdr.h. It is used to specify
    // certain method properties.
    [Flags]
    public enum MethodImplOptions
    {
        Unmanaged = 0x0004,
        NoInlining = 0x0008,
        ForwardRef = 0x0010,
        Synchronized = 0x0020,
        NoOptimization = 0x0040,
        PreserveSig = 0x0080,
        AggressiveInlining = 0x0100,
        AggressiveOptimization = 0x0200,
        InternalCall = 0x1000
    }
}
#endif