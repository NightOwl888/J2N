// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !FEATURE_REQUIRESDYNAMICCODEATTRIBUTE
using System;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace J2N
{
    /// <summary>
    /// Provides a compatibility shim for <c>System.Diagnostics.CodeAnalysis.RequiresDynamicCodeAttribute</c>
    /// on target frameworks where it is not available (for example, .NET Framework and older .NET Standard).
    /// </summary>
    /// <remarks>
    /// This attribute is used to annotate APIs that rely on runtime code generation or reflection-based
    /// generic dispatch, which may not be supported in ahead-of-time (AOT) compilation scenarios.
    /// <para/>
    /// On platforms that support AOT, callers should avoid invoking members marked with this attribute,
    /// or ensure that <c>RuntimeFeature.IsDynamicCodeSupported</c> returns <c>true</c>.
    /// <para/>
    /// This implementation is a no-op placeholder and exists only to allow multi-targeted builds
    /// to compile without conditional code paths.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Class, Inherited = false)]
    internal class RequiresDynamicCodeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequiresDynamicCodeAttribute"/> class
        /// with the specified message.
        /// </summary>
        /// <param name="message">
        /// A message that contains information about the usage of dynamic code.
        /// </param>
        public RequiresDynamicCodeAttribute(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets a message that contains information about the usage of dynamic code.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets or sets an optional URL that contains more information about the method,
        /// why it requires dynamic code, and what options a consumer has to deal with it.
        /// </summary>
        public string? Url { get; set; }
    }
}
#endif