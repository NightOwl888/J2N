// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !FEATURE_RUNTIMEFEATURE

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace J2N
{
    /// <summary>
    /// Provides a compatibility shim for <c>System.Runtime.CompilerServices.RuntimeFeature</c>
    /// on target frameworks where it is not available.
    /// </summary>
    /// <remarks>
    /// This implementation always returns <c>true</c>, assuming dynamic code is supported.
    /// <para/>
    /// On modern runtimes that support AOT (such as NativeAOT, iOS, or WASM),
    /// the real <c>RuntimeFeature.IsDynamicCodeSupported</c> should be used instead.
    /// <para/>
    /// This shim exists solely to simplify multi-targeting and avoid conditional compilation
    /// in shared code.
    /// </remarks>
    internal static class RuntimeFeature
    {
        /// <summary>
        /// Gets a value that indicates whether the runtime supports dynamic code.
        /// </summary>
        /// <value>
        /// Always returns <c>true</c> on .NET Framework and older .NET Standard versions, which do support dynamic code.
        /// </value>
        public static bool IsDynamicCodeSupported => true;
    }
}
#endif