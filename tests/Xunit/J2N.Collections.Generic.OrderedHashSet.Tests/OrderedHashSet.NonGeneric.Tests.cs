// Source: https://github.com/dotnet/runtime/blob/v10.0.0-rc.2.25502.107/src/libraries/System.Collections/tests/Generic/HashSet/HashSet.NonGeneric.Tests.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using System;
using System.Reflection;
using System.Runtime.Serialization;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public static class OrderedHashSet_NonGeneric_Tests
    {
        // J2N: HashSet_CopyConstructor_ShouldWorkWithRandomizedEffectiveComparer omitted because serialization is not supported

        [Theory]
        [InlineData(10)]
        [InlineData(1000)]
        [InlineData(1000_000)]
        public static void InsertionOpsOnly_Enumeration_PreservesInsertionOrder(int count)
        {
            var set = new HashSet<string>();
            for (int i = 0; i < count; i++)
            {
                set.Add(i.ToString());
            }

            int j = 0;
            foreach (string elem in set)
            {
                Assert.Equal(j.ToString(), elem);
                j++;
            }
        }
    }
}
