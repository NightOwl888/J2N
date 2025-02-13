// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if FEATURE_COMPOSITEFORMAT

using System;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// Extensions to <see cref="CompositeFormat"/>.
    /// </summary>
    internal static class CompositeFormatExtensions
    {
        /// <summary>Throws an exception if the specified number of arguments is fewer than the number required.</summary>
        /// <param name="format">This <see cref="CompositeFormat"/>.</param>
        /// <param name="numArgs">The number of arguments provided by the caller.</param>
        /// <exception cref="FormatException">An insufficient number of arguments were provided.</exception>
        public static void ValidateNumberOfArgs(this CompositeFormat format, int numArgs) // From CompositeFormat
        {
            if (numArgs < format.MinimumArgumentCount)
            {
                ThrowHelper.ThrowFormatIndexOutOfRange();
            }
        }
    }
}

#endif