// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using JCG = J2N.Collections.Generic;

namespace J2N.Collections.Tests
{
    public class Net5_HashSet_Generic_Tests_string : HashSet_Generic_Tests<string>
    {
        protected override string CreateT(int seed)
        {
            int stringLength = seed % 10 + 5;
            Random rand = new Random(seed);
            byte[] bytes = new byte[stringLength];
            rand.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }

    public class Net5_HashSet_Generic_Tests_int : HashSet_Generic_Tests<int>
    {
        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override bool DefaultValueAllowed => true;
    }

    public class Net5_HashSet_Generic_Tests_int_With_Comparer_WrapStructural_Int : HashSet_Generic_Tests<int>
    {
        protected override IEqualityComparer<int> GetIEqualityComparer()
        {
            return new WrapStructural_Int();
        }

        protected override IComparer<int> GetIComparer()
        {
            return new WrapStructural_Int();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            return new JCG.Net5.HashSet<int>(new WrapStructural_Int());
        }
    }

    public class Net5_HashSet_Generic_Tests_int_With_Comparer_WrapStructural_SimpleInt : HashSet_Generic_Tests<SimpleInt>
    {
        protected override IEqualityComparer<SimpleInt> GetIEqualityComparer()
        {
            return new WrapStructural_SimpleInt();
        }

        protected override IComparer<SimpleInt> GetIComparer()
        {
            return new WrapStructural_SimpleInt();
        }

        protected override SimpleInt CreateT(int seed)
        {
            Random rand = new Random(seed);
            return new SimpleInt(rand.Next());
        }

        protected override ISet<SimpleInt> GenericISetFactory()
        {
            return new JCG.Net5.HashSet<SimpleInt>(new WrapStructural_SimpleInt());
        }
    }

    //[OuterLoop]
    public class Net5_HashSet_Generic_Tests_EquatableBackwardsOrder : HashSet_Generic_Tests<EquatableBackwardsOrder>
    {
        protected override EquatableBackwardsOrder CreateT(int seed)
        {
            Random rand = new Random(seed);
            return new EquatableBackwardsOrder(rand.Next());
        }

        protected override ISet<EquatableBackwardsOrder> GenericISetFactory()
        {
            return new JCG.Net5.HashSet<EquatableBackwardsOrder>();
        }
    }

    //[OuterLoop]
    public class Net5_HashSet_Generic_Tests_int_With_Comparer_SameAsDefaultComparer : HashSet_Generic_Tests<int>
    {
        protected override IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_SameAsDefaultComparer();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            return new JCG.Net5.HashSet<int>(new Comparer_SameAsDefaultComparer());
        }
    }

    //[OuterLoop]
    public class Net5_HashSet_Generic_Tests_int_With_Comparer_HashCodeAlwaysReturnsZero : HashSet_Generic_Tests<int>
    {
        protected override IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_HashCodeAlwaysReturnsZero();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            return new JCG.Net5.HashSet<int>(new Comparer_HashCodeAlwaysReturnsZero());
        }
    }

    //[OuterLoop]
    public class Net5_HashSet_Generic_Tests_int_With_Comparer_ModOfInt : HashSet_Generic_Tests<int>
    {
        protected override IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_ModOfInt(15000);
        }

        protected override IComparer<int> GetIComparer()
        {
            return new Comparer_ModOfInt(15000);
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            return new JCG.Net5.HashSet<int>(new Comparer_ModOfInt(15000));
        }
    }

    //[OuterLoop]
    public class Net5_HashSet_Generic_Tests_int_With_Comparer_AbsOfInt : HashSet_Generic_Tests<int>
    {
        protected override IEqualityComparer<int> GetIEqualityComparer()
        {
            return new Comparer_AbsOfInt();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            return new JCG.Net5.HashSet<int>(new Comparer_AbsOfInt());
        }
    }

    //[OuterLoop]
    public class Net5_HashSet_Generic_Tests_int_With_Comparer_BadIntEqualityComparer : HashSet_Generic_Tests<int>
    {
        protected override IEqualityComparer<int> GetIEqualityComparer()
        {
            return new BadIntEqualityComparer();
        }

        protected override int CreateT(int seed)
        {
            Random rand = new Random(seed);
            return rand.Next();
        }

        protected override ISet<int> GenericISetFactory()
        {
            return new JCG.Net5.HashSet<int>(new BadIntEqualityComparer());
        }
    }
}
