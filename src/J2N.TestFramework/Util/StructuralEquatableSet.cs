#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using J2N.Collections.Generic;
using System.Collections;

namespace J2N.Util
{
    public class StructuralEquatableSet<T> : System.Collections.Generic.HashSet<T>, IStructuralEquatable
    {
        public bool Equals(object other, IEqualityComparer comparer)
        {
            return SetEqualityComparer<T>.Equals(this, other, comparer);
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return SetEqualityComparer<T>.GetHashCode(this, comparer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj, SetEqualityComparer<T>.Default);
        }

        public override int GetHashCode()
        {
            return GetHashCode(SetEqualityComparer<T>.Default);
        }
    }

    //public class StructuralEquatableSet<T> : HashSet<T>, IStructuralEquatable
    //{
    //    private readonly SetEqualityComparer<T> equalityComparer;

    //    public StructuralEquatableSet(SetEqualityComparer<T> equalityComparer)
    //    {
    //        this.equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
    //    }

    //    public bool Equals(object other, IEqualityComparer comparer)
    //    {
    //        if (other is ISet<T> otherSet)
    //            return equalityComparer.Equals(this, otherSet);
    //        return false;
    //    }

    //    public int GetHashCode(IEqualityComparer comparer)
    //    {
    //        return equalityComparer.GetHashCode(this);
    //    }

    //    //public override bool Equals(object obj)
    //    //{
    //    //    return Equals(obj, equalityComparer);
    //    //}

    //    //public override int GetHashCode()
    //    //{
    //    //    return GetHashCode(equalityComparer);
    //    //}
    //}
}
