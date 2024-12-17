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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace J2N.Collections
{
    /// <summary>
    /// Provides comparers that use structural equality rules for one dimensional arrays similar to those in Java.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    // J2N NOTE: subclassing ArrayEqualityComparer works on .NET Framework, .NET Core, and .NET 5, but causes infinite recursion on Xamarin.Android
    // due to the static initialization of Default being called by LoadDefault().
    internal abstract class OneDimensionalArrayEqualityComparer<T> : IEqualityComparer<T[]>, System.Collections.IEqualityComparer //: ArrayEqualityComparer<T[]>
    {
        /// <summary>
        /// Returns a default equality comparer for the type specified by the generic argument with equality rules similar
        /// to those in Java.
        /// </summary>
        public static IEqualityComparer<T[]> Default { get; } = LoadDefault();

        private static IEqualityComparer<T[]> LoadDefault()
        {
            Type elementType = typeof(T);
            if (elementType.IsPrimitive)
                return (IEqualityComparer<T[]>)ArrayEqualityUtil.GetPrimitiveOneDimensionalArrayEqualityComparer(elementType);
            else if (elementType.IsValueType)
                return new ValueTypeOneDimensionalArrayEqualityComparer();

            return new GenericOneDimensionalArrayEqualityComparer();
        }

        /// <summary>
        /// Compares the two arrays.
        /// </summary>
        /// <param name="array1">The first <typeparamref name="T"/> array.</param>
        /// <param name="array2">The second <typeparamref name="T"/> array.</param>
        /// <returns><c>true</c> if both arrays are <c>null</c> or if the arrays have the
        /// same length and the elements at each index in the two arrays are
        /// equal; otherwise, <c>false</c>.</returns>
        public abstract bool Equals([AllowNull] T[] array1, [AllowNull] T[] array2);

        /// <summary>
        /// Returns a hash code based on the contents of the given array. For any two
        /// <typeparamref name="T"/> arrays <c>a</c> and <c>b</c>, if
        /// <c>Equals(a, b)</c> returns <c>true</c>, it means
        /// that the return value of <c>GetHashCode(a)</c> equals <c>GetHashCode(b)</c>.
        /// </summary>
        /// <param name="array">The array whose hash code to compute.</param>
        /// <returns>The hash code for <paramref name="array"/>.</returns>
        public abstract int GetHashCode([AllowNull] T[] array);


        int System.Collections.IEqualityComparer.GetHashCode(object? array)
        {
            if (array == null) return 0;
            if (array is T[] t) return GetHashCode(t);
            throw new ArgumentException(SR.Argument_InvalidArgumentForComparison);
        }

        bool System.Collections.IEqualityComparer.Equals(object? array1, object? array2)
        {
            if (array1 == array2) return true;
            if (array1 == null || array2 == null) return false;
            if ((array1 is T[] t1) && (array2 is T[] t2)) return Equals(t1, t2);
            throw new ArgumentException(SR.Argument_InvalidArgumentForComparison);
        }

        /// <summary>
        /// Structural equality comparer for arrays of <typeparamref name="T"/>.
        /// Used if the array type is not primitive.
        /// </summary>
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class GenericOneDimensionalArrayEqualityComparer : OneDimensionalArrayEqualityComparer<T>
        {
            /// <summary>
            /// Compares the two arrays.
            /// </summary>
            /// <param name="array1">The first <typeparamref name="T"/> array.</param>
            /// <param name="array2">The second <typeparamref name="T"/> array.</param>
            /// <returns><c>true</c> if both arrays are <c>null</c> or if the arrays have the
            /// same length and the elements at each index in the two arrays are
            /// equal; otherwise, <c>false</c>.</returns>
            public override bool Equals([AllowNull] T[] array1, [AllowNull] T[] array2)
            {
                if (ReferenceEquals(array1, array2))
                    return true;

                if (array1 == null || array2 == null || array1.Length != array2.Length)
                    return false;
                T e1, e2;
                for (int i = 0; i < array1.Length; i++)
                {
                    e1 = array1[i];
                    e2 = array2[i];
                    if (!(e1 == null ? e2 == null : J2N.Collections.Generic.EqualityComparer<T>.Default.Equals(e1, e2)))
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Returns a hash code based on the contents of the given array. For any two
            /// <typeparamref name="T"/> arrays <c>a</c> and <c>b</c>, if
            /// <c>GenericOneDimensionalArrayEqualityComparer.Equals(a, b)</c> returns <c>true</c>, it means
            /// that the return value of <c>GenericOneDimensionalArrayEqualityComparer.GetHashCode(a)</c> equals
            /// <c>GenericOneDimensionalArrayEqualityComparer.GetHashCode(b)</c>.
            /// </summary>
            /// <param name="array">The array whose hash code to compute.</param>
            /// <returns>The hash code for <paramref name="array"/>.</returns>
            public override int GetHashCode([AllowNull] T[] array)
            {
                if (array == null)
                    return 0;
                int hashCode = 1, elementHashCode;
                foreach (var element in array)
                {
                    elementHashCode = 0;
                    if (element != null)
                    {
                        // NOTE: An array of type object can contain primitive types. So we need to do that
                        // check within the loop.
                        if (element is string eString)
                            elementHashCode = J2N.Collections.Generic.EqualityComparer<string>.Default.GetHashCode(eString);
                        else if (element is float eFloat)
                            elementHashCode = J2N.Collections.Generic.EqualityComparer<float>.Default.GetHashCode(eFloat);
                        else if (element is double eDouble)
                            elementHashCode = J2N.Collections.Generic.EqualityComparer<double>.Default.GetHashCode(eDouble);
                        else
                            elementHashCode = J2N.Collections.Generic.EqualityComparer<T>.Default.GetHashCode(element);
                    }

                    hashCode = 31 * hashCode + elementHashCode;
                }
                return hashCode;
            }
        }

        /// <summary>
        /// Structural equality comparer for arrays of value types without any special rules
        /// that differ from .NET defaults.
        /// </summary>
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class ValueTypeOneDimensionalArrayEqualityComparer : OneDimensionalArrayEqualityComparer<T>
        // where TValue : struct // J2N: Need to ensure this is correct explicitly
        {
            /// <summary>
            /// Compares the two arrays.
            /// </summary>
            /// <param name="array1">The first <typeparamref name="T"/> array.</param>
            /// <param name="array2">The second <typeparamref name="T"/> array.</param>
            /// <returns><c>true</c> if both arrays are <c>null</c> or if the arrays have the
            /// same length and the elements at each index in the two arrays are
            /// equal; otherwise, <c>false</c>.</returns>
            public override bool Equals([AllowNull] T[] array1, [AllowNull] T[] array2)
            {
                if (ReferenceEquals(array1, array2))
                    return true;

                if (array1 == null || array2 == null || array1.Length != array2.Length)
                    return false;

                for (int i = 0; i < array1.Length; i++)
                {
                    if (!J2N.Collections.Generic.EqualityComparer<T>.Default.Equals(array1[i], array2[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Returns a hash code based on the contents of the given array. For any two
            /// <typeparamref name="T"/> arrays <c>a</c> and <c>b</c>, if
            /// <c>ValueTypeOneDimensionalArrayEqualityComparer.Equals(a, b)</c> returns <c>true</c>, it means
            /// that the return value of <c>ValueTypeOneDimensionalArrayEqualityComparer.GetHashCode(a)</c> equals
            /// <c>ValueTypeOneDimensionalArrayEqualityComparer.GetHashCode(b)</c>.
            /// </summary>
            /// <param name="array">The array whose hash code to compute.</param>
            /// <returns>The hash code for <paramref name="array"/>.</returns>
            public override int GetHashCode([AllowNull] T[] array)
            {
                if (array == null)
                    return 0;
                int hashCode = 1;
                foreach (var element in array)
                {
                    // the hash code value is determined by the default equality comparer
                    hashCode = 31 * hashCode + J2N.Collections.Generic.EqualityComparer<T>.Default.GetHashCode(element!);
                }
                return hashCode;
            }
        }

        /// <summary>
        /// Structural equality comparer for arrays of <see cref="string"/>.
        /// </summary>
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal class StringOneDimensionalArrayEqualityComparer : OneDimensionalArrayEqualityComparer<string>
        {
            /// <summary>
            /// Compares the two arrays.
            /// </summary>
            /// <param name="array1">The first <see cref="string"/> array.</param>
            /// <param name="array2">The second <see cref="string"/> array.</param>
            /// <returns><c>true</c> if both arrays are <c>null</c> or if the arrays have the
            /// same length and the elements at each index in the two arrays are
            /// equal; otherwise, <c>false</c>.</returns>
            public override bool Equals([AllowNull] string[] array1, [AllowNull] string[] array2)
            {
                if (ReferenceEquals(array1, array2))
                    return true;

                if (array1 == null || array2 == null || array1.Length != array2.Length)
                    return false;

                for (int i = 0; i < array1.Length; i++)
                {
                    if (!StringComparer.Ordinal.Equals(array1[i], array2[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Returns a hash code based on the contents of the given array. For any two
            /// <see cref="string"/> arrays <c>a</c> and <c>b</c>, if
            /// <c>StringOneDimensionalArrayEqualityComparer.Equals(a, b)</c> returns <c>true</c>, it means
            /// that the return value of <c>StringOneDimensionalArrayEqualityComparer.GetHashCode(a)</c> equals
            /// <c>StringOneDimensionalArrayEqualityComparer.GetHashCode(b)</c>.
            /// </summary>
            /// <param name="array">The array whose hash code to compute.</param>
            /// <returns>The hash code for <paramref name="array"/>.</returns>
            public override int GetHashCode([AllowNull] string[] array)
            {
                if (array == null)
                    return 0;
                int hashCode = 1, elementHashCode;
                foreach (var element in array)
                {
                    elementHashCode = element == null ? 0 : StringComparer.Ordinal.GetHashCode(element);
                    hashCode = 31 * hashCode + elementHashCode;
                }
                return hashCode;
            }
        }
    }
}
