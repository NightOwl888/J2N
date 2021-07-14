#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
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

using J2N.Globalization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JCG = J2N.Collections.Generic;

namespace J2N.Numerics
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// An immutable reference type that wraps the primitive <see cref="float"/> type.
    /// <para/>
    /// In addition, this class provides methods for converting a <see cref="float"/> to a <see cref="string"/> and
    /// a <see cref="string"/> to a <see cref="float"/> that are compatible with Java. These methods provide better performance
    /// for .NET Core 3.0 and higher as well as patch the broken round-trip behavior and add support for negative zero for
    /// .NET platforms prior to .NET Core 3.0.
    /// <para/>
    /// Instances of this class can be produced implicitly by setting a <see cref="float"/> value to a variable declared
    /// as <see cref="Single"/>
    /// <code>
    /// float value = 4.4f;
    /// Single instance = value;
    /// </code>
    /// Or explicitly by calling one of the <see cref="GetInstance(float)"/> methods.
    /// <para/>
    /// The <see cref="float"/> value of a <see cref="Single"/> can also be retrieved in several ways. For implicit
    /// conversion, simply assign a <see cref="float"/> variable an instance of <see cref="Single"/>.
    /// <code>
    /// Single instance = Single.GetInstance(4.4f);
    /// float value = instance;
    /// </code>
    /// To explicitly get the value, call <see cref="ToSingle()"/> or use the <see cref="Convert"/> class.
    /// <code>
    /// float converted1 = instance.ToSingle();
    /// float converted2 = Convert.ToSingle(instance, NumberFormatInfo.InvariantInfo);
    /// </code>
    /// <para/>
    /// In most cases, the number types in .NET will suffice. The main reason for creating an object to wrap numeric types is to
    /// provide a way to make strongly-typed instances that can co-exist in collections and arrays with reference types.
    /// For example, when creating a table object that has columns with a mix of number and string data types.
    /// When porting code from Java, there are sometimes cases where the design didn't factor in the use of value types,
    /// so these classes can be used rather than reworking the design.
    /// For more information about numbers classes, see
    /// <a href="https://docs.oracle.com/javase/tutorial/java/data/numberclasses.html">The Numbers Classes</a>.
    /// </summary>
    /// <seealso cref="Number"/>
    /// <seealso cref="IConvertible"/>
    /// <seealso cref="IFormattable"/>
    /// <seealso cref="IComparable"/>
    [DebuggerDisplay("{value}")]
    public sealed class Single : Number, IComparable<Single>, IComparable, IConvertible, IEquatable<Single>
    {
        /// <summary>
        /// The value which the receiver represents.
        /// </summary>
        private readonly float value;

        /// <summary>
        /// Maximum exponent a finite <see cref="float"/> variable may have.
        /// </summary>
        public const int MaxExponent = 127;

        /// <summary>
        /// Minimum exponent a normalized <see cref="float"/> variable may have.
        /// </summary>
        public const int MinExponent = -126;

        /// <summary>
        /// The smallest positive normal value of type <see cref="float"/>, 2<sup>-126</sup>.
        /// It is equal to the value <c>BitConversion.Int32BitsToSingle(0x00800000)</c>.
        /// </summary>
        public const float MinNormal = 1.17549435E-38F;

        /// <summary>
        /// The number of bits needed to represent a <see cref="float"/> in
        /// two's complement form.
        /// </summary>
        public const int Size = 32;

        /// <summary>
        /// Initializes a new instance of the <see cref="Single"/> class.
        /// </summary>
        /// <param name="value">The primitive <see cref="float"/> value to store in the new instance.</param>
        internal Single(float value) // J2N: This has been marked deprecated in JDK 16, so we are marking it internal
        {
            this.value = value;
        }

        // J2N: Removed other overloads because all of the constructors are deprecated in JDK 16

        #region CompareTo

        /// <summary>
        /// Compares this instance to a specified <see cref="Single"/> and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">An <see cref="Single"/> to compare, or <c>null</c>.</param>
        /// <returns>
        /// A signed integer that indicates the relative order of this instance and <paramref name="value"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <term>Description </term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <term>This instance is less than <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <term>This instance is equal to <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <term>This instance is greater than <paramref name="value"/>, or <paramref name="value"/> is <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        /// <remarks>
        /// There are two special cases:
        /// <list type="table">
        ///     <item><description><see cref="float.NaN"/> is equal to <see cref="float.NaN"/> and it is greater
        ///     than any other double value, including <see cref="float.PositiveInfinity"/></description></item>
        ///     <item><description>+0.0f (positive zero) is greater than -0.0f (negative zero).</description></item>
        /// </list>
        /// <para/>
        /// This method implements the <see cref="IComparable{T}"/> interface and performs slightly better than the <see cref="CompareTo(object?)"/>
        /// method because it does not have to convert the <paramref name="value"/> parameter to an object.
        /// </remarks>
        public int CompareTo(Single? value)
        {
            if (value is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537
            return Compare(this.value, value.value);
        }

        /// <summary>
        /// Compares this instance to a specified object and returns an indication of their relative values.
        /// </summary>
        /// <param name="value">An object to compare, or <c>null</c>.</param>
        /// <returns>
        /// A signed integer that indicates the relative order of this instance and <paramref name="value"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <term>Description </term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <term>This instance is less than <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <term>This instance is equal to <paramref name="value"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <term>This instance is greater than <paramref name="value"/>, or <paramref name="value"/> is <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="value"/> is not a <see cref="Single"/>.</exception>
        /// <remarks>
        /// <paramref name="value"/> must be <c>null</c> or an instance of <see cref="Single"/>; otherwise, an exception is thrown.
        /// <para/>
        /// Any instance of <see cref="Single"/>, regardless of its value, is considered greater than <c>null</c>.
        /// <para/>
        /// There are two special cases:
        /// <list type="table">
        ///     <item><description><see cref="float.NaN"/> is equal to <see cref="float.NaN"/> and it is greater
        ///     than any other double value, including <see cref="float.PositiveInfinity"/></description></item>
        ///     <item><description>+0.0f (positive zero) is greater than -0.0f (negative zero).</description></item>
        /// </list>
        /// <para/>
        /// This method is implemented to support the <see cref="IComparable"/> interface.
        /// </remarks>
        public int CompareTo(object? value)
        {
            if (value is null) return 1; // Using 1 if other is null as specified here: https://stackoverflow.com/a/4852537
            if (!(value is Single other))
                throw new ArgumentException(SR.Arg_MustBeSingle);
            return Compare(this.value, other.value);
        }

        #endregion CompareTo

        #region Equals

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="Single"/> object represent the same value.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method implements the <see cref="IEquatable{T}"/> interface, and performs slightly better than
        /// <see cref="Equals(Single?)"/> because it does not have to convert the <paramref name="obj"/> parameter to an object.
        /// <para/>
        /// The <see cref="Equals(Single?)"/> method should be used with caution, because two apparently equivalent values
        /// can be unequal due to the differing precision of the two values. The following example reports that the <see cref="Single"/>
        /// value .3333 and the <see cref="Single"/> returned by dividing 1 by 3 are unequal.
        /// <code>
        /// // Initialize two floats with apparently identical values<br/>
        /// Single float1 = .33333f;<br/>
        /// Single float2 = (float)1 / 3;<br/>
        /// // Compare them for equality<br/>
        /// Console.WriteLine(float1.Equals(float2));    // displays false
        /// </code>
        /// <para/>
        /// Rather than comparing for equality, one technique involves defining an acceptable relative margin of difference
        /// between two values (such as .01% of one of the values). If the absolute value of the difference between the two
        /// values is less than or equal to that margin, the difference is likely to be due to differences in precision and,
        /// therefore, the values are likely to be equal. The following example uses this technique to compare .33333 and 1/3,
        /// the two <see cref="Single"/> values that the previous code example found to be unequal. In this case, the values are equal.
        /// <code>
        /// // Initialize two floats with apparently identical values<br/>
        /// Single float1 = .333333f;<br/>
        /// Single float2 = (float) 1/3;<br/>
        /// // Define the tolerance for variation in their values<br/>
        /// float difference = Math.Abs(float1 * .00001f);<br/>
        /// <br/>
        /// // Compare the values<br/>
        /// // The output to the console indicates that the two values are equal<br/>
        /// if (Math.Abs(float1 - float2) &lt;= difference)<br/>
        /// Console.WriteLine("float1 and float2 are equal.");<br/>
        /// else<br/>
        /// Console.WriteLine("float1 and float2 are unequal.");
        /// </code>
        /// In this case, the values are equal.
        /// <para/>
        /// NOTE: Because <see cref="float.Epsilon"/> defines the minimum expression of a positive value
        /// whose range is near zero, the margin of difference between two similar values must be greater
        /// than <see cref="float.Epsilon"/>. Typically, it is many times greater than <see cref="float.Epsilon"/>.
        /// <para/>
        /// A second technique involves comparing the difference between two floating-point numbers with some absolute value.
        /// If the difference is less than or equal to that absolute value, the numbers are equal. If it is greater, the
        /// numbers are not equal. One alternative is to arbitrarily select an absolute value. This is problematic, however,
        /// because an acceptable margin of difference depends on the magnitude of the <see cref="Single"/> values. A second
        /// alternative takes advantage of a design feature of the floating-point format: The difference between the integer
        /// representation of two floating-point values indicates the number of possible floating-point values that separates
        /// them. For example, the difference between 0.0 and <see cref="float.Epsilon"/> is 1, because <see cref="float.Epsilon"/>
        /// is the smallest representable value when working with a <see cref="Single"/> whose value is zero. The following
        /// example uses this technique to compare .33333 and 1/3, which are the two <see cref="Single"/> values that the
        /// previous code example with the <see cref="Equals(Single?)"/> method found to be unequal. Note that the example
        /// uses <see cref="BitConverter.GetBytes(float)"/> <see cref="BitConverter.ToInt32(byte[], int)"/> method to convert
        /// a double-precision floating-point value to its integer representation.
        /// <code>
        /// using System;<br/>
        /// <br/>
        /// public class Example<br/>
        /// {<br/>
        ///     public static void Main()<br/>
        ///     {<br/>
        ///         Single value1 = .1f * 10f;<br/>
        ///         float value2x = 0f;<br/>
        ///         for (int ctr = 0; ctr &lt; 10; ctr++)<br/>
        ///             value2x += .1f;<br/>
        ///         Single value2 = Single.GetInstance(value2x);<br/>
        ///         <br/>
        ///         Console.WriteLine("{0:R} = {1:R}: {2}", value1, value2,<br/>
        ///                           HasMinimalDifference(value1, value2, 1));<br/>
        ///     }<br/>
        ///     <br/>
        ///     public static bool HasMinimalDifference(float value1, float value2, int units)<br/>
        ///     {<br/>
        ///         byte[] bytes = BitConverter.GetBytes(value1);<br/>
        ///         int lValue1 = BitConverter.ToInt32(bytes, 0);<br/>
        ///         <br/>
        ///         bytes = BitConverter.GetBytes(value2);<br/>
        ///         int lValue2 = BitConverter.ToInt32(bytes, 0);<br/>
        ///         <br/>
        ///         // If the signs are different, return false except for +0 and -0.<br/>
        ///         if ((lValue1 &gt;&gt; 63) != (lValue2 &gt;&gt; 63))<br/>
        ///         {<br/>
        ///             if (value1 == value2)<br/>
        ///                 return true;<br/>
        ///             <br/>
        ///             return false;<br/>
        ///         }<br/>
        ///         <br/>
        ///         int diff = Math.Abs(lValue1 - lValue2);<br/>
        ///         <br/>
        ///         if (diff &lt;= units)<br/>
        ///             return true;<br/>
        ///             <br/>
        ///         return false;<br/>
        ///     }<br/>
        /// }<br/>
        /// // The example displays the following output:<br/>
        /// //        1 = 1.00000012: True
        /// </code>
        /// <para/>
        /// If two <see cref="float.NaN"/> values are tested for equality by calling the <see cref="Equals(Single?)"/>
        /// method, the method returns <c>true</c>. However, if two <see cref="float.NaN"/> values are tested for equality
        /// by using the equality operator, the operator returns <c>false</c>. When you want to determine whether the value
        /// of a <see cref="Single"/> is not a number (NaN), an alternative is to call the <see cref="IsNaN()"/> method.
        /// </remarks>
        public bool Equals(Single? obj)
        {
            if (obj is null) return false;

            return ReferenceEquals(obj, this)
                || (BitConversion.SingleToInt32Bits(this.value) == BitConversion.SingleToInt32Bits(obj.value));
        }

        /// <summary>
        /// Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns><c>true</c> if <paramref name="obj"/> is an instance of <see cref="Single"/> and equals the value of
        /// this instance; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The <see cref="Equals(object?)"/> method should be used with caution, because two apparently equivalent values
        /// can be unequal due to the differing precision of the two values. The following example reports that the <see cref="Single"/>
        /// value .3333 and the <see cref="Single"/> returned by dividing 1 by 3 are unequal.
        /// <code>
        /// // Initialize two singles with apparently identical values<br/>
        /// Single float1 = Single.GetInstance(.33333f);<br/>
        /// Single float2 = Single.GetInstance((float)1/3);<br/>
        /// // Compare them for equality<br/>
        /// Console.WriteLine(float1.Equals(float2));    // displays false
        /// </code>
        /// <para/>
        /// NOTE: Because <see cref="float.Epsilon"/> defines the minimum expression of a positive value
        /// whose range is near zero, the margin of difference between two similar values must be greater
        /// than <see cref="float.Epsilon"/>. Typically, it is many times greater than <see cref="float.Epsilon"/>.
        /// <para/>
        /// If two <see cref="float.NaN"/> values are tested for equality by calling the <see cref="Equals(object?)"/>
        /// method, the method returns <c>true</c>. However, if two <see cref="float.NaN"/> values are tested for equality
        /// by using the equality operator, the operator returns <c>false</c>. When you want to determine whether the value
        /// of a <see cref="Single"/> is not a number (NaN), an alternative is to call the <see cref="IsNaN()"/> method.
        /// </remarks>
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;

            return ReferenceEquals(obj, this)
                || (obj is Single other)
                && (BitConversion.SingleToInt32Bits(this.value) == BitConversion.SingleToInt32Bits(other.value));
        }

        #endregion Equals

        #region SingleToInt32Bits

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "single format" bit
        /// layout.
        ///
        /// <para/>Bit 31 (the bit that is selected by the mask
        /// <c>0x80000000</c>) represents the sign of the floating-point
        /// number.
        /// Bits 30-23 (the bits that are selected by the mask
        /// <c>0x7f800000</c>) represent the exponent.
        /// Bits 22-0 (the bits that are selected by the mask
        /// <c>0x007fffff</c>) represent the significand (sometimes called
        /// the mantissa) of the floating-point number.
        ///
        /// <para/>If the argument is positive infinity, the result is
        /// <c>0x7f800000</c>.
        ///
        /// <para/>If the argument is negative infinity, the result is
        /// <c>0xff800000</c>.
        ///
        /// <para/>If the argument is NaN, the result is <c>0x7fc00000</c>.
        ///
        /// <para/>In all cases, the result is an integer that, when given to the
        /// <see cref="Int32BitsToSingle(int)"/> method, will produce a floating-point
        /// value the same as the argument to <see cref="SingleToInt32Bits(float)"/>
        /// (except all NaN values are collapsed to a single
        /// "canonical" NaN value).
        /// 
        /// <para/>
        /// NOTE: This corresponds to Float.floatToIntBits() in the JDK.
        /// There is no corresponding method in .NET.
        /// </summary>
        /// <param name="value">A floating-point number.</param>
        /// <returns>The bits that represent the floating-point number.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        internal static int SingleToInt32Bits(float value) // J2N: Only used as a proxy for testing purposes
        {
            return BitConversion.SingleToInt32Bits(value);
        }

        #endregion SingleToInt32Bits

        #region SingleToRawInt32Bits

        /// <summary>
        /// Returns a representation of the specified floating-point value
        /// according to the IEEE 754 floating-point "single format" bit
        /// layout, preserving Not-a-Number (NaN) values.
        ///
        /// <para/>Bit 31 (the bit that is selected by the mask
        /// <c>0x80000000</c>) represents the sign of the floating-point
        /// number.
        /// Bits 30-23 (the bits that are selected by the mask
        /// <c>0x7f800000</c>) represent the exponent.
        /// Bits 22-0 (the bits that are selected by the mask
        /// <c>0x007fffff</c>) represent the significand (sometimes called
        /// the mantissa) of the floating-point number.
        ///
        /// <para/>If the argument is positive infinity, the result is
        /// <c>0x7f800000</c>.
        ///
        /// <para/>If the argument is negative infinity, the result is
        /// <c>0xff800000</c>.
        ///
        /// <para/>If the argument is NaN, the result is the integer representing
        /// the actual NaN value.  Unlike the <see cref="SingleToInt32Bits(float)"/>
        /// method, <see cref="SingleToRawInt32Bits(float)"/> does not collapse all the
        /// bit patterns encoding a NaN to a single "canonical"
        /// NaN value.
        ///
        /// <para/>In all cases, the result is an integer that, when given to the
        /// <see cref="Int32BitsToSingle(int)"/> method, will produce a
        /// floating-point value the same as the argument to
        /// <see cref="SingleToRawInt32Bits(float)"/>.
        /// 
        /// <para/>
        /// NOTE: This corresponds to Float.floatToRawIntBits(float) in the JDK
        /// and BitConverter.SingleToInt32Bits(float) in .NET (where implemented).
        /// </summary>
        /// <param name="value">A floating-point number.</param>
        /// <returns>The bits that represent the floating-point number.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        internal static int SingleToRawInt32Bits(float value) // J2N: Only used as a proxy for testing purposes
        {
            return BitConversion.SingleToRawInt32Bits(value);
        }

        #endregion SingleToRawInt32Bits

        #region GetHashCode

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return SingleToInt32Bits(value);
        }

        #endregion GetHashCode

        #region Int32BitsToSingle

        /// <summary>
        /// Returns the <see cref="float"/> value corresponding to a given
        /// bit representation.
        /// The argument is considered to be a representation of a
        /// floating-point value according to the IEEE 754 floating-point
        /// "single format" bit layout.
        ///
        /// <para/>If the argument is <c>0x7f800000</c>, the result is positive
        /// infinity.
        ///
        /// <para/>If the argument is <c>0xff800000</c>, the result is negative
        /// infinity.
        ///
        /// <para/>If the argument is any value in the range
        /// <c>0x7f800001</c> through <c>0x7fffffff</c> or in
        /// the range <c>0xff800001</c> through
        /// <c>0xffffffff</c>, the result is a NaN.  No IEEE 754
        /// floating-point operation provided by .NET can distinguish
        /// between two NaN values of the same type with different bit
        /// patterns.  Distinct values of NaN are only distinguishable by
        /// use of the <see cref="SingleToRawInt32Bits(float)"/> method.
        ///
        /// <para/>In all other cases, let <i>s</i>, <i>e</i>, and <i>m</i> be three
        /// values that can be computed from the argument:
        ///
        /// <code>
        /// int s = ((bits &gt;&gt; 31) == 0) ? 1 : -1;
        /// int e = ((bits &gt;&gt; 23) &amp; 0xff);
        /// int m = (e == 0) ?
        ///                 (bits &amp; 0x7fffff) &lt;&lt; 1 :
        ///                 (bits &amp; 0x7fffff) | 0x800000;
        /// </code>
        ///
        /// Then the floating-point result equals the value of the mathematical
        /// expression <i>s</i>&#183;<i>m</i>&#183;2<sup><i>e</i>-150</sup>.
        ///
        /// <para/>Note that this method may not be able to return a
        /// <see cref="float.NaN"/> with exactly same bit pattern as the
        /// <see cref="int"/> argument.  IEEE 754 distinguishes between two
        /// kinds of NaNs, quiet NaNs and <i>signaling NaNs</i>.  The
        /// differences between the two kinds of NaN are generally not
        /// visible in .NET.  Arithmetic operations on signaling NaNs turn
        /// them into quiet NaNs with a different, but often similar, bit
        /// pattern.  However, on some processors merely copying a
        /// signaling NaN also performs that conversion.  In particular,
        /// copying a signaling NaN to return it to the calling method may
        /// perform this conversion.  So <see cref="Int32BitsToSingle(int)"/> may
        /// not be able to return a <see cref="float"/> with a signaling NaN
        /// bit pattern.  Consequently, for some <see cref="int"/> values,
        /// <c>BitConversion.SingleToRawInt32Bits(BitConversion.Int32BitsToSingle(start))</c> may
        /// <i>not</i> equal <c>start</c>.  Moreover, which
        /// particular bit patterns represent signaling NaNs is platform
        /// dependent; although all NaN bit patterns, quiet or signaling,
        /// must be in the NaN range identified above.
        /// 
        /// <para/>
        /// NOTE: This corresponds to Float.intBitsToFloat(int) in the JDK
        /// and BitConverter.Int32BitsToSingle(int) in .NET (where implemented).
        /// </summary>
        /// <param name="value">The integer to convert.</param>
        /// <returns>A single-precision floating-point value that represents the converted integer.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        internal static float Int32BitsToSingle(int value) // J2N: Only used as a proxy for testing purposes
        {
            return BitConversion.Int32BitsToSingle(value);
        }

        #endregion Int32BitsToSingle

        #region IsFinite

        /// <summary>
        /// Determines whether this object's value is finite (zero, subnormal, or normal).
        /// </summary>
        /// <returns><c>true</c> if the value is finite (zero, subnormal or normal); otherwise <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public bool IsFinite()
        {
            return value.IsFinite();
        }

        #endregion

        #region IsInfinity

        /// <summary>
        /// Returns a value indicating whether this object's value evaluates to negative or positive infinity.
        /// </summary>
        /// <returns><c>true</c> if this object's value evaluates to <see cref="float.PositiveInfinity"/> or
        /// <see cref="float.NegativeInfinity"/>; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public bool IsInfinity()
        {
            return value.IsInfinity();
        }

        #endregion IsInfinity

        #region IsNaN

        /// <summary>
        /// Returns a value that indicates whether this objects's value is not a number
        /// (<see cref="float.NaN"/>).
        /// </summary>
        /// <returns><c>true</c> if this object's' value evaluates to <see cref="float.NaN"/>;
        /// otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public bool IsNaN()
        {
            return value.IsNaN();
        }

        #endregion IsNaN

        #region IsNegative

        /// <summary>
        /// Determines whether this object's value value is negative.
        /// </summary>
        /// <returns><c>true</c> if the value is negative; otherwise, <c>false</c>.</returns>

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public bool IsNegative()
        {
            return value.IsNegative();
        }

        #endregion IsNegative

        #region IsNegativeInfinity

        /// <summary>
        /// Returns a value indicating whether this object's value evaluates to negative
        /// infinity.
        /// </summary>
        /// <returns><c>true</c> if the value evaluates to <see cref="float.NegativeInfinity"/>;
        /// otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public bool IsNegativeInfinity()
        {
            return value.IsNegativeInfinity();
        }

        #endregion IsNegativeInfinity

        #region IsNegativeZero

        /// <summary>
        /// Gets a value indicating whether the current <see cref="float"/> has the value negative zero (<c>-0.0f</c>).
        /// While negative zero is supported by the <see cref="float"/> datatype in .NET, comparisons and string formatting ignore
        /// this feature. This method allows a simple way to check whether the current <see cref="float"/> has the value negative zero.
        /// </summary>
        /// <returns><c>true</c> if the current value represents negative zero; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public bool IsNegativeZero()
        {
            return value.IsNegativeZero();
        }

        #endregion IsNegativeZero

        #region IsNormal

        /// <summary>
        /// Determines whether this object's value specified value is normal.
        /// </summary>
        /// <returns><c>true</c> if the value is normal; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public bool IsNormal()
        {
            return value.IsNormal();
        }

        #endregion IsNormal

        #region IsPositiveInfinity

        /// <summary>
        /// Returns a value indicating whether this object's value evaluates to positive infinity.
        /// </summary>
        /// <returns><c>true</c> if the value evaluates to <see cref="float.PositiveInfinity"/>; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public bool IsPositiveInfinity()
        {
            return value.IsPositiveInfinity();
        }

        #endregion IsPositiveInfinity

        #region IsSubnormal

        /// <summary>
        /// Determines whether this object's value is subnormal.
        /// </summary>
        /// <returns><c>true</c> if the value is subnormal; <c>false</c> otherwise.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public bool IsSubnormal()
        {
            return value.IsSubnormal();
        }

        #endregion IsSubnormal

        #region Parse_CharSequence_IFormatProvider

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to
        /// its single-precision floating-point number equivalent.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string that contains a number to convert.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <returns>A single-precision floating-point number equivalent to the numeric value or symbol specified in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> does not represent a number in a valid format.</exception>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="float.PositiveInfinity"/> or
        /// <see cref="float.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0.
        /// <para/>
        /// This overload is typically used to convert text that can be formatted in a variety of ways to a <see cref="float"/> value. For example,
        /// it can be used to convert the text entered by a user into an HTML text box to a numeric value.
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using a combination of the <see cref="NumberStyle.Float"/> and <see cref="NumberStyle.AllowThousands"/> flags.
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>, or it can contain a string of the form:
        /// <para/>
        /// [ws][sign]integral-digits[.[fractional-digits]][E[sign]exponential-digits][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. Elements that contain the term "digits" consist of a series of numeric characters
        /// ranging from 0 to 9. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+).</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. Runs of <i>integral-digits</i> can be partitioned
        ///         by a group-separator symbol. For example, in some cultures a comma (,) separates groups of thousands. The <i>integral-digits</i> element can be absent
        ///         if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential (scientific) notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// For more information about numeric formats, see the <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">
        /// Formatting Types</a> topic.
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="float"/> data type, the method returns <see cref="float.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="float.MinValue"/> and <see cref="float.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="float.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// <para/>
        /// Some examples of <paramref name="s"/> are "100", "-123,456,789", "123.45e+6", "+500", "5e2", "3.1416", "600.", "-.123", and "-Infinity".
        /// </remarks>
        /// <seealso cref="GetInstance(string, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string, NumberStyle, IFormatProvider?, out float)"/>
        public static float Parse(string s, IFormatProvider? provider)
        {
            return Parse(s, NumberStyle.Float | NumberStyle.AllowThousands, provider);
        }

        #endregion Parse_CharSequence_IFormatProvider

        #region TryParse_CharSequence_Single

        /// <summary>
        /// Converts the string representation of a number to
        /// its single-precision floating-point number equivalent. A return value indicates whether the
        /// conversion succeeded or failed.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <c>provider</c>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <c>provider</c>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string representing a number to convert.</param>
        /// <param name="result">When this method returns, contains the single-precision floating-point number equivalent to the
        /// numeric value or symbol contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the <paramref name="s"/> parameter is <c>null</c> or <see cref="string.Empty"/>.
        /// This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="float.PositiveInfinity"/> or
        /// <see cref="float.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// This overload differs from the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method by returning a Boolean value that indicates
        /// whether the parse operation succeeded instead of returning the parsed numeric value. It eliminates the need to use exception handling to test
        /// for a <see cref="FormatException"/> in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> (the string comparison will use <see cref="StringComparison.OrdinalIgnoreCase"/> comparison rules), or a string of the form:
        /// <para/>
        /// [ws][sign][integral-digits,]integral-digits[.[fractional-digits]][e[sign]exponential-digits][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. Elements that contain the term "digits" consist of a series of numeric characters
        /// ranging from 0 to 9. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+).</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///         if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific group separator symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential (scientific) notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// For more information about numeric formats, see the <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">
        /// Formatting Types</a> topic.
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using a combination of the <see cref="NumberStyle.Float"/> and <see cref="NumberStyle.AllowThousands"/>
        /// flags. This means that white space and thousands separators are allowed but currency symbols are not. To explicitly define the
        /// elements (such as currency symbols, thousands separators, and white space) that can be present in <paramref name="s"/>, use the
        /// <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out float)"/> method overload.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/> object that is initialized for the
        /// current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>. To parse a string using the formatting information of
        /// some other specified culture, use the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out float)"/> method overload.
        /// <para/>
        /// If you pass the <see cref="TryParse(string?, out float)"/> method a string that is created by calling the <see cref="ToString(float, IFormatProvider?)"/>
        /// method and pass <see cref="J2N.Text.StringFormatter.InvariantCulture"/>, the original <see cref="float"/> value is returned. However, because of a loss of precision,
        /// the values using other implementations of <see cref="IFormatProvider"/> may not be equal.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="float"/> data type, the method returns <see cref="float.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="float.MinValue"/> and <see cref="float.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="float.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        /// <seealso cref="GetInstance(string, IFormatProvider?)"/>
        public static bool TryParse([NotNullWhen(true)] string? s, out float result)
        {
            if (s == null)
            {
                result = 0;
                return false;
            }

            return DotNetNumber.TryParseSingle(s, NumberStyle.Float | NumberStyle.AllowThousands, NumberFormatInfo.CurrentInfo, out result);
        }

