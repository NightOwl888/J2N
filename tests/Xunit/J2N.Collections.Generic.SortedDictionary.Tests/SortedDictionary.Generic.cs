// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using System;
using Xunit;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class SortedDictionary_Generic_Tests_string_string : SortedDictionary_Generic_Tests<string, string>
    {
        protected override SCG.KeyValuePair<string, string> CreateT(int seed)
        {
            return new SCG.KeyValuePair<string, string>(CreateTKey(seed), CreateTKey(seed + 500));
        }

        protected override string CreateTKey(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes1 = new byte[stringLength];
            rand.NextBytes(bytes1);
            return Convert.ToBase64String(bytes1);
        }

        protected override string CreateTValue(int seed)
        {
            return CreateTKey(seed);
        }
    }

    public class SortedDictionary_Generic_Tests_int_int : SortedDictionary_Generic_Tests<int, int>
    {
        protected override bool DefaultValueAllowed { get { return true; } }
        protected override SCG.KeyValuePair<int, int> CreateT(int seed)
        {
            Random rand = new Random(seed);
            return new SCG.KeyValuePair<int, int>(rand.Next(), rand.Next());
        }

        protected override int CreateTKey(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override int CreateTValue(int seed)
        {
            return CreateTKey(seed);
        }
    }

    //[OuterLoop]
    public class SortedDictionary_Generic_Tests_EquatableBackwardsOrder_int : SortedDictionary_Generic_Tests<EquatableBackwardsOrder, int>
    {
        protected override SCG.KeyValuePair<EquatableBackwardsOrder, int> CreateT(int seed)
        {
            Random rand = new Random(seed);
            return new SCG.KeyValuePair<EquatableBackwardsOrder, int>(new EquatableBackwardsOrder(rand.Next()), rand.Next());
        }

        protected override EquatableBackwardsOrder CreateTKey(int seed)
        {
            Random rand = new Random(seed);
            return new EquatableBackwardsOrder(rand.Next());
        }

        protected override int CreateTValue(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override SCG.IDictionary<EquatableBackwardsOrder, int> GenericIDictionaryFactory()
        {
            return new SortedDictionary<EquatableBackwardsOrder, int>();
        }
    }
}
