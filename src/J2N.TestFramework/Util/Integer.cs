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

using System;
using System.Globalization;

namespace J2N.Util
{
    /// <summary>
    /// Very simple wrapper for <see cref="int"/> to make it into a reference type.
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class Integer : IComparable<Integer>
    {
        public Integer(int value)
        {
            this.Value = value;
        }

        public int Value { get; set; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Integer integer)
            {
                return Value.Equals(integer.Value);
            }
            return Value.Equals(obj);
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public int CompareTo(Integer other)
        {
            if (other == null)
                return 1;
            return this.Value.CompareTo(other.Value);
        }

        public static implicit operator int(Integer integer)
        {
            return integer.Value;
        }

        public static implicit operator Integer(int integer)
        {
            return new Integer(integer);
        }
    }
}