#if FEATURE_READONLYSPAN

        /// <summary>
        /// Converts the string representation of a number in a character span to its single-precision floating-point number equivalent.
        /// A return value indicates whether the conversion succeeded or failed.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <c>provider</c>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <c>provider</c>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string representing a number to convert.</param>
        /// <param name="result">When this method returns, contains the single-precision floating-point number equivalent to the
        /// numeric value or symbol contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the <paramref name="s"/> parameter is <c>null</c> or <see cref="string.Empty"/>.
        /// This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="float.PositiveInfinity"/> or
        /// <see cref="float.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// This overload differs from the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method by returning a Boolean value that indicates
        /// whether the parse operation succeeded instead of returning the parsed numeric value. It eliminates the need to use exception handling to test
        /// for a <see cref="FormatException"/> in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> (the string comparison will use <see cref="StringComparison.OrdinalIgnoreCase"/> comparison rules), or a string of the form:
        /// <para/>
        /// [ws][sign][integral-digits,]integral-digits[.[fractional-digits]][e[sign]exponential-digits][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. Elements that contain the term "digits" consist of a series of numeric characters
        /// ranging from 0 to 9. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+).</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///         if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific group separator symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential (scientific) notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// For more information about numeric formats, see the <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">
        /// Formatting Types</a> topic.
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using a combination of the <see cref="NumberStyle.Float"/> and <see cref="NumberStyle.AllowThousands"/>
        /// flags. This means that white space and thousands separators are allowed but currency symbols are not. To explicitly define the
        /// elements (such as currency symbols, thousands separators, and white space) that can be present in <paramref name="s"/>, use the
        /// <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out float)"/> method overload.
        /// <para/>
        /// The <paramref name="s"/> parameter is parsed using the formatting information in a <see cref="NumberFormatInfo"/> object that is initialized for the
        /// current system culture. For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>. To parse a string using the formatting information of
        /// some other specified culture, use the <see cref="TryParse(string?, NumberStyle, IFormatProvider?, out float)"/> method overload.
        /// <para/>
        /// If you pass the <see cref="TryParse(ReadOnlySpan{char}, out float)"/> method a string that is created by calling the <see cref="ToString(float, IFormatProvider?)"/>
        /// method and pass <see cref="J2N.Text.StringFormatter.InvariantCulture"/>, the original <see cref="float"/> value is returned. However, because of a loss of precision,
        /// the values using other implementations of <see cref="IFormatProvider"/> may not be equal.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="float"/> data type, the method returns <see cref="float.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="float.MinValue"/> and <see cref="float.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="float.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        /// <seealso cref="Parse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="GetInstance(string, IFormatProvider?)"/>
        public static bool TryParse(ReadOnlySpan<char> s, out float result)
        {
            return DotNetNumber.TryParseSingle(s, NumberStyle.Float | NumberStyle.AllowThousands, NumberFormatInfo.CurrentInfo, out result);
        }

