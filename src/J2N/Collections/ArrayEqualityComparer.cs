using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace J2N.Collections
{
    /// <summary>
    /// Provides comparers that use structural equality rules for arrays similar to those in Java.
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public abstract class ArrayEqualityComparer<T> : System.Collections.Generic.EqualityComparer<T>
    {
        /// <summary>
        /// Hidden default property that doesn't apply to this class.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new static IEqualityComparer<T> Default { get; }

        /// <summary>
        /// Gets a structural equality comparer for the specified generic array type with comparison rules similar
        /// to the JDK's Arrays class.
        /// <para/>
        /// This provides a high-performance array comparison that is faster than the
        /// <see cref="System.Collections.StructuralComparisons.StructuralEqualityComparer"/>.
        /// </summary>
        public static IEqualityComparer<T[]> OneDimensional { get; } = OneDimensionalArrayEqualityComparer<T>.Default;


        internal static System.Collections.IEqualityComparer GetPrimitiveOneDimensionalArrayEqualityComparer(Type elementType)
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

            // .NET primitive types
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

            throw new ArgumentException($"'{elementType}' is not a primitive type.");
        }


        /// <summary>
        /// Provides comparers that use structural equality rules for one dimensional arrays similar to those in Java.
        /// </summary>
        /// <typeparam name="T1">The type of objects to compare.</typeparam>
#if FEATURE_SERIALIZABLE
        [Serializable]
#endif
        internal abstract class OneDimensionalArrayEqualityComparer<T1> : ArrayEqualityComparer<T1[]> //: System.Collections.Generic.EqualityComparer<T[]>//, Comparer<T[]>
        {
            /// <summary>
            /// Returns a default equality comparer for the type specified by the generic argument with equality rules similar
            /// to those in Java.
            /// </summary>
            public new static IEqualityComparer<T1[]> Default { get; } = LoadDefault();

            private static IEqualityComparer<T1[]> LoadDefault()
            {
                Type elementType = typeof(T1);
                if (elementType.GetTypeInfo().IsPrimitive)
                    return (IEqualityComparer<T1[]>)GetPrimitiveOneDimensionalArrayEqualityComparer(elementType);
                else if (elementType.GetTypeInfo().IsValueType)
                    return new ValueTypeOneDimensionalArrayEqualityComparer();

                return new GenericOneDimensionalArrayEqualityComparer();
            }

            /// <summary>
            /// Compares the two arrays.
            /// </summary>
            /// <param name="array1">The first <typeparamref name="T1"/> array.</param>
            /// <param name="array2">The second <typeparamref name="T1"/> array.</param>
            /// <returns><c>true</c> if both arrays are <c>null</c> or if the arrays have the
            /// same length and the elements at each index in the two arrays are
            /// equal; otherwise, <c>false</c>.</returns>
            public abstract override bool Equals(/*[AllowNull]*/ T1[] array1, /*[AllowNull]*/ T1[] array2);

            /// <summary>
            /// Returns a hash code based on the contents of the given array. For any two
            /// <typeparamref name="T1"/> arrays <c>a</c> and <c>b</c>, if
            /// <c>Equals(a, b)</c> returns <c>true</c>, it means
            /// that the return value of <c>GetHashCode(a)</c> equals <c>GetHashCode(b)</c>.
            /// </summary>
            /// <param name="array">The array whose hash code to compute.</param>
            /// <returns>The hash code for <paramref name="array"/>.</returns>
            public abstract override int GetHashCode(/*[AllowNull]*/ T1[] array);


            /// <summary>
            /// Structural equality comparer for arrays of <typeparamref name="T1"/>.
            /// Used if the array type is not primitive.
            /// </summary>
#if FEATURE_SERIALIZABLE
            [Serializable]
#endif
            internal class GenericOneDimensionalArrayEqualityComparer : OneDimensionalArrayEqualityComparer<T1>
            {
                /// <summary>
                /// Compares the two arrays.
                /// </summary>
                /// <param name="array1">The first <typeparamref name="T1"/> array.</param>
                /// <param name="array2">The second <typeparamref name="T1"/> array.</param>
                /// <returns><c>true</c> if both arrays are <c>null</c> or if the arrays have the
                /// same length and the elements at each index in the two arrays are
                /// equal; otherwise, <c>false</c>.</returns>
                public override bool Equals(/*[AllowNull]*/ T1[] array1, /*[AllowNull]*/ T1[] array2)
                {
                    if (ReferenceEquals(array1, array2))
                        return true;
                    int arrayLength = array1.Length;
                    if (array1 == null || array2 == null || arrayLength != array2.Length)
                        return false;
                    T1 e1, e2;
                    for (int i = 0; i < array1.Length; i++)
                    {
                        e1 = array1[i];
                        e2 = array2[i];
                        if (!(e1 == null ? e2 == null : EqualityComparer<T1>.Default.Equals(e1, e2)))
                        {
                            return false;
                        }
                    }
                    return true;
                }

                /// <summary>
                /// Returns a hash code based on the contents of the given array. For any two
                /// <typeparamref name="T1"/> arrays <c>a</c> and <c>b</c>, if
                /// <c>Arrays.Equals(b)</c> returns <c>true</c>, it means
                /// that the return value of <c>Arrays.GetHashCode(a)</c> equals <c>Arrays.GetHashCode(b)</c>.
                /// </summary>
                /// <param name="array">The array whose hash code to compute.</param>
                /// <returns>The hash code for <paramref name="array"/>.</returns>
                public override int GetHashCode(/*[AllowNull]*/ T1[] array)
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
                                elementHashCode = EqualityComparer<string>.Default.GetHashCode(eString);
                            else if (element is float eFloat)
                                elementHashCode = EqualityComparer<float>.Default.GetHashCode(eFloat);
                            else if (element is double eDouble)
                                elementHashCode = EqualityComparer<double>.Default.GetHashCode(eDouble);
                            else
                                elementHashCode = EqualityComparer<T1>.Default.GetHashCode(element);
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
            internal class ValueTypeOneDimensionalArrayEqualityComparer : OneDimensionalArrayEqualityComparer<T1>
            // where TValue : struct // J2N: Need to ensure this is correct explicitly
            {
                /// <summary>
                /// Compares the two arrays.
                /// </summary>
                /// <param name="array1">The first <see cref="short"/> array.</param>
                /// <param name="array2">The second <see cref="short"/> array.</param>
                /// <returns><c>true</c> if both arrays are <c>null</c> or if the arrays have the
                /// same length and the elements at each index in the two arrays are
                /// equal; otherwise, <c>false</c>.</returns>
                public override bool Equals(/*[AllowNull]*/ T1[] array1, /*[AllowNull]*/ T1[] array2)
                {
                    if (ReferenceEquals(array1, array2))
                        return true;
                    int arrayLength = array1.Length;
                    if (array1 == null || array2 == null || arrayLength != array2.Length)
                        return false;

                    for (int i = 0; i < array1.Length; i++)
                    {
                        if (!J2N.EqualityComparer<T1>.Default.Equals(array1[i], array2[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }

                /// <summary>
                /// Returns a hash code based on the contents of the given array. For any two
                /// <see cref="short"/> arrays <c>a</c> and <c>b</c>, if
                /// <c>Arrays.Equals(b)</c> returns <c>true</c>, it means
                /// that the return value of <c>Arrays.GetHashCode(a)</c> equals <c>Arrays.GetHashCode(b)</c>.
                /// </summary>
                /// <param name="array">The array whose hash code to compute.</param>
                /// <returns>The hash code for <paramref name="array"/>.</returns>
                public override int GetHashCode(/*[AllowNull]*/ T1[] array)
                {
                    if (array == null)
                        return 0;
                    int hashCode = 1;
                    foreach (var element in array)
                    {
                        // the hash code value is determined by the default equality comparer
                        hashCode = 31 * hashCode + J2N.EqualityComparer<T1>.Default.GetHashCode(element);
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
                public override bool Equals(/*[AllowNull]*/ string[] array1, /*[AllowNull]*/ string[] array2)
                {
                    if (ReferenceEquals(array1, array2))
                        return true;
                    int arrayLength = array1.Length;
                    if (array1 == null || array2 == null || arrayLength != array2.Length)
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
                /// <c>Arrays.Equals(b)</c> returns <c>true</c>, it means
                /// that the return value of <c>Arrays.GetHashCode(a)</c> equals <c>Arrays.GetHashCode(b)</c>.
                /// </summary>
                /// <param name="array">The array whose hash code to compute.</param>
                /// <returns>The hash code for <paramref name="array"/>.</returns>
                public override int GetHashCode(/*[AllowNull]*/ string[] array)
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
}