#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using J2N.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace J2N.Util
{
    [SuppressMessage("Usage", "CA2237:Mark ISerializable types with serializable", Justification = "serialization not required for testing")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "CA2237 doesn't fire on all target frameworks")]
    public class StructuralEquatableDictionary<TKey, TValue> : System.Collections.Generic.Dictionary<TKey, TValue>, IStructuralEquatable
    {

        public bool Equals(object other, IEqualityComparer comparer)
        {
            return DictionaryEqualityComparer<TKey, TValue>.Equals(this, other, comparer);
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return DictionaryEqualityComparer<TKey, TValue>.GetHashCode(this, comparer);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj, DictionaryEqualityComparer<TKey, TValue>.Default);
        }

        public override int GetHashCode()
        {
            return GetHashCode(DictionaryEqualityComparer<TKey, TValue>.Default);
        }
    }
}