#endif

        #endregion TryParse_CharSequence_Single

        #region Parse_CharSequence_NumberStyle_IFormatProvider

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to
        /// its single-precision floating-point number equivalent.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string that contains a number to convert.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements
        /// that can be present in <paramref name="s"/>. Typical values to specify is <see cref="NumberStyle.Float"/>
        /// combined with <see cref="NumberStyle.AllowThousands"/> or, in the case of hexadecimal numbers,
        /// <see cref="NumberStyle.HexFloat"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <returns>A single-precision floating-point number equivalent to the numeric value or symbol specified in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> does not represent a numeric value.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/>, <see cref="NumberStyle.HexFloat"/>
        /// and <see cref="NumberStyle.AllowTypeSpecifier"/> values.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> has <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.AllowTypeSpecifier"/> values
        /// but not <see cref="NumberStyle.AllowExponent"/>. When type suffix is supplied in a hexadecimal number, exponent (prefixed with a
        /// p or P) is required.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is a combination of <see cref="NumberStyle.AllowCurrencySymbol"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// </exception>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="float.PositiveInfinity"/> or
        /// <see cref="float.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space, thousands separators, and currency symbols) that
        /// are allowed in the s parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/> enumeration.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>. Depending on the value of <paramref name="style"/>,
        /// it can also take one of the following forms:
        /// <para/>
        /// [ws] [$] [sign][integral-digits,]integral-digits[.[fractional-digits]][E[sign]exponential-digits][type][ws]
        /// <para/>
        /// Or, if <paramref name="style"/> includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws] [$] [sign][hexdigits,]hexdigits[.[fractional-hexdigits]][P[sign]exponential-digits][type][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters. White space can appear at the beginning of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowLeadingWhite"/> flag, and it can appear at the end of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowTrailingWhite"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+). The sign can appear at the beginning of <paramref name="s"/> if
        ///             <paramref name="style"/> includes the <see cref="NumberStyle.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="s"/>
        ///             if <paramref name="style"/> includes the <see cref="NumberStyle.AllowTrailingSign"/> flag. Parentheses can be used in <paramref name="s"/>
        ///             to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyle.AllowParentheses"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///             if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the integral part of the number. The <i>hexdigits</i>
        ///         element can be absent if the string contains the <i>fractional-hexdigits</i> element. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific thousands separator. The thousands separator of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowThousands"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number. Fractional digits can appear in
        ///         <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-hexdigits</i>
        ///         </term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the fractional part of the number.
        ///         Fractional digits can appear in <paramref name="s"/> if <paramref name="style"/> includes both the
        ///         <see cref="NumberStyle.AllowDecimalPoint"/> and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>p</i></term>
        ///         <term>The 'p' or 'P' character, which indicates that the hexadecimal value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a hexadecimal number in exponential notation if style includes both the <see cref="NumberStyle.AllowExponent"/>
        ///         and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         [real type suffix](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#real-literals)
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. If <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/>
        ///         flag, specifying the <see cref="NumberStyle.AllowExponent"/> is required. Including an exponent in the string (prefixed with 'p' or 'P') is
        ///         required for the type suffix characters 'f', 'F', 'd', and 'D' since they would otherwise be interpreted as hexadecimal integral or fractional digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="long"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTypeSpecifier"/></term>
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.HexFloat"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with a 'p' or 'P').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Number"/></term>
        ///         <term>The <i>ws</i>, <i>sign</i>, thousands separator (,), and decimal point (.) elements.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Any"/></term>
        ///         <term>All styles, except <paramref name="s"/> cannot represent a hexadecimal number.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value which may include an optional 0x or 0X prefix.
        /// For example, "0xC9AF3.8BD4C". This flag may be combined with any of the flags specified in <see cref="NumberStyle.Float"/> and also
        /// <see cref="NumberStyle.AllowTypeSpecifier"/>. (The <see cref="NumberStyle"/> enumeration has a composite style, <see cref="NumberStyle.HexFloat"/>,
        /// that includes all of those flags except <see cref="NumberStyle.AllowTypeSpecifier"/>.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="float"/> data type, the method returns <see cref="float.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="float.MinValue"/> and <see cref="float.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="float.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        /// <seealso cref="GetInstance(string, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string, NumberStyle, IFormatProvider?, out float)"/>
        public static float Parse(string s, NumberStyle style, IFormatProvider? provider)
        {
            if (s is null)
                throw new ArgumentNullException(nameof(s));
            NumberStyleExtensions.ValidateParseStyleFloatingPoint(style);
            return DotNetNumber.ParseSingle(s, style, NumberFormatInfo.GetInstance(provider));
        }

#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts a character span that contains the string representation of a number in a specified style and culture-specific format to
        /// its single-precision floating-point number equivalent.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A character span that contains the number to convert.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements
        /// that can be present in <paramref name="s"/>. Typical values to specify is <see cref="NumberStyle.Float"/>
        /// combined with <see cref="NumberStyle.AllowThousands"/> or, in the case of hexadecimal numbers,
        /// <see cref="NumberStyle.HexFloat"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <returns>A single-precision floating-point number equivalent to the numeric value or symbol specified in <paramref name="s"/>.</returns>
        /// <exception cref="FormatException"><paramref name="s"/> does not represent a numeric value.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/>, <see cref="NumberStyle.HexFloat"/>
        /// and <see cref="NumberStyle.AllowTypeSpecifier"/> values.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> has <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.AllowTypeSpecifier"/> values
        /// but not <see cref="NumberStyle.AllowExponent"/>. When type suffix is supplied in a hexadecimal number, exponent (prefixed with a
        /// p or P) is required.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is a combination of <see cref="NumberStyle.AllowCurrencySymbol"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// </exception>
        /// <remarks>
        /// If <paramref name="s"/> is out of range of the <see cref="float"/> data type, the method returns <see cref="float.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="float.MinValue"/> and <see cref="float.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="float.MaxValue"/>.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space, thousands separators, and currency symbols) that
        /// are allowed in the s parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/> enumeration.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>. Depending on the value of <paramref name="style"/>,
        /// it can also take one of the following forms:
        /// <para/>
        /// [ws] [$] [sign][integral-digits,]integral-digits[.[fractional-digits]][E[sign]exponential-digits][type][ws]
        /// <para/>
        /// Or, if <paramref name="style"/> includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws] [$] [sign][hexdigits,]hexdigits[.[fractional-hexdigits]][P[sign]exponential-digits][type][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters. White space can appear at the beginning of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowLeadingWhite"/> flag, and it can appear at the end of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowTrailingWhite"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+). The sign can appear at the beginning of <paramref name="s"/> if
        ///             <paramref name="style"/> includes the <see cref="NumberStyle.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="s"/>
        ///             if <paramref name="style"/> includes the <see cref="NumberStyle.AllowTrailingSign"/> flag. Parentheses can be used in <paramref name="s"/>
        ///             to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyle.AllowParentheses"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///             if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the integral part of the number. The <i>hexdigits</i>
        ///         element can be absent if the string contains the <i>fractional-hexdigits</i> element. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific thousands separator. The thousands separator of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowThousands"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number. Fractional digits can appear in
        ///         <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-hexdigits</i>
        ///         </term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the fractional part of the number.
        ///         Fractional digits can appear in <paramref name="s"/> if <paramref name="style"/> includes both the
        ///         <see cref="NumberStyle.AllowDecimalPoint"/> and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>p</i></term>
        ///         <term>The 'p' or 'P' character, which indicates that the hexadecimal value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a hexadecimal number in exponential notation if style includes both the <see cref="NumberStyle.AllowExponent"/>
        ///         and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         [real type suffix](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#real-literals)
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. If <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/>
        ///         flag, specifying the <see cref="NumberStyle.AllowExponent"/> is required. Including an exponent in the string (prefixed with 'p' or 'P') is
        ///         required for the type suffix characters 'f', 'F', 'd', and 'D' since they would otherwise be interpreted as hexadecimal integral or fractional digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="long"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTypeSpecifier"/></term>
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.HexFloat"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with a 'p' or 'P').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Number"/></term>
        ///         <term>The <i>ws</i>, <i>sign</i>, thousands separator (,), and decimal point (.) elements.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Any"/></term>
        ///         <term>All styles, except <paramref name="s"/> cannot represent a hexadecimal number.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value which may include an optional 0x or 0X prefix.
        /// For example, "0xC9AF3.8BD4C". This flag may be combined with any of the flags specified in <see cref="NumberStyle.Float"/> and also
        /// <see cref="NumberStyle.AllowTypeSpecifier"/>. (The <see cref="NumberStyle"/> enumeration has a composite style, <see cref="NumberStyle.HexFloat"/>,
        /// that includes all of those flags except <see cref="NumberStyle.AllowTypeSpecifier"/>.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="float"/> data type, the method returns <see cref="float.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="float.MinValue"/> and <see cref="float.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="float.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        /// <seealso cref="GetInstance(string, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(ReadOnlySpan{char}, NumberStyle, IFormatProvider?, out float)"/>
        public static float Parse(ReadOnlySpan<char> s, NumberStyle style, IFormatProvider? provider)
        {
            NumberStyleExtensions.ValidateParseStyleFloatingPoint(style);
            return DotNetNumber.ParseSingle(s, style, NumberFormatInfo.GetInstance(provider));
        }
