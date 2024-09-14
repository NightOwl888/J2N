// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Xunit
{
    internal class ConditionalDiscovererException : Exception
    {
        public ConditionalDiscovererException(string message) : base(message) { }
    }
}
