// Source: https://github.com/dotnet/arcade/blob/release/9.0/src/Microsoft.DotNet.XUnitExtensions/tests/AlphabeticalOrderer.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace J2N.TestUtilities.Xunit.Tests
{
    // We need this TestCaseOrderer in order to guarantee that the ConditionalAttributeTests are always executed in the same order.
    public class AlphabeticalOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
                where TTestCase : ITestCase
        {
            List<TTestCase> result = testCases.ToList();
            result.Sort((x, y) => StringComparer.Ordinal.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
            return result;
        }
    }
}