#endif

        #endregion Parse_CharSequence_NumberStyle_IFormatProvider

        #region TryParse_CharSequence_NumberStyle_IFormatProvider_Single

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to
        /// its single-precision floating-point number equivalent. A return value indicates whether the
        /// conversion succeeded or failed.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string representing a number to convert.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements
        /// that can be present in <paramref name="s"/>. Typical values to specify is <see cref="NumberStyle.Float"/>
        /// combined with <see cref="NumberStyle.AllowThousands"/> or, in the case of hexadecimal numbers,
        /// <see cref="NumberStyle.HexFloat"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the single-precision floating-point number equivalent to the
        /// numeric value or symbol contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the <paramref name="s"/> parameter is <c>null</c> or <see cref="string.Empty"/>, is not in a format
        /// compliant with <paramref name="style"/>, or if <paramref name="style"/> is not a valid combination of <see cref="NumberStyle"/>
        /// enumeration constants. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/>, <see cref="NumberStyle.HexFloat"/>
        /// and <see cref="NumberStyle.AllowTypeSpecifier"/> values.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> has <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.AllowTypeSpecifier"/> values
        /// but not <see cref="NumberStyle.AllowExponent"/>. When type suffix is supplied in a hexadecimal number, exponent (prefixed with a
        /// p or P) is required.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is a combination of <see cref="NumberStyle.AllowCurrencySymbol"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// </exception>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="float.PositiveInfinity"/> or
        /// <see cref="float.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// This overload differs from the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method by returning a Boolean value that indicates
        /// whether the parse operation succeeded instead of returning the parsed numeric value. It eliminates the need to use exception handling to test
        /// for a <see cref="FormatException"/> in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space, thousands separators, and currency symbols) that
        /// are allowed in the s parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/> enumeration.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>. Depending on the value of <paramref name="style"/>,
        /// it can also take one of the following forms:
        /// <para/>
        /// [ws] [$] [sign][integral-digits,]integral-digits[.[fractional-digits]][E[sign]exponential-digits][type][ws]
        /// <para/>
        /// Or, if <paramref name="style"/> includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws] [$] [sign][hexdigits,]hexdigits[.[fractional-hexdigits]][P[sign]exponential-digits][type][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters. White space can appear at the beginning of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowLeadingWhite"/> flag, and it can appear at the end of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowTrailingWhite"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+). The sign can appear at the beginning of <paramref name="s"/> if
        ///             <paramref name="style"/> includes the <see cref="NumberStyle.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="s"/>
        ///             if <paramref name="style"/> includes the <see cref="NumberStyle.AllowTrailingSign"/> flag. Parentheses can be used in <paramref name="s"/>
        ///             to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyle.AllowParentheses"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///             if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the integral part of the number. The <i>hexdigits</i>
        ///         element can be absent if the string contains the <i>fractional-hexdigits</i> element. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific thousands separator. The thousands separator of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowThousands"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number. Fractional digits can appear in
        ///         <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-hexdigits</i>
        ///         </term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the fractional part of the number.
        ///         Fractional digits can appear in <paramref name="s"/> if <paramref name="style"/> includes both the
        ///         <see cref="NumberStyle.AllowDecimalPoint"/> and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>p</i></term>
        ///         <term>The 'p' or 'P' character, which indicates that the hexadecimal value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a hexadecimal number in exponential notation if style includes both the <see cref="NumberStyle.AllowExponent"/>
        ///         and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         [real type suffix](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#real-literals)
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. If <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/>
        ///         flag, specifying the <see cref="NumberStyle.AllowExponent"/> is required. Including an exponent in the string (prefixed with 'p' or 'P') is
        ///         required for the type suffix characters 'f', 'F', 'd', and 'D' since they would otherwise be interpreted as hexadecimal integral or fractional digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="long"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTypeSpecifier"/></term>
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.HexFloat"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with a 'p' or 'P').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Number"/></term>
        ///         <term>The <i>ws</i>, <i>sign</i>, thousands separator (,), and decimal point (.) elements.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Any"/></term>
        ///         <term>All styles, except <paramref name="s"/> cannot represent a hexadecimal number.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value which may include an optional 0x or 0X prefix.
        /// For example, "0xC9AF3.8BD4C". This flag may be combined with any of the flags specified in <see cref="NumberStyle.Float"/> and also
        /// <see cref="NumberStyle.AllowTypeSpecifier"/>. (The <see cref="NumberStyle"/> enumeration has a composite style, <see cref="NumberStyle.HexFloat"/>,
        /// that includes all of those flags except <see cref="NumberStyle.AllowTypeSpecifier"/>.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="float"/> data type, the method returns <see cref="float.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="float.MinValue"/> and <see cref="float.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="float.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        public static bool TryParse([NotNullWhen(true)] string? s, NumberStyle style, IFormatProvider? provider, out float result)
        {
            NumberStyleExtensions.ValidateParseStyleFloatingPoint(style);

            if (s == null)
            {
                result = 0;
                return false;
            }

            return DotNetNumber.TryParseSingle(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }

#if FEATURE_READONLYSPAN
        /// <summary>
        /// Converts the span representation of a number in a specified style and culture-specific format to
        /// its single-precision floating-point number equivalent. A return value indicates whether the
        /// conversion succeeded or failed.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A read-only character span that contains the number to convert. The span is interpreted using the style specified
        /// by <paramref name="style"/>.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements
        /// that can be present in <paramref name="s"/>. Typical values to specify is <see cref="NumberStyle.Float"/>
        /// combined with <see cref="NumberStyle.AllowThousands"/> or, in the case of hexadecimal numbers,
        /// <see cref="NumberStyle.HexFloat"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <param name="result">When this method returns, contains the single-precision floating-point number equivalent to the
        /// numeric value or symbol contained in <paramref name="s"/>, if the conversion succeeded, or zero if the conversion failed.
        /// The conversion fails if the <paramref name="s"/> parameter is <c>null</c> or <see cref="string.Empty"/>, is not in a format
        /// compliant with <paramref name="style"/>, or if <paramref name="style"/> is not a valid combination of <see cref="NumberStyle"/>
        /// enumeration constants. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
        /// <returns><c>true</c> if <paramref name="s"/> was converted successfully; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/>, <see cref="NumberStyle.HexFloat"/>
        /// and <see cref="NumberStyle.AllowTypeSpecifier"/> values.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> has <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.AllowTypeSpecifier"/> values
        /// but not <see cref="NumberStyle.AllowExponent"/>. When type suffix is supplied in a hexadecimal number, exponent (prefixed with a
        /// p or P) is required.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is a combination of <see cref="NumberStyle.AllowCurrencySymbol"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// </exception>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="float.PositiveInfinity"/> or
        /// <see cref="float.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// This overload differs from the <see cref="Parse(string, NumberStyle, IFormatProvider?)"/> method by returning a Boolean value that indicates
        /// whether the parse operation succeeded instead of returning the parsed numeric value. It eliminates the need to use exception handling to test
        /// for a <see cref="FormatException"/> in the event that <paramref name="s"/> is invalid and cannot be successfully parsed.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space, thousands separators, and currency symbols) that
        /// are allowed in the s parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/> enumeration.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>. Depending on the value of <paramref name="style"/>,
        /// it can also take one of the following forms:
        /// <para/>
        /// [ws] [$] [sign][integral-digits,]integral-digits[.[fractional-digits]][E[sign]exponential-digits][type][ws]
        /// <para/>
        /// Or, if <paramref name="style"/> includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws] [$] [sign][hexdigits,]hexdigits[.[fractional-hexdigits]][P[sign]exponential-digits][type][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters. White space can appear at the beginning of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowLeadingWhite"/> flag, and it can appear at the end of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowTrailingWhite"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+). The sign can appear at the beginning of <paramref name="s"/> if
        ///             <paramref name="style"/> includes the <see cref="NumberStyle.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="s"/>
        ///             if <paramref name="style"/> includes the <see cref="NumberStyle.AllowTrailingSign"/> flag. Parentheses can be used in <paramref name="s"/>
        ///             to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyle.AllowParentheses"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///             if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the integral part of the number. The <i>hexdigits</i>
        ///         element can be absent if the string contains the <i>fractional-hexdigits</i> element. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific thousands separator. The thousands separator of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowThousands"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number. Fractional digits can appear in
        ///         <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-hexdigits</i>
        ///         </term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the fractional part of the number.
        ///         Fractional digits can appear in <paramref name="s"/> if <paramref name="style"/> includes both the
        ///         <see cref="NumberStyle.AllowDecimalPoint"/> and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>p</i></term>
        ///         <term>The 'p' or 'P' character, which indicates that the hexadecimal value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a hexadecimal number in exponential notation if style includes both the <see cref="NumberStyle.AllowExponent"/>
        ///         and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         [real type suffix](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#real-literals)
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. If <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/>
        ///         flag, specifying the <see cref="NumberStyle.AllowExponent"/> is required. Including an exponent in the string (prefixed with 'p' or 'P') is
        ///         required for the type suffix characters 'f', 'F', 'd', and 'D' since they would otherwise be interpreted as hexadecimal integral or fractional digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="long"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTypeSpecifier"/></term>
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.HexFloat"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with a 'p' or 'P').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Number"/></term>
        ///         <term>The <i>ws</i>, <i>sign</i>, thousands separator (,), and decimal point (.) elements.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Any"/></term>
        ///         <term>All styles, except <paramref name="s"/> cannot represent a hexadecimal number.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value which may include an optional 0x or 0X prefix.
        /// For example, "0xC9AF3.8BD4C". This flag may be combined with any of the flags specified in <see cref="NumberStyle.Float"/> and also
        /// <see cref="NumberStyle.AllowTypeSpecifier"/>. (The <see cref="NumberStyle"/> enumeration has a composite style, <see cref="NumberStyle.HexFloat"/>,
        /// that includes all of those flags except <see cref="NumberStyle.AllowTypeSpecifier"/>.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="Parse(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="float"/> data type, the method returns <see cref="float.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="float.MinValue"/> and <see cref="float.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="float.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        public static bool TryParse(ReadOnlySpan<char> s, NumberStyle style, IFormatProvider? provider, out float result)
        {
            NumberStyleExtensions.ValidateParseStyleFloatingPoint(style);
            return DotNetNumber.TryParseSingle(s, style, NumberFormatInfo.GetInstance(provider), out result);
        }
#endif

        #endregion TryParse_CharSequence_NumberStyle_IFormatProvider_Single

        #region ToString

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation of this instance.</returns>
        /// <remarks>
        /// The <see cref="ToString()"/> method formats the current instance in the default ("J", or Java)
        /// format of the current culture. If you want to specify a different format, precision, or culture, use the
        /// other overloads of the <see cref="ToString(string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// The return value can be <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>,
        /// <see cref="NumberFormatInfo.NaNSymbol"/>, or the string of the form:
        /// <para/>
        /// [sign]integral-digits[.[fractional-digits]][E[sign]exponential-digits]
        /// <para/>
        /// Optional elements are framed in square brackets ([ and ]). Elements that contain the term "digits" consist of a series of numeric
        /// characters ranging from 0 to 9. The elements listed in the following table are supported.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign or positive sign symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits specifying the integral part of the number. Integral-digits can be absent if there
        ///         are fractional-digits.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>'.'</i></term>
        ///         <term>A culture-specific decimal point symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i></term>
        ///         <term>A series of digits specifying the fractional part of the number.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>'E'</i></term>
        ///         <term>An uppercase character 'E', indicating exponential (scientific) notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A series of digits specifying an exponent.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// Some examples of the return value are "100", "-123,456,789", "123.45E+6", "500", "3.1416", "600", "-0.123", and "-Infinity".
        /// <para/>
        /// This instance is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// </remarks>
        public override string ToString()
        {
            return DotNetNumber.FormatSingle(value, null, null);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation, using the specified format.
        /// </summary>
        /// <param name="format">A numeric format string.</param>
        /// <returns>The string representation of the current instance as specified by <paramref name="format"/>.</returns>
        /// <remarks>
        /// The <see cref="ToString(string?)"/> method formats the current instance in
        /// a specified format by using the conventions of the current culture. If you want to specify a different format or culture,
        /// use the other overloads of the <see cref="ToString(string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// The return value can be <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>,
        /// <see cref="NumberFormatInfo.NaNSymbol"/>, or the string representation of a number, as specified by <paramref name="format"/>.
        /// <para/>
        /// The <paramref name="format"/> parameter can be any valid standard numeric format specifier except for D, as well as any
        /// combination of custom numeric format specifiers. If <paramref name="format"/> is <c>null</c> or an empty string, the return value for this
        /// instance is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// <para/>
        /// In addition to the formats specified in the above articles, the Java ("J") format and ("X") hexadecimal format are also supported.
        /// <para/>
        /// <h2>Java format specifier ("J")</h2>
        /// <para/>
        /// The ("J") format follows the specification of the Java Language Specification. However, do note that it may not return exactly the same
        /// number representation as the JDK due to precision and rounding differences. The ("J") format is similar to the ("R") format in that
        /// it can be fully round tripped and it ignores the precision specifier. However, do note that on platforms lower than .NET Core 3.0, this
        /// format can only be round-tripped using overloads of <see cref="Parse(string, IFormatProvider?)"/> and <see cref="TryParse(string?, out float)"/>
        /// in J2N. On .NET Core 3.0 and higher overloads of <see cref="float.Parse(string)"/> and <see cref="float.TryParse(string, out float)"/> will
        /// also successfully round trip the number back to the same set of bits.
        /// <para/>
        /// Although you can include a precision specifier, it is ignored. Round trips are given precedence over precision when using this specifier.
        /// The result string is affected by the formatting information of the current <see cref="NumberFormatInfo"/> object. The following table lists
        /// the <see cref="NumberFormatInfo"/> properties that control the formatting of the result string.
        /// <list type="table">
        ///     <listheader>
        ///         <term>NumberFormatInfo property</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberFormatInfo.NegativeSign"/></term>
        ///         <term>Defines the string that indicates that a number is negative.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberFormatInfo.NumberDecimalSeparator"/></term>
        ///         <term>Defines the string that separates integral digits from decimal digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// Note that although this format follows the Java Language Specification, to exactly match the behavior as the JDK, it
        /// must be in the invariant culture by specifying the provider as <see cref="CultureInfo.InvariantCulture"/>,
        /// <see cref="NumberFormatInfo.InvariantInfo"/> or <see cref="J2N.Text.StringFormatter.InvariantCulture"/>.
        /// <para/>
        /// <h2>Hexadecimal format specifier ("X")</h2>
        /// <para/>
        /// The hexadecimal ("X") format is uses the same floating-point hexadecimal format as the JDK and is provided for interoperability
        /// with Java 6 and higher. The hexadecimal form have an analogous syntax to the simple and scaled decimal forms with the following differences:
        /// <list type="number">
        ///     <item><description>Every hexadecimal floating point literal starts with a zero (0) and then an x or X.</description></item>
        ///     <item><description>The digits of the number (but not the exponent part!) also include the hexadecimal digits a through f
        ///     and their uppercase equivalents.</description></item>
        ///     <item><description>The exponent is introduced by the letter p (or P) instead of an e or E. The exponent represents a
        ///     scaling factor that is a power of 2 instead of a power of 10.</description></item>
        /// </list>
        /// <para/>
        /// Here are some examples:
        /// <code>
        /// 0x0.0p0    // this is zero expressed in hexadecimal form (float)<br/>
        /// 0xff.0p19   // this is 255.0 x 2^19 (double)
        /// </code>
        /// See the <see cref="ToHexString(IFormatProvider?)"/> for more information about the format.
        /// <para/>
        /// Note that although this format follows the Java Language Specification, to exactly match the behavior as the JDK, it
        /// must be in the invariant culture by specifying the provider as <see cref="CultureInfo.InvariantCulture"/>,
        /// <see cref="NumberFormatInfo.InvariantInfo"/> or <see cref="J2N.Text.StringFormatter.InvariantCulture"/>.
        /// </remarks>
        public override string ToString(string? format)
        {
            return DotNetNumber.FormatSingle(value, format, null);
        }

        /// <summary>
        /// Converts the numeric value of the current instance to its equivalent string representation using the specified
        /// culture-specific format information.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the current instance as specified by <paramref name="provider"/>.</returns>
        /// <remarks>
        /// The <see cref="ToString(IFormatProvider?)"/> method formats the current instance in
        /// the default ("J") format of a specified culture. If you want to specify a different format or culture, use the
        /// other overloads of the <see cref="ToString(string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// The return value can be <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>,
        /// <see cref="NumberFormatInfo.NaNSymbol"/>, or the string of the form:
        /// <para/>
        /// [sign]integral-digits[.[fractional-digits]][E[sign]exponential-digits]
        /// <para/>
        /// Optional elements are framed in square brackets ([ and ]). Elements that contain the term "digits" consist of a series of numeric
        /// characters ranging from 0 to 9. The elements listed in the following table are supported.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign or positive sign symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits specifying the integral part of the number. Integral-digits can be absent if there
        ///         are fractional-digits.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>'.'</i></term>
        ///         <term>A culture-specific decimal point symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i></term>
        ///         <term>A series of digits specifying the fractional part of the number.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>'E'</i></term>
        ///         <term>An uppercase character 'E', indicating exponential (scientific) notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A series of digits specifying an exponent.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// Some examples of the return value are "100", "-123,456,789", "123.45E+6", "500", "3.1416", "600", "-0.123", and "-Infinity".
        /// <para/>
        /// This instance is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation whose <see cref="IFormatProvider.GetFormat(Type)"/> method returns
        /// a <see cref="NumberFormatInfo"/> object. Typically, provider is a <see cref="CultureInfo"/> object or a <see cref="NumberFormatInfo"/> object.
        /// The <paramref name="provider"/> parameter supplies culture-specific information used in formatting. If provider is <c>null</c>, the return value is formatted using
        /// the <see cref="NumberFormatInfo"/> object for the current culture.
        /// </remarks>
        public override string ToString(IFormatProvider? provider)
        {
            return DotNetNumber.FormatSingle(value, null, provider);
        }

        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation using
        /// the specified format and culture-specific format information.
        /// </summary>
        /// <param name="format">A numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the current instance as specified by <paramref name="format"/>
        /// and <paramref name="provider"/>.</returns>
        /// <remarks>
        /// The <see cref="ToString(string?, IFormatProvider?)"/> method formats the current instance in
        /// a specified format of a specified culture. If you want to specify a different format or culture, use the
        /// other overloads of the <see cref="ToString(string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        /// </list>
        /// The return value can be <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>,
        /// <see cref="NumberFormatInfo.NaNSymbol"/>, or the string representation of a number, as specified by <paramref name="format"/>.
        /// <para/>
        /// The <paramref name="format"/> parameter can be any valid standard numeric format specifier except for D, as well as any
        /// combination of custom numeric format specifiers. If <paramref name="format"/> is <c>null</c> or an empty string, the return value for this
        /// instance is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation whose <see cref="IFormatProvider.GetFormat(Type)"/> method returns
        /// a <see cref="NumberFormatInfo"/> object. Typically, provider is a <see cref="CultureInfo"/> object or a <see cref="NumberFormatInfo"/> object.
        /// The <paramref name="provider"/> parameter supplies culture-specific information used in formatting. If <paramref name="provider"/> is <c>null</c>, the return value
        /// is formatted using the <see cref="NumberFormatInfo"/> object for the current culture.
        /// <para/>
        /// In addition to the formats specified in the above articles, the Java ("J") format and ("X") hexadecimal format are also supported.
        /// <para/>
        /// <h2>Java format specifier ("J")</h2>
        /// <para/>
        /// The ("J") format follows the specification of the Java Language Specification. However, do note that it may not return exactly the same
        /// number representation as the JDK due to precision and rounding differences. The ("J") format is similar to the ("R") format in that
        /// it can be fully round tripped and it ignores the precision specifier. However, do note that on platforms lower than .NET Core 3.0, this
        /// format can only be round-tripped using overloads of <see cref="Parse(string, IFormatProvider?)"/> and <see cref="TryParse(string?, out float)"/>
        /// in J2N. On .NET Core 3.0 and higher overloads of <see cref="float.Parse(string)"/> and <see cref="float.TryParse(string, out float)"/> will
        /// also successfully round trip the number back to the same set of bits.
        /// <para/>
        /// Although you can include a precision specifier, it is ignored. Round trips are given precedence over precision when using this specifier.
        /// The result string is affected by the formatting information of the current <see cref="NumberFormatInfo"/> object. The following table lists
        /// the <see cref="NumberFormatInfo"/> properties that control the formatting of the result string.
        /// <list type="table">
        ///     <listheader>
        ///         <term>NumberFormatInfo property</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberFormatInfo.NegativeSign"/></term>
        ///         <term>Defines the string that indicates that a number is negative.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberFormatInfo.NumberDecimalSeparator"/></term>
        ///         <term>Defines the string that separates integral digits from decimal digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// Note that although this format follows the Java Language Specification, to exactly match the behavior as the JDK, it
        /// must be in the invariant culture by specifying the <paramref name="provider"/> as <see cref="CultureInfo.InvariantCulture"/>,
        /// <see cref="NumberFormatInfo.InvariantInfo"/> or <see cref="J2N.Text.StringFormatter.InvariantCulture"/>.
        /// <para/>
        /// <h2>Hexadecimal format specifier ("X")</h2>
        /// <para/>
        /// The hexadecimal ("X") format is uses the same floating-point hexadecimal format as the JDK and is provided for interoperability
        /// with Java 6 and higher. The hexadecimal form have an analogous syntax to the simple and scaled decimal forms with the following differences:
        /// <list type="number">
        ///     <item><description>Every hexadecimal floating point literal starts with a zero (0) and then an x or X.</description></item>
        ///     <item><description>The digits of the number (but not the exponent part!) also include the hexadecimal digits a through f
        ///     and their uppercase equivalents.</description></item>
        ///     <item><description>The exponent is introduced by the letter p (or P) instead of an e or E. The exponent represents a
        ///     scaling factor that is a power of 2 instead of a power of 10.</description></item>
        /// </list>
        /// <para/>
        /// Here are some examples:
        /// <code>
        /// 0x0.0p0    // this is zero expressed in hexadecimal form (float)<br/>
        /// 0xff.0p19   // this is 255.0 x 2^19 (double)
        /// </code>
        /// See the <see cref="ToHexString(IFormatProvider?)"/> for more information about the format.
        /// <para/>
        /// Note that although this format follows the Java Language Specification, to exactly match the behavior as the JDK, it
        /// must be in the invariant culture by specifying the <paramref name="provider"/> as <see cref="CultureInfo.InvariantCulture"/>,
        /// <see cref="NumberFormatInfo.InvariantInfo"/> or <see cref="J2N.Text.StringFormatter.InvariantCulture"/>.
        /// </remarks>
        public override string ToString(string? format, IFormatProvider? provider)
        {
            return DotNetNumber.FormatSingle(value, format, provider);
        }


        /// <summary>
        /// Converts the numeric <paramref name="value"/> to its equivalent string representation.
        /// </summary>
        /// <param name="value">The <see cref="float"/> to convert.</param>
        /// <returns>The string representation of the value.</returns>
        /// <remarks>
        /// The <see cref="ToString(float)"/> method formats <paramref name="value"/> in the default ("J", or Java)
        /// format of the current culture. If you want to specify a different format, precision, or culture, use the
        /// other overloads of the <see cref="ToString(float, string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// The return value can be <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>,
        /// <see cref="NumberFormatInfo.NaNSymbol"/>, or the string of the form:
        /// <para/>
        /// [sign]integral-digits[.[fractional-digits]][E[sign]exponential-digits]
        /// <para/>
        /// Optional elements are framed in square brackets ([ and ]). Elements that contain the term "digits" consist of a series of numeric
        /// characters ranging from 0 to 9. The elements listed in the following table are supported.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign or positive sign symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits specifying the integral part of the number. Integral-digits can be absent if there
        ///         are fractional-digits.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>'.'</i></term>
        ///         <term>A culture-specific decimal point symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i></term>
        ///         <term>A series of digits specifying the fractional part of the number.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>'E'</i></term>
        ///         <term>An uppercase character 'E', indicating exponential (scientific) notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A series of digits specifying an exponent.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// Some examples of the return value are "100", "-123,456,789", "123.45E+6", "500", "3.1416", "600", "-0.123", and "-Infinity".
        /// <para/>
        /// The value is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// </remarks>
        public static string ToString(float value)
        {
            return DotNetNumber.FormatSingle(value, null, null);
        }

        /// <summary>
        /// Converts the numeric <paramref name="value"/> to its equivalent string representation, using the specified format.
        /// </summary>
        /// <param name="value">The <see cref="float"/> to convert.</param>
        /// <param name="format">A numeric format string.</param>
        /// <returns>The string representation of the value as specified by <paramref name="format"/>.</returns>
        /// <remarks>
        /// The <see cref="ToString(float, string?)"/> method formats <paramref name="value"/> in
        /// a specified format by using the conventions of the current culture. If you want to specify a different format or culture,
        /// use the other overloads of the <see cref="ToString(float, string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// The return value can be <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>,
        /// <see cref="NumberFormatInfo.NaNSymbol"/>, or the string representation of a number, as specified by <paramref name="format"/>.
        /// <para/>
        /// The <paramref name="format"/> parameter can be any valid standard numeric format specifier except for D, as well as any
        /// combination of custom numeric format specifiers. If <paramref name="format"/> is <c>null</c> or an empty string, the return value for this
        /// instance is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// <para/>
        /// In addition to the formats specified in the above articles, the Java ("J") format and ("X") hexadecimal format are also supported.
        /// <para/>
        /// <h2>Java format specifier ("J")</h2>
        /// <para/>
        /// The ("J") format follows the specification of the Java Language Specification. However, do note that it may not return exactly the same
        /// number representation as the JDK due to precision and rounding differences. The ("J") format is similar to the ("R") format in that
        /// it can be fully round tripped and it ignores the precision specifier. However, do note that on platforms lower than .NET Core 3.0, this
        /// format can only be round-tripped using overloads of <see cref="Parse(string, IFormatProvider?)"/> and <see cref="TryParse(string?, out float)"/>
        /// in J2N. On .NET Core 3.0 and higher overloads of <see cref="float.Parse(string)"/> and <see cref="float.TryParse(string, out float)"/> will
        /// also successfully round trip the number back to the same set of bits.
        /// <para/>
        /// Although you can include a precision specifier, it is ignored. Round trips are given precedence over precision when using this specifier.
        /// The result string is affected by the formatting information of the current <see cref="NumberFormatInfo"/> object. The following table lists
        /// the <see cref="NumberFormatInfo"/> properties that control the formatting of the result string.
        /// <list type="table">
        ///     <listheader>
        ///         <term>NumberFormatInfo property</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberFormatInfo.NegativeSign"/></term>
        ///         <term>Defines the string that indicates that a number is negative.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberFormatInfo.NumberDecimalSeparator"/></term>
        ///         <term>Defines the string that separates integral digits from decimal digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// Note that although this format follows the Java Language Specification, to exactly match the behavior as the JDK, it
        /// must be in the invariant culture by specifying the provider as <see cref="CultureInfo.InvariantCulture"/>,
        /// <see cref="NumberFormatInfo.InvariantInfo"/> or <see cref="J2N.Text.StringFormatter.InvariantCulture"/>.
        /// <para/>
        /// <h2>Hexadecimal format specifier ("X")</h2>
        /// <para/>
        /// The hexadecimal ("X") format is uses the same floating-point hexadecimal format as the JDK and is provided for interoperability
        /// with Java 6 and higher. The hexadecimal form have an analogous syntax to the simple and scaled decimal forms with the following differences:
        /// <list type="number">
        ///     <item><description>Every hexadecimal floating point literal starts with a zero (0) and then an x or X.</description></item>
        ///     <item><description>The digits of the number (but not the exponent part!) also include the hexadecimal digits a through f
        ///     and their uppercase equivalents.</description></item>
        ///     <item><description>The exponent is introduced by the letter p (or P) instead of an e or E. The exponent represents a
        ///     scaling factor that is a power of 2 instead of a power of 10.</description></item>
        /// </list>
        /// <para/>
        /// Here are some examples:
        /// <code>
        /// 0x0.0p0    // this is zero expressed in hexadecimal form (float)<br/>
        /// 0xff.0p19   // this is 255.0 x 2^19 (double)
        /// </code>
        /// See the <see cref="ToHexString(IFormatProvider?)"/> for more information about the format.
        /// <para/>
        /// Note that although this format follows the Java Language Specification, to exactly match the behavior as the JDK, it
        /// must be in the invariant culture by specifying the provider as <see cref="CultureInfo.InvariantCulture"/>,
        /// <see cref="NumberFormatInfo.InvariantInfo"/> or <see cref="J2N.Text.StringFormatter.InvariantCulture"/>.
        /// </remarks>
        public static string ToString(float value, string? format)
        {
            return DotNetNumber.FormatSingle(value, format, null);
        }

        /// <summary>
        /// Converts the numeric <paramref name="value"/> to its equivalent string representation using the specified
        /// culture-specific format information.
        /// </summary>
        /// <param name="value">The <see cref="float"/> to convert.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the value as specified by <paramref name="provider"/>.</returns>
        /// <remarks>
        /// The <see cref="ToString(float, IFormatProvider?)"/> method formats <paramref name="value"/> in
        /// the default ("J") format of a specified culture. If you want to specify a different format or culture, use the
        /// other overloads of the <see cref="ToString(float, string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, string?, IFormatProvider?)"/> or <see cref="ToString(string?, IFormatProvider?)"/></term>
        ///     </item>
        /// </list>
        /// The return value can be <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>,
        /// <see cref="NumberFormatInfo.NaNSymbol"/>, or the string of the form:
        /// <para/>
        /// [sign]integral-digits[.[fractional-digits]][E[sign]exponential-digits]
        /// <para/>
        /// Optional elements are framed in square brackets ([ and ]). Elements that contain the term "digits" consist of a series of numeric
        /// characters ranging from 0 to 9. The elements listed in the following table are supported.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign or positive sign symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits specifying the integral part of the number. Integral-digits can be absent if there
        ///         are fractional-digits.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>'.'</i></term>
        ///         <term>A culture-specific decimal point symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i></term>
        ///         <term>A series of digits specifying the fractional part of the number.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>'E'</i></term>
        ///         <term>An uppercase character 'E', indicating exponential (scientific) notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A series of digits specifying an exponent.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// Some examples of the return value are "100", "-123,456,789", "123.45E+6", "500", "3.1416", "600", "-0.123", and "-Infinity".
        /// <para/>
        /// The value is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation whose <see cref="IFormatProvider.GetFormat(Type)"/> method returns
        /// a <see cref="NumberFormatInfo"/> object. Typically, provider is a <see cref="CultureInfo"/> object or a <see cref="NumberFormatInfo"/> object.
        /// The <paramref name="provider"/> parameter supplies culture-specific information used in formatting. If provider is <c>null</c>, the return value is formatted using
        /// the <see cref="NumberFormatInfo"/> object for the current culture.
        /// </remarks>
        public static string ToString(float value, IFormatProvider? provider)
        {
            return DotNetNumber.FormatSingle(value, null, provider);
        }

        /// <summary>
        /// Converts the numeric <paramref name="value"/> to its equivalent string representation using
        /// the specified format and culture-specific format information.
        /// </summary>
        /// <param name="value">The <see cref="float"/> to convert.</param>
        /// <param name="format">A numeric format string.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The string representation of the value as specified by <paramref name="format"/>
        /// and <paramref name="provider"/>.</returns>
        /// <remarks>
        /// The <see cref="ToString(float, string?, IFormatProvider?)"/> method formats <paramref name="value"/> in
        /// a specified format of a specified culture. If you want to specify a different format or culture, use the
        /// other overloads of the <see cref="ToString(float, string?, IFormatProvider?)"/> method, as follows:
        /// <list type="table">
        ///     <listheader>
        ///         <term>To use format</term>
        ///         <term>For culture</term>
        ///         <term>Use the overload</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float)"/> or <see cref="ToString()"/></term>
        ///     </item>
        ///     <item>
        ///         <term>Default ("J") format</term>
        ///         <term>A specific culture</term>
        ///         <term><see cref="ToString(float, IFormatProvider?)"/> or <see cref="ToString(IFormatProvider?)"/></term>
        ///     </item>
        ///     <item>
        ///         <term>A specific format or precision</term>
        ///         <term>Default (current) culture</term>
        ///         <term><see cref="ToString(float, string?)"/> or <see cref="ToString(string?)"/></term>
        ///     </item>
        /// </list>
        /// The return value can be <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>,
        /// <see cref="NumberFormatInfo.NaNSymbol"/>, or the string representation of a number, as specified by <paramref name="format"/>.
        /// <para/>
        /// The <paramref name="format"/> parameter can be any valid standard numeric format specifier except for D, as well as any
        /// combination of custom numeric format specifiers. If <paramref name="format"/> is <c>null</c> or an empty string, the return value for this
        /// instance is formatted with the Java numeric format specifier ("J").
        /// <para/>
        /// .NET provides extensive formatting support, which is described in greater detail in the following formatting topics:
        /// <list type="bullet">
        ///     <item><description>For more information about numeric format specifiers, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings">Standard Numeric Format Strings</a>
        ///     and <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">Custom Numeric Format Strings</a>.
        ///     </description></item>
        ///     <item><description>For more information about formatting, see
        ///     <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">Formatting Types</a>.</description></item>
        /// </list>
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation whose <see cref="IFormatProvider.GetFormat(Type)"/> method returns
        /// a <see cref="NumberFormatInfo"/> object. Typically, provider is a <see cref="CultureInfo"/> object or a <see cref="NumberFormatInfo"/> object.
        /// The <paramref name="provider"/> parameter supplies culture-specific information used in formatting. If <paramref name="provider"/> is <c>null</c>, the return value
        /// is formatted using the <see cref="NumberFormatInfo"/> object for the current culture.
        /// <para/>
        /// In addition to the formats specified in the above articles, the Java ("J") format and ("X") hexadecimal format are also supported.
        /// <para/>
        /// <h2>Java format specifier ("J")</h2>
        /// <para/>
        /// The ("J") format follows the specification of the Java Language Specification. However, do note that it may not return exactly the same
        /// number representation as the JDK due to precision and rounding differences. The ("J") format is similar to the ("R") format in that
        /// it can be fully round tripped and it ignores the precision specifier. However, do note that on platforms lower than .NET Core 3.0, this
        /// format can only be round-tripped using overloads of <see cref="Parse(string, IFormatProvider?)"/> and <see cref="TryParse(string?, out float)"/>
        /// in J2N. On .NET Core 3.0 and higher overloads of <see cref="float.Parse(string)"/> and <see cref="float.TryParse(string, out float)"/> will
        /// also successfully round trip the number back to the same set of bits.
        /// <para/>
        /// Although you can include a precision specifier, it is ignored. Round trips are given precedence over precision when using this specifier.
        /// The result string is affected by the formatting information of the current <see cref="NumberFormatInfo"/> object. The following table lists
        /// the <see cref="NumberFormatInfo"/> properties that control the formatting of the result string.
        /// <list type="table">
        ///     <listheader>
        ///         <term>NumberFormatInfo property</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberFormatInfo.NegativeSign"/></term>
        ///         <term>Defines the string that indicates that a number is negative.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberFormatInfo.NumberDecimalSeparator"/></term>
        ///         <term>Defines the string that separates integral digits from decimal digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// Note that although this format follows the Java Language Specification, to exactly match the behavior as the JDK, it
        /// must be in the invariant culture by specifying the <paramref name="provider"/> as <see cref="CultureInfo.InvariantCulture"/>,
        /// <see cref="NumberFormatInfo.InvariantInfo"/> or <see cref="J2N.Text.StringFormatter.InvariantCulture"/>.
        /// <para/>
        /// <h2>Hexadecimal format specifier ("X")</h2>
        /// <para/>
        /// The hexadecimal ("X") format is uses the same floating-point hexadecimal format as the JDK and is provided for interoperability
        /// with Java 6 and higher. The hexadecimal form have an analogous syntax to the simple and scaled decimal forms with the following differences:
        /// <list type="number">
        ///     <item><description>Every hexadecimal floating point literal starts with a zero (0) and then an x or X.</description></item>
        ///     <item><description>The digits of the number (but not the exponent part!) also include the hexadecimal digits a through f
        ///     and their uppercase equivalents.</description></item>
        ///     <item><description>The exponent is introduced by the letter p (or P) instead of an e or E. The exponent represents a
        ///     scaling factor that is a power of 2 instead of a power of 10.</description></item>
        /// </list>
        /// <para/>
        /// Here are some examples:
        /// <code>
        /// 0x0.0p0    // this is zero expressed in hexadecimal form (float)<br/>
        /// 0xff.0p19   // this is 255.0 x 2^19 (double)
        /// </code>
        /// See the <see cref="ToHexString(IFormatProvider?)"/> for more information about the format.
        /// <para/>
        /// Note that although this format follows the Java Language Specification, to exactly match the behavior as the JDK, it
        /// must be in the invariant culture by specifying the <paramref name="provider"/> as <see cref="CultureInfo.InvariantCulture"/>,
        /// <see cref="NumberFormatInfo.InvariantInfo"/> or <see cref="J2N.Text.StringFormatter.InvariantCulture"/>.
        /// </remarks>
        public static string ToString(float value, string? format, IFormatProvider? provider)
        {
            return DotNetNumber.FormatSingle(value, format, provider);
        }

        #endregion ToString

        #region Compare

        /// <summary>
        /// Compares the two specified <see cref="float"/> values. There are two special cases:
        /// <list type="table">
        ///     <item><description><see cref="float.NaN"/> is equal to <see cref="float.NaN"/> and it is greater
        ///     than any other double value, including <see cref="float.PositiveInfinity"/></description></item>
        ///     <item><description>+0.0f (positive zero) is greater than -0.0f (negative zero).</description></item>
        /// </list>
        /// </summary>
        /// <param name="floatA">The first value to compare.</param>
        /// <param name="floatB">The second value to compare.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relationship between the two comparands.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Return Value</term>
        ///         <term>Description </term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero</term>
        ///         <term><paramref name="floatA"/> is less than <paramref name="floatB"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero</term>
        ///         <term><paramref name="floatA"/> equal to <paramref name="floatB"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero</term>
        ///         <term><paramref name="floatA"/> is greater than <paramref name="floatB"/>.</term>
        ///     </item>
        /// </list>
        /// </returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int Compare(float floatA, float floatB)
        {
            return JCG.Comparer<float>.Default.Compare(floatA, floatB);
        }

        #endregion

        #region GetInstance (ValueOf)

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to
        /// its <see cref="Single"/> instance equivalent.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <c>style</c> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string that contains a number to convert.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <returns>An immutable <see cref="Single"/> instance equivalent to the numeric value or symbol specified in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> does not represent a number in a valid format.</exception>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="float.PositiveInfinity"/> or
        /// <see cref="float.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0.
        /// <para/>
        /// This overload is typically used to convert text that can be formatted in a variety of ways to a <see cref="float"/> value. For example,
        /// it can be used to convert the text entered by a user into an HTML text box to a numeric value.
        /// <para/>
        /// The <paramref name="s"/> parameter is interpreted using a combination of the <see cref="NumberStyle.Float"/> and <see cref="NumberStyle.AllowThousands"/> flags.
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>, or it can contain a string of the form:
        /// <para/>
        /// [ws][sign]integral-digits[.[fractional-digits]][E[sign]exponential-digits][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. Elements that contain the term "digits" consist of a series of numeric characters
        /// ranging from 0 to 9. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+).</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. Runs of <i>integral-digits</i> can be partitioned
        ///         by a group-separator symbol. For example, in some cultures a comma (,) separates groups of thousands. The <i>integral-digits</i> element can be absent
        ///         if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential (scientific) notation.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// For more information about numeric formats, see the <a href="https://docs.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">
        /// Formatting Types</a> topic.
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="GetInstance(string, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="float"/> data type, the method returns <see cref="float.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="float.MinValue"/> and <see cref="float.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="float.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// <para/>
        /// Some examples of <paramref name="s"/> are "100", "-123,456,789", "123.45e+6", "+500", "5e2", "3.1416", "600.", "-.123", and "-Infinity".
        /// </remarks>
        /// <seealso cref="Parse(string, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string, NumberStyle, IFormatProvider?, out float)"/>
        public static Single GetInstance(string s, IFormatProvider? provider)
        {
            return GetInstance(Parse(s, NumberStyle.Float, provider));
        }

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to
        /// its <see cref="Single"/> equivalent.
        /// <para/>
        /// Usage Note: To approximately match the behavior of the JDK for decimal numbers, use <see cref="NumberStyle.Float"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// To approximately match the behavior of the JDK for hexadecimal numbers, use <see cref="NumberStyle.HexFloat"/> combined with
        /// <see cref="NumberStyle.AllowTypeSpecifier"/> for <paramref name="style"/> and <see cref="NumberFormatInfo.InvariantInfo"/> for <paramref name="provider"/>.
        /// These options will parse any number that is supported by Java, but the hex specifier, exponent, and type suffix are all optional.
        /// </summary>
        /// <param name="s">A string that contains a number to convert.</param>
        /// <param name="style">A bitwise combination of enumeration values that indicates the style elements
        /// that can be present in <paramref name="s"/>. Typical values to specify is <see cref="NumberStyle.Float"/>
        /// combined with <see cref="NumberStyle.AllowThousands"/> or, in the case of hexadecimal numbers,
        /// <see cref="NumberStyle.HexFloat"/>.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="s"/>.</param>
        /// <returns>An immutable <see cref="Single"/> instance equivalent to the numeric value or symbol specified in <paramref name="s"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
        /// <exception cref="FormatException"><paramref name="s"/> does not represent a numeric value.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="style"/> is not a <see cref="NumberStyle"/> value.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is not a combination of <see cref="NumberStyle.AllowHexSpecifier"/>, <see cref="NumberStyle.HexFloat"/>
        /// and <see cref="NumberStyle.AllowTypeSpecifier"/> values.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> has <see cref="NumberStyle.AllowHexSpecifier"/> and <see cref="NumberStyle.AllowTypeSpecifier"/> values
        /// but not <see cref="NumberStyle.AllowExponent"/>. When type suffix is supplied in a hexadecimal number, exponent (prefixed with a
        /// p or P) is required.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="style"/> is a combination of <see cref="NumberStyle.AllowCurrencySymbol"/> and <see cref="NumberStyle.AllowTypeSpecifier"/>.
        /// </exception>
        /// <remarks>
        /// In .NET Core 3.0 and later, values that are too large to represent are rounded to <see cref="float.PositiveInfinity"/> or
        /// <see cref="float.NegativeInfinity"/> as required by the IEEE 754 specification. In prior versions, including .NET Framework, parsing a
        /// value that was too large to represent resulted in failure. This implementation uses the new parser, but patches it back to .NET Framework 4.0
        /// and upgrades it to support Java's hexadecimal floating point format.
        /// <para/>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space, thousands separators, and currency symbols) that
        /// are allowed in the s parameter for the parse operation to succeed. It must be a combination of bit flags from the <see cref="NumberStyle"/> enumeration.
        /// <para/>
        /// The <paramref name="s"/> parameter can contain <see cref="NumberFormatInfo.PositiveInfinitySymbol"/>, <see cref="NumberFormatInfo.NegativeInfinitySymbol"/>, or
        /// <see cref="NumberFormatInfo.NaNSymbol"/> for the culture specified by <paramref name="provider"/>. Depending on the value of <paramref name="style"/>,
        /// it can also take one of the following forms:
        /// <para/>
        /// [ws] [$] [sign][integral-digits,]integral-digits[.[fractional-digits]][E[sign]exponential-digits][type][ws]
        /// <para/>
        /// Or, if <paramref name="style"/> includes <see cref="NumberStyle.AllowHexSpecifier"/>:
        /// <para/>
        /// [ws] [$] [sign][hexdigits,]hexdigits[.[fractional-hexdigits]][P[sign]exponential-digits][type][ws]
        /// <para/>
        /// Elements framed in square brackets ([ and ]) are optional. The following table describes each element.
        /// <para/>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Element</term>
        ///         <term>Description</term>
        ///     </listheader>
        ///     <item>
        ///         <term><i>ws</i></term>
        ///         <term>A series of white-space characters. White space can appear at the beginning of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowLeadingWhite"/> flag, and it can appear at the end of <paramref name="s"/> if style includes the
        ///             <see cref="NumberStyle.AllowTrailingWhite"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>$</i></term>
        ///         <term>A culture-specific currency symbol. Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/>
        ///             and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object returned by the
        ///             <see cref="IFormatProvider.GetFormat(Type?)"/> method of the <paramref name="provider"/> parameter. The current culture's currency symbol can
        ///             appear in <paramref name="s"/> if style includes the <see cref="NumberStyle.AllowCurrencySymbol"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>sign</i></term>
        ///         <term>A negative sign symbol (-) or a positive sign symbol (+). The sign can appear at the beginning of <paramref name="s"/> if
        ///             <paramref name="style"/> includes the <see cref="NumberStyle.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="s"/>
        ///             if <paramref name="style"/> includes the <see cref="NumberStyle.AllowTrailingSign"/> flag. Parentheses can be used in <paramref name="s"/>
        ///             to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyle.AllowParentheses"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>integral-digits</i></term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the integral part of the number. The <i>integral-digits</i> element can be absent
        ///             if the string contains the <i>fractional-digits</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>hexdigits</i></term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the integral part of the number. The <i>hexdigits</i>
        ///         element can be absent if the string contains the <i>fractional-hexdigits</i> element. Hexadecimal digits can appear in <paramref name="s"/>
        ///         if <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>,</i></term>
        ///         <term>A culture-specific thousands separator. The thousands separator of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowThousands"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>.</i></term>
        ///         <term>A culture-specific decimal point symbol. The decimal point symbol of the culture specified by <paramref name="provider"/>
        ///         can appear in <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-digits</i>
        ///         </term>
        ///         <term>A series of digits ranging from 0 to 9 that specify the fractional part of the number. Fractional digits can appear in
        ///         <paramref name="s"/> if <paramref name="style"/> includes the <see cref="NumberStyle.AllowDecimalPoint"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>fractional-hexdigits</i>
        ///         </term>
        ///         <term>A sequence of hexadecimal digits from 0 through f, or 0 through F that specify the fractional part of the number.
        ///         Fractional digits can appear in <paramref name="s"/> if <paramref name="style"/> includes both the
        ///         <see cref="NumberStyle.AllowDecimalPoint"/> and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>e</i></term>
        ///         <term>The 'e' or 'E' character, which indicates that the value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a number in exponential notation if style includes the <see cref="NumberStyle.AllowExponent"/> flag.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>p</i></term>
        ///         <term>The 'p' or 'P' character, which indicates that the hexadecimal value is represented in exponential notation. The <paramref name="s"/>
        ///         parameter can represent a hexadecimal number in exponential notation if style includes both the <see cref="NumberStyle.AllowExponent"/>
        ///         and <see cref="NumberStyle.AllowHexSpecifier"/> flags.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>exponential-digits</i></term>
        ///         <term>A sequence of decimal digits from 0 through 9 that specify an exponent.</term>
        ///     </item>
        ///     <item>
        ///         <term><i>type</i></term>
        ///         <term>The 'f', 'F', 'd', 'D', 'm' or 'M' character, which is the
        ///         [real type suffix](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/lexical-structure#real-literals)
        ///         of the number as specified in the C# language specification. The type suffix can appear in <paramref name="s"/> if <paramref name="style"/>
        ///         includes the <see cref="NumberStyle.AllowTypeSpecifier"/> flag. If <paramref name="style"/> includes the <see cref="NumberStyle.AllowHexSpecifier"/>
        ///         flag, specifying the <see cref="NumberStyle.AllowExponent"/> is required. Including an exponent in the string (prefixed with 'p' or 'P') is
        ///         required for the type suffix characters 'f', 'F', 'd', and 'D' since they would otherwise be interpreted as hexadecimal integral or fractional digits.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// NOTE: Any terminating NUL (U+0000) characters in <paramref name="s"/> are ignored by the parsing operation, regardless of
        /// the value of the <paramref name="style"/> argument.
        /// <para/>
        /// A string with decimal digits only (which corresponds to the <see cref="NumberStyle.None"/> flag) always parses successfully.
        /// Most of the remaining <see cref="NumberStyle"/> members control elements that may be but are not required to be present in
        /// this input string. The following table indicates how individual <see cref="NumberStyle"/> members affect the elements that may
        /// be present in <paramref name="s"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Non-composite NumberStyle values</term>
        ///         <term>Elements permitted in <paramref name="s"/> in addition to digits</term>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="NumberStyle.None"/></term>
        ///         <term>Decimal digits only.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowDecimalPoint"/></term>
        ///         <term>The decimal point (.) and <i>fractional-digits</i> elements. However, <i>fractional-digits</i> must consist
        ///         of only one or more 0 digits or the method returns <c>false</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowExponent"/></term>
        ///         <term>The <paramref name="s"/> parameter can also use exponential notation. If <paramref name="s"/> represents a
        ///         number in exponential notation, it must represent an integer within the range of the <see cref="long"/> data type
        ///         without a non-zero, fractional component.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingWhite"/></term>
        ///         <term>The <i>ws</i> element at the beginning of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingWhite"/></term>
        ///         <term>The <i>ws</i> element at the end of <paramref name="s"/>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowLeadingSign"/></term>
        ///         <term>A sign can appear before <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTrailingSign"/></term>
        ///         <term>A sign can appear after <i>digits</i>.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowParentheses"/></term>
        ///         <term>The <i>sign</i> element in the form of parentheses enclosing the numeric value.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowThousands"/></term>
        ///         <term>The thousands separator (,) element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowCurrencySymbol"/></term>
        ///         <term>The <i>$</i> element.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.AllowTypeSpecifier"/></term>
        ///         <term>The literal <i>type</i> suffix used in the literal identifier syntax of C# or Java.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Currency"/></term>
        ///         <term>All elements. The <paramref name="s"/> parameter cannot represent a hexadecimal
        ///         number or a number in exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Float"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with an 'e' or 'E').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.HexFloat"/></term>
        ///         <term>The <i>ws</i> element at the beginning or end of <paramref name="s"/>, sign at
        ///         the beginning of <paramref name="s"/>, and the decimal point (.) symbol. The <paramref name="s"/>
        ///         parameter can also use exponential notation (prefixed with a 'p' or 'P').</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Number"/></term>
        ///         <term>The <i>ws</i>, <i>sign</i>, thousands separator (,), and decimal point (.) elements.</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="NumberStyle.Any"/></term>
        ///         <term>All styles, except <paramref name="s"/> cannot represent a hexadecimal number.</term>
        ///     </item>
        /// </list>
        /// <para/>
        /// The <see cref="NumberStyle"/> enum can be converted to the .NET <see cref="NumberStyles"/> enum by using the
        /// <see cref="NumberStyleExtensions.ToNumberStyles(NumberStyle)"/> extension method.
        /// Similarly, <see cref="NumberStyles"/> enum can be converted to the J2N <see cref="NumberStyle"/> enum by using
        /// the <see cref="NumberStyleExtensions.ToNumberStyle(NumberStyles)"/> extension method.
        /// <para/>
        /// If the <see cref="NumberStyle.AllowHexSpecifier"/> flag is used, <paramref name="s"/> must be a hexadecimal value which may include an optional 0x or 0X prefix.
        /// For example, "0xC9AF3.8BD4C". This flag may be combined with any of the flags specified in <see cref="NumberStyle.Float"/> and also
        /// <see cref="NumberStyle.AllowTypeSpecifier"/>. (The <see cref="NumberStyle"/> enumeration has a composite style, <see cref="NumberStyle.HexFloat"/>,
        /// that includes all of those flags except <see cref="NumberStyle.AllowTypeSpecifier"/>.)
        /// <para/>
        /// The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation, such as a <see cref="CultureInfo"/> object,
        /// a <see cref="NumberFormatInfo"/> object or a <see cref="J2N.Text.StringFormatter"/> object, whose <see cref="IFormatProvider.GetFormat(Type?)"/>
        /// method returns a <see cref="NumberFormatInfo"/> object. The <see cref="NumberFormatInfo"/> object provides culture-specific information about
        /// the format of <paramref name="s"/>. When the <see cref="GetInstance(string, NumberStyle, IFormatProvider)"/> method is invoked, it calls the provider parameter's
        /// <see cref="IFormatProvider.GetFormat(Type?)"/> method and passes it a <see cref="Type"/> object that represents the <see cref="NumberFormatInfo"/> type.
        /// The <see cref="IFormatProvider.GetFormat(Type?)"/> method then returns the <see cref="NumberFormatInfo"/> object that provides information about the
        /// format of the <paramref name="s"/> parameter. There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting
        /// information to the parse operation:
        /// <list type="bullet">
        ///     <item><description>You can pass a <see cref="CultureInfo"/> object that represents the culture that supplies formatting information. Its
        ///     <see cref="IFormatProvider.GetFormat(Type?)"/> method returns the <see cref="NumberFormatInfo"/> object that provides numeric formatting
        ///     information for that culture.</description></item>
        ///     <item><description>You can pass the actual <see cref="NumberFormatInfo"/> object that provides numeric formatting information. (Its
        ///     implementation of <see cref="IFormatProvider.GetFormat(Type?)"/> just returns itself.)</description></item>
        ///     <item><description>You can pass a custom object that implements <see cref="IFormatProvider"/>. Its <see cref="IFormatProvider.GetFormat(Type?)"/>
        ///     method instantiates and returns the <see cref="NumberFormatInfo"/> object that provides formatting information.</description></item>
        /// </list>
        /// If <paramref name="provider"/> is <c>null</c> or a <see cref="NumberFormatInfo"/> object cannot be obtained, the <see cref="NumberFormatInfo"/>
        /// object for the current culture is used.
        /// <para/>
        /// If <paramref name="s"/> is out of range of the <see cref="float"/> data type, the method returns <see cref="float.NegativeInfinity"/> if
        /// <paramref name="s"/> is less than <see cref="float.MinValue"/> and <see cref="float.PositiveInfinity"/> if <paramref name="s"/> is greater
        /// than <see cref="float.MaxValue"/>.
        /// <para/>
        /// If a separator is encountered in the <paramref name="s"/> parameter during a parse operation, and the applicable currency or number
        /// decimal and group separators are the same, the parse operation assumes that the separator is a decimal separator rather than a group
        /// separator. For more information about separators, see <see cref="NumberFormatInfo.CurrencyDecimalSeparator"/>,
        /// <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, <see cref="NumberFormatInfo.CurrencyGroupSeparator"/>, and
        /// <see cref="NumberFormatInfo.NumberGroupSeparator"/>.
        /// </remarks>
        /// <seealso cref="Parse(string, NumberStyle, IFormatProvider?)"/>
        /// <seealso cref="TryParse(string, NumberStyle, IFormatProvider?, out float)"/>
        public static Single GetInstance(string s, NumberStyle style, IFormatProvider? provider)
        {
            return GetInstance(Parse(s, style, provider));
        }

        /// <summary>
        /// Returns an immutable <see cref="Single"/> instance for the specified <paramref name="value"/>.
        /// <para/>
        /// Usage Note: This is the same operation as Float.valueOf() in the JDK.
        /// </summary>
        /// <param name="value">The <see cref="float"/> value the returned instance represents.</param>
        /// <returns>An immutable <see cref="Single"/> instance containing the <paramref name="value"/>.</returns>
        public static Single GetInstance(float value)
        {
            return new Single(value);
        }

        #endregion GetInstance (ValueOf)

        #region ToHexString

        /// <summary>
        /// Returns a hexadecimal string representation of the current instance. All characters mentioned below are ASCII characters.
        /// <list type="bullet">
        ///     <item><description>If the current value is <see cref="float.NaN"/>, the result is <see cref="NumberFormatInfo.NumberDecimalSeparator"/>
        ///         of the <paramref name="provider"/>. </description></item>
        ///     <item><description>Otherwise, the result is a string that represents the sign and magnitude of the current value. If the sign
        ///         is negative, it is prefixed by <see cref="NumberFormatInfo.NegativeSign"/> of the <paramref name="provider"/>; if the
        ///         sign is positive, no sign character appears in the result. As for the magnitude <i>m</i>: </description>
        ///         <list type="bullet">
        ///             <item><description>If <i>m</i> is positive infinity, it is represented by <see cref="NumberFormatInfo.PositiveInfinitySymbol"/> of the <paramref name="provider"/>;
        ///                 if <i>m</i> is negative infinity, it is represented by <see cref="NumberFormatInfo.NegativeInfinitySymbol"/> of the <paramref name="provider"/>.</description></item>
        ///             <item><description>If <i>m</i> is zero, it is represented by the string "0x0.0p0"; thus, negative zero produces the result
        ///                 "-0x0.0p0" and positive zero produces the result "0x0.0p0". The negative symbol is represented by <see cref="NumberFormatInfo.NegativeSign"/>
        ///                 and decimal separator character is represented by <see cref="NumberFormatInfo.NumberDecimalSeparator"/>.</description></item>
        ///             <item><description>If <i>m</i> is a <see cref="float"/> value with a normalized representation, substrings are used to represent the
        ///                 significand and exponent fields. The significand is represented by the characters "0x1" followed by <see cref="NumberFormatInfo.NumberDecimalSeparator"/>,
        ///                 followed by a lowercase hexadecimal representation of the rest of the significand as a fraction. Trailing zeros in the hexadecimal representation
        ///                 are removed unless all the digits are zero, in which case a single zero is used. Next, the exponent is represented by "p"
        ///                 followed by a decimal string of the unbiased exponent as if produced by a call to <see cref="int.ToString()"/> with invariant culture on the exponent value. </description></item>
        ///             <item><description>If <i>m</i> is a <see cref="float"/> value with a subnormal representation, the significand is represented by the characters "0x0"
        ///                 followed by <see cref="NumberFormatInfo.NumberDecimalSeparator"/>, followed by a hexadecimal representation of the rest of the significand as a fraction.
        ///                 Trailing zeros in the hexadecimal representation are removed. Next, the exponent is represented by "p-126". Note that there must be at least one nonzero
        ///                 digit in a subnormal significand. </description></item>
        ///         </list>
        ///     </item>
        /// </list>
        /// <para/>
        /// The value of <see cref="NumberFormatInfo.NumberNegativePattern"/> of <paramref name="provider"/> is ignored.
        /// <para/>
        /// <h3>Examples (using <see cref="NumberFormatInfo.InvariantInfo"/>)</h3>
        /// <list type="table">
        ///     <listheader>
        ///         <term>Floating-point Value</term>
        ///         <term>Hexadecimal String</term>
        ///     </listheader>
        ///     <item>
        ///         <term>1.0</term>
        ///         <term>0x1.0p0</term>
        ///     </item>
        ///     <item>
        ///         <term>-1.0</term>
        ///         <term>-0x1.0p0</term>
        ///     </item>
        ///     <item>
        ///         <term>2.0</term>
        ///         <term>0x1.0p1</term>
        ///     </item>
        ///     <item>
        ///         <term>3.0</term>
        ///         <term>0x1.8p1</term>
        ///     </item>
        ///     <item>
        ///         <term>0.5</term>
        ///         <term>0x1.0p-1</term>
        ///     </item>
        ///     <item>
        ///         <term>0.25</term>
        ///         <term>0x1.0p-2</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="float.MaxValue"/></term>
        ///         <term>0x1.fffffep127</term>
        ///     </item>
        ///     <item>
        ///         <term>Minimum Normal Value</term>
        ///         <term>0x1.0p-126</term>
        ///     </item>
        ///     <item>
        ///         <term>Maximum Subnormal Value</term>
        ///         <term>0x0.fffffep-126</term>
        ///     </item>
        ///     <item>
        ///         <term><see cref="float.Epsilon"/></term>
        ///         <term>0x0.000002p-126</term>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A hex string representing the current instance.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public string ToHexString(IFormatProvider? provider)
        {
            return value.ToHexString(provider);
        }

        #endregion ToHexString

        // J2N: Support implicit conversion

        /// <inheritdoc/>
        public static implicit operator float(Single value) => value.value;
        /// <inheritdoc/>
        public static implicit operator Single(float value) => GetInstance(value);

        #region IConvertible implementation

        /// <inheritdoc/>
        public override byte ToByte()
        {
            return (byte)value;
        }

        /// <inheritdoc/>
        public override double ToDouble()
        {
            return value;
        }

        /// <inheritdoc/>
        public override short ToInt16()
        {
            return (short)value;
        }

        /// <inheritdoc/>
        public override int ToInt32()
        {
            return (int)value;
        }

        /// <inheritdoc/>
        public override long ToInt64()
        {
            return (long)value;
        }

        /// <inheritdoc/>
        [CLSCompliant(false)]
        public override sbyte ToSByte()
        {
            return (sbyte)value;
        }

        /// <inheritdoc/>
        public override float ToSingle()
        {
            return value;
        }

        //
        // IConvertible implementation
        //

        /// <summary>
        /// Returns the <see cref="TypeCode"/> for value type <see cref="float"/>.
        /// </summary>
        /// <returns>The enumerated constant, <see cref="TypeCode.Single"/>.</returns>
        public TypeCode GetTypeCode()
        {
            return TypeCode.Single;
        }

        bool IConvertible.ToBoolean(IFormatProvider? provider)
        {
            return Convert.ToBoolean(value);
        }

        char IConvertible.ToChar(IFormatProvider? provider)
        {
            throw new InvalidCastException(J2N.SR.Format(SR.InvalidCast_FromTo, "Single", "Char"));
        }

        sbyte IConvertible.ToSByte(IFormatProvider? provider)
        {
            return Convert.ToSByte(value);
        }

        byte IConvertible.ToByte(IFormatProvider? provider)
        {
            return Convert.ToByte(value);
        }

        short IConvertible.ToInt16(IFormatProvider? provider)
        {
            return Convert.ToInt16(value);
        }

        ushort IConvertible.ToUInt16(IFormatProvider? provider)
        {
            return Convert.ToUInt16(value);
        }

        int IConvertible.ToInt32(IFormatProvider? provider)
        {
            return Convert.ToInt32(value);
        }

        uint IConvertible.ToUInt32(IFormatProvider? provider)
        {
            return Convert.ToUInt32(value);
        }

        long IConvertible.ToInt64(IFormatProvider? provider)
        {
            return Convert.ToInt64(value);
        }

        ulong IConvertible.ToUInt64(IFormatProvider? provider)
        {
            return Convert.ToUInt64(value);
        }

        float IConvertible.ToSingle(IFormatProvider? provider)
        {
            return value;
        }

        double IConvertible.ToDouble(IFormatProvider? provider)
        {
            return Convert.ToDouble(value);
        }

        decimal IConvertible.ToDecimal(IFormatProvider? provider)
        {
            return Convert.ToDecimal(value);
        }

        DateTime IConvertible.ToDateTime(IFormatProvider? provider)
        {
            throw new InvalidCastException(J2N.SR.Format(SR.InvalidCast_FromTo, "Single", "DateTime"));
        }

        object IConvertible.ToType(Type type, IFormatProvider? provider)
        {
            return /*Convert.*/DefaultToType((IConvertible)this.value, type, provider);
        }

        #endregion
    }
}
