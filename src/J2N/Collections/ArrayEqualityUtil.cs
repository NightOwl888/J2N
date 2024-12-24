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

using System;

namespace J2N.Collections
{
    /// <summary>
    /// Utilities for array equality of arrays and collections.
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    internal static class ArrayEqualityUtil
    {
        public static System.Collections.IEqualityComparer GetPrimitiveOneDimensionalArrayEqualityComparer(Type elementType)
        {
            if (typeof(int).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<int>.ValueTypeOneDimensionalArrayEqualityComparer();
            if (typeof(char).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<char>.ValueTypeOneDimensionalArrayEqualityComparer();
            if (typeof(string).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<string>.StringOneDimensionalArrayEqualityComparer(); // Special case: always Ordinal
            if (typeof(bool).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<bool>.ValueTypeOneDimensionalArrayEqualityComparer();
            if (typeof(byte).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<byte>.ValueTypeOneDimensionalArrayEqualityComparer();
            if (typeof(long).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<long>.ValueTypeOneDimensionalArrayEqualityComparer();
            if (typeof(float).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<float>.ValueTypeOneDimensionalArrayEqualityComparer(); // Special case: negative zero not equal to postive zero, NaN always equal to NaN;
            if (typeof(double).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<double>.ValueTypeOneDimensionalArrayEqualityComparer(); // Special case: negative zero not equal to postive zero, NaN always equal to NaN
            if (typeof(short).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<short>.ValueTypeOneDimensionalArrayEqualityComparer();

            // .NET primitive types that don't exist in Java
            if (typeof(decimal).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<decimal>.ValueTypeOneDimensionalArrayEqualityComparer();
            if (typeof(IntPtr).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<IntPtr>.ValueTypeOneDimensionalArrayEqualityComparer();
            if (typeof(uint).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<uint>.ValueTypeOneDimensionalArrayEqualityComparer();
            if (typeof(sbyte).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<sbyte>.ValueTypeOneDimensionalArrayEqualityComparer();
            if (typeof(ulong).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<ulong>.ValueTypeOneDimensionalArrayEqualityComparer();
            if (typeof(ushort).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<ushort>.ValueTypeOneDimensionalArrayEqualityComparer();
            if (typeof(UIntPtr).Equals(elementType))
                return new OneDimensionalArrayEqualityComparer<UIntPtr>.ValueTypeOneDimensionalArrayEqualityComparer();

            throw new ArgumentException(J2N.SR.Format(SR.Argument_MustBePrimitiveType, elementType));
        }
    }
}
