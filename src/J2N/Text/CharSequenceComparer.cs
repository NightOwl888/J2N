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

using J2N.Buffers;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace J2N.Text
{
    /// <summary>
    /// Represents a character sequence comparison operation that uses specific case and culture-based ordinal comparison rules.
    /// </summary>
    public abstract class CharSequenceComparer :
        IComparer, IEqualityComparer,
        IComparer<ICharSequence>, IEqualityComparer<ICharSequence>
    {
        private static readonly CharSequenceComparer ordinal = new OrdinalComparer();

        /// <summary>
        /// Gets a <see cref="CharSequenceComparer"/> object that performs a case-sensitive ordinal string comparison.
        /// </summary>
        public static CharSequenceComparer Ordinal => ordinal;

        /// <summary>
        /// Compares two objects and returns an indication of their relative sort order.
        /// </summary>
        /// <param name="x">An object to compare to <paramref name="y"/>.</param>
        /// <param name="y">An object to compare to <paramref name="x"/>.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Value</term>
        ///         <term>Meaning</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero </term>
        ///         <term><paramref name="x"/> precedes y in the sort order. -or- <paramref name="x"/> is <c>null</c> and <paramref name="y"/> is not <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero </term>
        ///         <term><paramref name="x"/> is equal to <paramref name="y"/>. -or- <paramref name="x"/> and <paramref name="y"/> are both <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero </term>
        ///         <term><paramref name="x"/> follows <paramref name="y"/> in the sort order. -or- <paramref name="y"/> is <c>null</c> and <paramref name="x"/> is not <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        public virtual int Compare(object? x, object? y)
        {
            if (x == y) return 0;
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
            if (x == null) return -1;
            if (y == null) return 1;
#endif

            if (x is ICharSequence sa)
            {
                if (y is ICharSequence otherCharSequence)
                    return Compare(sa, otherCharSequence);
                else if (y is string otherString)
                    return Compare(sa, otherString);
                else if (y is StringBuilder otherStringBuilder)
                    return Compare(sa, otherStringBuilder);
                else if (y is char[] otherCharArray)
                    return Compare(sa, otherCharArray);
                else if (y is StringBuffer otherStringBuffer)
                    return Compare(sa, otherStringBuffer.builder);
            }

#if FEATURE_BROKEN_CHARSEQENCE_EXCEPTION_HANDLING
            if (x is IComparable comparable)
                return comparable.CompareTo(y);

            throw new ArgumentException($"Argument '{nameof(x)}' must implement IComparable");
#else
            ThrowHelper.ThrowArgumentException(ExceptionResource.NotSupported_StringComparison);
            return 0; // unreachable
#endif
        }

        /// <summary>
        /// Compares two <see cref="ICharSequence"/>s and returns an indication of their relative sort order.
        /// </summary>
        /// <param name="x">A <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">An <see cref="ICharSequence"/> to compare to <paramref name="x"/>.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Value</term>
        ///         <term>Meaning</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero </term>
        ///         <term><paramref name="x"/> precedes y in the sort order. -or- <paramref name="x"/> is <c>null</c> and <paramref name="y"/> is not <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero </term>
        ///         <term><paramref name="x"/> is equal to <paramref name="y"/>. -or- <paramref name="x"/> and <paramref name="y"/> are both <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero </term>
        ///         <term><paramref name="x"/> follows <paramref name="y"/> in the sort order. -or- <paramref name="y"/> is <c>null</c> and <paramref name="x"/> is not <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        public abstract int Compare(ICharSequence? x, ICharSequence? y);

        /// <summary>
        /// Compares two character sequences and returns an indication of their relative sort order.
        /// </summary>
        /// <param name="x">A <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">A <see cref="T:char[]"/> to compare to <paramref name="x"/>.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Value</term>
        ///         <term>Meaning</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero </term>
        ///         <term><paramref name="x"/> precedes y in the sort order. -or- <paramref name="x"/> is <c>null</c> and <paramref name="y"/> is not <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero </term>
        ///         <term><paramref name="x"/> is equal to <paramref name="y"/>. -or- <paramref name="x"/> and <paramref name="y"/> are both <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero </term>
        ///         <term><paramref name="x"/> follows <paramref name="y"/> in the sort order. -or- <paramref name="y"/> is <c>null</c> and <paramref name="x"/> is not <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        public abstract int Compare(ICharSequence? x, char[]? y);

        /// <summary>
        /// Compares two character sequences and returns an indication of their relative sort order.
        /// </summary>
        /// <param name="x">A <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">A <see cref="StringBuilder"/> to compare to <paramref name="x"/>.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Value</term>
        ///         <term>Meaning</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero </term>
        ///         <term><paramref name="x"/> precedes y in the sort order. -or- <paramref name="x"/> is <c>null</c> and <paramref name="y"/> is not <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero </term>
        ///         <term><paramref name="x"/> is equal to <paramref name="y"/>. -or- <paramref name="x"/> and <paramref name="y"/> are both <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero </term>
        ///         <term><paramref name="x"/> follows <paramref name="y"/> in the sort order. -or- <paramref name="y"/> is <c>null</c> and <paramref name="x"/> is not <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        public abstract int Compare(ICharSequence? x, StringBuilder? y);

        /// <summary>
        /// Compares two character sequences and returns an indication of their relative sort order.
        /// </summary>
        /// <param name="x">A <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">A <see cref="string"/> to compare to <paramref name="x"/>.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Value</term>
        ///         <term>Meaning</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero </term>
        ///         <term><paramref name="x"/> precedes y in the sort order. -or- <paramref name="x"/> is <c>null</c> and <paramref name="y"/> is not <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero </term>
        ///         <term><paramref name="x"/> is equal to <paramref name="y"/>. -or- <paramref name="x"/> and <paramref name="y"/> are both <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero </term>
        ///         <term><paramref name="x"/> follows <paramref name="y"/> in the sort order. -or- <paramref name="y"/> is <c>null</c> and <paramref name="x"/> is not <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        public abstract int Compare(ICharSequence? x, string? y);

        /// <summary>
        /// Compares two character sequences and returns an indication of their relative sort order.
        /// </summary>
        /// <param name="x">A <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">A <see cref="CharArrayCharSequence"/> to compare to <paramref name="x"/>.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Value</term>
        ///         <term>Meaning</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero </term>
        ///         <term><paramref name="x"/> precedes y in the sort order. -or- <paramref name="x"/> is <c>null</c> and <paramref name="y"/> is not <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero </term>
        ///         <term><paramref name="x"/> is equal to <paramref name="y"/>. -or- <paramref name="x"/> and <paramref name="y"/> are both <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero </term>
        ///         <term><paramref name="x"/> follows <paramref name="y"/> in the sort order. -or- <paramref name="y"/> is <c>null</c> and <paramref name="x"/> is not <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        public virtual int Compare(ICharSequence? x, CharArrayCharSequence? y) => Compare(x, y?.Value);

        /// <summary>
        /// Compares two character sequences and returns an indication of their relative sort order.
        /// </summary>
        /// <param name="x">A <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">A <see cref="StringBuilderCharSequence"/> to compare to <paramref name="x"/>.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Value</term>
        ///         <term>Meaning</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero </term>
        ///         <term><paramref name="x"/> precedes y in the sort order. -or- <paramref name="x"/> is <c>null</c> and <paramref name="y"/> is not <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero </term>
        ///         <term><paramref name="x"/> is equal to <paramref name="y"/>. -or- <paramref name="x"/> and <paramref name="y"/> are both <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero </term>
        ///         <term><paramref name="x"/> follows <paramref name="y"/> in the sort order. -or- <paramref name="y"/> is <c>null</c> and <paramref name="x"/> is not <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        public virtual int Compare(ICharSequence? x, StringBuilderCharSequence? y) => Compare(x, y?.Value);

        /// <summary>
        /// Compares two character sequences and returns an indication of their relative sort order.
        /// </summary>
        /// <param name="x">A <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">A <see cref="StringCharSequence"/> to compare to <paramref name="x"/>.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Value</term>
        ///         <term>Meaning</term>
        ///     </listheader>
        ///     <item>
        ///         <term>Less than zero </term>
        ///         <term><paramref name="x"/> precedes y in the sort order. -or- <paramref name="x"/> is <c>null</c> and <paramref name="y"/> is not <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Zero </term>
        ///         <term><paramref name="x"/> is equal to <paramref name="y"/>. -or- <paramref name="x"/> and <paramref name="y"/> are both <c>null</c>.</term>
        ///     </item>
        ///     <item>
        ///         <term>Greater than zero </term>
        ///         <term><paramref name="x"/> follows <paramref name="y"/> in the sort order. -or- <paramref name="y"/> is <c>null</c> and <paramref name="x"/> is not <c>null</c>.</term>
        ///     </item>
        /// </list>
        /// </returns>
        public virtual int Compare(ICharSequence? x, StringCharSequence? y) => Compare(x, y?.Value);

        /// <summary>
        /// Indicates whether two objects or character sequences are equal.
        /// </summary>
        /// <param name="x">An <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">An <see cref="ICharSequence"/> to compare to <paramref name="x"/>.</param>
        /// <returns><c>true</c> if <paramref name="x"/> and <paramref name="y"/> refer to the same object,
        /// or <paramref name="x"/> and <paramref name="y"/> both contain the same sequence of characters,
        /// or both <paramref name="x"/> and <paramref name="y"/> are <c>null</c>; otherwise <c>false</c>.</returns>
        public virtual new bool Equals(object? x, object? y)
        {
            if (x == y) return true;
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
            if (x is null || y is null) return false;
#endif

            if (x is ICharSequence sa)
            {
                if (y is ICharSequence otherCharSequence)
                    return Equals(sa, otherCharSequence);
                else if (y is string otherString)
                    return Equals(sa, otherString);
                else if (y is StringBuilder otherStringBuilder)
                    return Equals(sa, otherStringBuilder);
                else if (y is char[] otherCharArray)
                    return Equals(sa, otherCharArray);
                else if (y is StringBuffer otherStringBuffer)
                    return Equals(sa, otherStringBuffer.builder);
            }

#if FEATURE_BROKEN_CHARSEQENCE_EXCEPTION_HANDLING
            return x.Equals(y);
#else
            return false;
#endif

        }

        /// <summary>
        /// Indicates whether two character sequences are equal.
        /// </summary>
        /// <param name="x">A <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">A <see cref="ICharSequence"/> to compare to <paramref name="x"/>.</param>
        /// <returns><c>true</c> if <paramref name="x"/> and <paramref name="y"/> refer to the same object,
        /// or <paramref name="x"/> and <paramref name="y"/> both contain the same sequence of characters,
        /// or both <paramref name="x"/> and <paramref name="y"/> are <c>null</c>; otherwise <c>false</c>.</returns>
        public abstract bool Equals(ICharSequence? x, ICharSequence? y);

        /// <summary>
        /// Indicates whether two character sequences are equal.
        /// </summary>
        /// <param name="x">A <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">A <see cref="T:char[]"/> to compare to <paramref name="x"/>.</param>
        /// <returns><c>true</c> if <paramref name="x"/> and <paramref name="y"/> refer to the same object,
        /// or <paramref name="x"/> and <paramref name="y"/> both contain the same sequence of characters,
        /// or both <paramref name="x"/> and <paramref name="y"/> are <c>null</c>; otherwise <c>false</c>.</returns>
        public abstract bool Equals(ICharSequence? x, char[]? y);

        /// <summary>
        /// Indicates whether two character sequences are equal.
        /// </summary>
        /// <param name="x">A <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">A <see cref="StringBuilder"/> to compare to <paramref name="x"/>.</param>
        /// <returns><c>true</c> if <paramref name="x"/> and <paramref name="y"/> refer to the same object,
        /// or <paramref name="x"/> and <paramref name="y"/> both contain the same sequence of characters,
        /// or both <paramref name="x"/> and <paramref name="y"/> are <c>null</c>; otherwise <c>false</c>.</returns>
        public abstract bool Equals(ICharSequence? x, StringBuilder? y);

        /// <summary>
        /// Indicates whether two character sequences are equal.
        /// </summary>
        /// <param name="x">A <see cref="ICharSequence"/> to compare to <paramref name="y"/>.</param>
        /// <param name="y">A <see cref="string"/> to compare to <paramref name="x"/>.</param>
        /// <returns><c>true</c> if <paramref name="x"/> and <paramref name="y"/> refer to the same object,
        /// or <paramref name="x"/> and <paramref name="y"/> both contain the same sequence of characters,
        /// or both <paramref name="x"/> and <paramref name="y"/> are <c>null</c>; otherwise <c>false</c>.</returns>
        public abstract bool Equals(ICharSequence? x, string? y);

        /// <summary>
        /// Gets the hash code for the specified object.
        /// </summary>
        /// <param name="obj">An object.</param>
        /// <returns>A 32-bit signed hash code calculated from the value of the <paramref name="obj"/> parameter,
        /// or <see cref="int.MaxValue"/> if <paramref name="obj"/> is <c>null</c>.</returns>
        public virtual int GetHashCode(object? obj)
        {
            if (obj is null)
                return int.MaxValue;

            if (obj is string otherString)
                return GetHashCode(otherString);
            else if (obj is StringBuilder otherStringBuilder)
                return GetHashCode(otherStringBuilder);
            else if (obj is char[] otherCharArray)
                return GetHashCode(otherCharArray);
            else if (obj is StringCharSequence otherStringCharSequence)
                return GetHashCode(otherStringCharSequence);
            else if (obj is StringBuilderCharSequence otherStringBuilderCharSequence)
                return GetHashCode(otherStringBuilderCharSequence);
            else if (obj is CharArrayCharSequence otherCharArrayCharSequence)
                return GetHashCode(otherCharArrayCharSequence);
            else if (obj is StringBuffer otherStringBuffer)
                return GetHashCode(otherStringBuffer.builder);

#if FEATURE_BROKEN_CHARSEQENCE_EXCEPTION_HANDLING
            return obj.GetHashCode();
#else
            ThrowHelper.ThrowArgumentException(ExceptionResource.NotSupported_StringComparison);
            return 0; // unreachable
#endif
        }

        /// <summary>
        /// Gets the hash code for the specified character sequence.
        /// </summary>
        /// <param name="obj">A character sequence.</param>
        /// <returns>A 32-bit signed hash code calculated from the value of the <paramref name="obj"/> parameter,
        /// or <see cref="int.MaxValue"/> if <paramref name="obj"/> is <c>null</c>.</returns>
        public abstract int GetHashCode(ICharSequence? obj);

        /// <summary>
        /// Gets the hash code for the specified character sequence.
        /// </summary>
        /// <param name="obj">A character sequence.</param>
        /// <returns>A 32-bit signed hash code calculated from the value of the <paramref name="obj"/> parameter,
        /// or <see cref="int.MaxValue"/> if <paramref name="obj"/> is <c>null</c>.</returns>
        public abstract int GetHashCode(char[]? obj);

        /// <summary>
        /// Gets the hash code for the specified character sequence.
        /// </summary>
        /// <param name="obj">A character sequence.</param>
        /// <returns>A 32-bit signed hash code calculated from the value of the <paramref name="obj"/> parameter,
        /// or <see cref="int.MaxValue"/> if <paramref name="obj"/> is <c>null</c>.</returns>
        public abstract int GetHashCode(StringBuilder? obj);

        /// <summary>
        /// Gets the hash code for the specified character sequence.
        /// </summary>
        /// <param name="obj">A character sequence.</param>
        /// <returns>A 32-bit signed hash code calculated from the value of the <paramref name="obj"/> parameter,
        /// or <see cref="int.MaxValue"/> if <paramref name="obj"/> is <c>null</c>.</returns>
        public abstract int GetHashCode(string? obj);


        private class OrdinalComparer : CharSequenceComparer
        {
            private const int CharStackBufferSize = 64;


            public override int Compare(object? x, object? y)
            {
                if (x == y) return 0;
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                if (x == null) return -1;
                if (y == null) return 1;
#endif

                if (x is SynchronizedTextBuilder stb1)
                    return Compare(stb1, y, ExceptionArgument.y);
                if (y is SynchronizedTextBuilder stb2)
                    return Compare(x, stb2, ExceptionArgument.x);

#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                if (x is SynchronizedTextBuilderCharSequence stbcs1)
                    return Compare(stbcs1, y, ExceptionArgument.y);
                if (y is SynchronizedTextBuilderCharSequence stbcs2)
                    return Compare(x, stbcs2, ExceptionArgument.x);
#else
                if (x is SynchronizedTextBuilderCharSequence stbcs1)
                    return Compare(stbcs1.Value, y, ExceptionArgument.y);
                if (y is SynchronizedTextBuilderCharSequence stbcs2)
                    return Compare(x, stbcs2.Value, ExceptionArgument.x);
#endif

                if (x is StringBuffer sbuf1)
                    return Compare(sbuf1, y, ExceptionArgument.y);
                if (y is StringBuffer sbuf2)
                    return Compare(x, sbuf2, ExceptionArgument.x);

                if (x is ICharSequence cs1)
                {
#if !FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                    if (y is null)
                    {
                        return !cs1.HasValue ? 0 : 1;
                    }
#endif

                    if (y is ISpannable<char> otherSpannable)
                    {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                        if (!cs1.HasValue) return otherSpannable.HasValue ? 0 : -1;
                        if (!otherSpannable.HasValue) return 1;
#else
                        if (!cs1.HasValue) return !otherSpannable.HasValue ? 0 : -1;
                        if (!otherSpannable.HasValue) return 1;
#endif

                        return Compare(cs1, otherSpannable.AsSpan());
                    }

                    if (y is ICharSequence otherCharSequence)
                        return Compare(cs1, otherCharSequence);
                    else if (y is string otherString)
                        return Compare(cs1, otherString);
                    else if (y is StringBuilder otherStringBuilder)
                        return Compare(cs1, otherStringBuilder);
                    else if (y is char[] otherCharArray)
                        return Compare(cs1, otherCharArray);
                }

                if (y is ICharSequence cs2)
                {
#if !FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                    if (x is null)
                    {
                        return !cs2.HasValue ? 0 : -1;
                    }
#endif

                    if (x is ISpannable<char> otherSpannable)
                    {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                        if (!otherSpannable.HasValue) return cs2.HasValue ? 0 : -1;
                        if (!cs2.HasValue) return 1;
#else
                        if (!otherSpannable.HasValue) return !cs2.HasValue ? 0 : -1;
                        if (!cs2.HasValue) return 1;
#endif

                        return Compare(otherSpannable.AsSpan(), cs2);
                    }
                    // x is ICharSequence tried above

                    if (x is string otherString)
                        return Compare(otherString, cs2);
                    else if (x is StringBuilder otherStringBuilder)
                        return J2N.Globalization.Ordinal.CompareString(otherStringBuilder, cs2);
                    else if (x is char[] otherCharArray)
                        return Compare(otherCharArray, cs2);
                }

                if (x is ISpannable<char> spannable1)
                {
                    if (y is null)
                    {
                        return !spannable1.HasValue ? 0 : 1;
                    }

                    if (y is ISpannable<char> yspannable2)
                    {
                        if (!spannable1.HasValue) return !yspannable2.HasValue ? 0 : -1;
                        if (!yspannable2.HasValue) return 1;

                        return spannable1.AsSpan().SequenceCompareTo(yspannable2.AsSpan());
                    }

                    if (!spannable1.HasValue) return -1; // y cannot be null here
                    if (y is string ys2)
                        return spannable1.AsSpan().SequenceCompareTo(ys2);
                    if (y is char[] yca2)
                        return spannable1.AsSpan().SequenceCompareTo(yca2);
                    if (y is StringBuilder ysb2)
                        return J2N.Globalization.Ordinal.CompareString(spannable1.AsSpan(), ysb2);
                }
                if (y is ISpannable<char> spannable2)
                {
                    if (x is null) return !spannable2.HasValue ? 0 : -1;
                    if (!spannable2.HasValue) return 1;
                    // x is ISpannable<char> tried above
                    if (x is string xs1)
                        return xs1.AsSpan().SequenceCompareTo(spannable2.AsSpan());
                    if (x is char[] xca1)
                        return xca1.AsSpan().SequenceCompareTo(spannable2.AsSpan());
                    if (x is StringBuilder xsb1)
                        return J2N.Globalization.Ordinal.CompareString(xsb1, spannable2.AsSpan());
                }

                if (x is string s1)
                {
                    if (y is null) return 1;
                    if (y is string ys2)
                        return s1.AsSpan().SequenceCompareTo(ys2);
                    if (y is char[] yca2)
                        return s1.AsSpan().SequenceCompareTo(yca2);
                    if (y is StringBuilder ysb2)
                        return J2N.Globalization.Ordinal.CompareString(s1, ysb2);
                }
                if (y is string s2)
                {
                    if (x is null) return -1;
                    // x is string tried above
                    if (x is char[] xca2)
                        return xca2.AsSpan().SequenceCompareTo(s2);
                    if (x is StringBuilder xsb2)
                        return J2N.Globalization.Ordinal.CompareString(xsb2, s2);
                }

                if (x is char[] ca1)
                {
                    if (y is null) return 1;
                    // y is string tried above
                    if (y is char[] yca2)
                        return ca1.AsSpan().SequenceCompareTo(yca2);
                    if (y is StringBuilder ysb2)
                        return J2N.Globalization.Ordinal.CompareString(ca1, ysb2);
                }
                if (y is char[] ca2)
                {
                    if (x is null) return -1;
                    // x is string tried above
                    // x is char[] tried above
                    if (x is StringBuilder xsb2)
                        return J2N.Globalization.Ordinal.CompareString(xsb2, ca2);
                }

                if (x is StringBuilder sb1)
                {
                    if (y is null) return 1;
                    // y is string tried above
                    // y is char[] tried above
                    if (y is StringBuilder ysb2)
                        return J2N.Globalization.Ordinal.CompareString(sb1, ysb2);
                }
                if (y is StringBuilder sb2)
                {
                    if (x is null) return -1;
                    // x is string tried above
                    // x is char[] tried above
                    // x is StringBuilder tried above
                }

#if FEATURE_BROKEN_CHARSEQENCE_EXCEPTION_HANDLING
                if (x is IComparable comparable)
                    return comparable.CompareTo(y);

                throw new ArgumentException($"Argument '{nameof(x)}' must implement IComparable");
#else
                ThrowHelper.ThrowArgumentException(ExceptionResource.NotSupported_StringComparison);
                return 0; // unreachable
#endif
            }

#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
            private int Compare(SynchronizedTextBuilderCharSequence? x, object? y, ExceptionArgument exceptionArgument)
            {
                if (x == y) return 0;

                if (x is null || !x.HasValue)
                {
                    if (y is null)
                        return 0;

                    if (y is ICharSequence cs)
                        return cs.HasValue ? 0 : -1;

                    if (y is ISpannable<char> spannable)
                        return spannable.HasValue ? 0 : -1;

                    // For non-ICharSequence/non-ISpannable types, preserve
                    // the historical object-comparison behavior.
                }


                return Compare(x?.Value, y, exceptionArgument);
            }

            private int Compare(object? x, SynchronizedTextBuilderCharSequence? y, ExceptionArgument exceptionArgument)
            {
                if (x == y) return 0;

                if (x is null)
                    return y is null || y.HasValue ? 0 : -1;

                if (x is ICharSequence xCharSequence)
                {
                    if (!xCharSequence.HasValue) return y is null || y.HasValue ? 0 : -1;
                    if (y is null || !y.HasValue) return 1;
                }

                if (x is ISpannable<char> xSpannable)
                {
                    if (!xSpannable.HasValue) return y is null || y.HasValue ? 0 : -1;
                    if (y is null || !y.HasValue) return 1;
                }

                return Compare(x, y?.Value, exceptionArgument);
            }
#endif


            private int Compare(SynchronizedTextBuilder? x, object? y, ExceptionArgument exceptionArgument)
            {
                if (x == y) return 0;
                if (x is not null && y is null) return 1;

                if (y is SynchronizedTextBuilder stb)
                {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                    if (x is null) return -1;
#endif
                    return J2N.Globalization.Ordinal.CompareString(x, stb);
                }
                if (y is ICharSequence cs)
                {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                    if (x is null) return cs.HasValue ? 0 : -1;
                    if (!cs.HasValue) return 1;
#endif
                    return J2N.Globalization.Ordinal.CompareString(x, cs);
                }
                if (y is ISpannable<char> spannable)
                {
                    if (x is null) return !spannable.HasValue ? 0 : -1;
                    if (!spannable.HasValue) return 1;

                    lock (x.SyncRoot)
                        return x.AsSpan().SequenceCompareTo(spannable.AsSpan());
                }
                if (y is string s)
                {
                    if (x is null) return -1;

                    lock (x.SyncRoot)
                        return x.AsSpan().SequenceCompareTo(s);
                }
                if (y is char[] charArray)
                {
                    if (x is null) return -1;

                    lock (x.SyncRoot)
                        return x.AsSpan().SequenceCompareTo(charArray);
                }
                if (y is StringBuilder sb)
                {
                    if (x is null) return -1;

                    lock (x.SyncRoot)
                        return J2N.Globalization.Ordinal.CompareString(x.AsSpan(), sb);
                }

                ThrowHelper.ThrowArgumentException(ExceptionResource.NotSupported_StringComparison, exceptionArgument);
                return 0; // Unreachable
            }

            private int Compare(object? x, SynchronizedTextBuilder? y, ExceptionArgument exceptionArgument)
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
            {
                if (x is ICharSequence xCharSequence && !xCharSequence.HasValue)
                    return y is null || y.Length == 0 ? 0 : -1;

                if (x is ISpannable<char> xSpannable && !xSpannable.HasValue)
                    return y is null || y.Length == 0 ? 0 : -1;

                return Compare(y, x, exceptionArgument) * -1;
            }
#else
                => Compare(y, x, exceptionArgument) * -1;
#endif

            private int Compare(StringBuffer? x, object? y, ExceptionArgument exceptionArgument)
            {
                if (x == y) return 0;
                if (x is not null && y is null) return 1;

                if (y is SynchronizedTextBuilder stb)
                    return J2N.Globalization.Ordinal.CompareString(x, stb);
                if (y is ICharSequence cs)
                {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                    if (x is null) return cs.HasValue ? 0 : -1;
                    if (!cs.HasValue) return 1;
#endif
                    return J2N.Globalization.Ordinal.CompareString(x, cs);
                }
                if (y is ISpannable<char> spannable)
                {
                    if (x is null) return !spannable.HasValue ? 0 : -1;
                    if (!spannable.HasValue) return -1;

                    lock (x.SyncRoot)
                        return J2N.Globalization.Ordinal.CompareString(x.builder, spannable.AsSpan());
                }
                if (y is string s)
                {
                    if (x is null) return -1;

                    lock (x.SyncRoot)
                        return J2N.Globalization.Ordinal.CompareString(x.builder, s);
                }
                if (y is char[] charArray)
                {
                    if (x is null) return -1;

                    lock (x.SyncRoot)
                        return J2N.Globalization.Ordinal.CompareString(x.builder, charArray);
                }
                if (y is StringBuilder sb)
                {
                    if (x is null) return -1;

                    lock (x.SyncRoot)
                        return J2N.Globalization.Ordinal.CompareString(x.builder, sb);
                }

                ThrowHelper.ThrowArgumentException(ExceptionResource.NotSupported_StringComparison, exceptionArgument);
                return 0; // Unreachable
            }

            private int Compare(object? x, StringBuffer? y, ExceptionArgument exceptionArgument)
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
            {
                if (x is ICharSequence xCharSequence && !xCharSequence.HasValue)
                    return y is null || ((ICharSequence)y).HasValue ? 0 : -1;

                if (x is ISpannable<char> xSpannable && !xSpannable.HasValue)
                    return y is null || ((ICharSequence)y).HasValue ? 0 : -1;

                return Compare(y, x, exceptionArgument) * -1;
            }
#else
                => Compare(y, x, exceptionArgument) * -1;
#endif

            private int Compare(ICharSequence? x, ReadOnlySpan<char> y)
            {
                if (x is null || !x.HasValue) return -1;

                if (x is ISpannable<char> spannable)
                {
                    return spannable.AsSpan().SequenceCompareTo(y);
                }
                if (x is StringBuilderCharSequence sb)
                {
                    return J2N.Globalization.Ordinal.CompareString(sb.Value, y);
                }
                if (x is SynchronizedTextBuilderCharSequence stb)
                {
                    lock (stb.SyncRoot)
                    {
                        return stb.Value.AsSpan().SequenceCompareTo(y);
                    }
                }
                if (x is StringBuffer stringBuffer)
                {
                    lock (stringBuffer.SyncRoot)
                    {
                        return J2N.Globalization.Ordinal.CompareString(stringBuffer.builder, y);
                    }
                }

                int result;
                int count = Math.Min(x.Length, y.Length);
                for (int i = 0; i < count; i++)
                {
                    if ((result = x[i] - y[i]) != 0)
                        return result;
                }

                // At this point, we have compared all the characters in at least one string.
                // The longer string will be larger.
                return x.Length - y.Length;
            }

            private int Compare(ReadOnlySpan<char> x, ICharSequence? y)
                => Compare(y, x) * -1;




            public override int Compare(ICharSequence? x, ICharSequence? y)
            {
                if (x == y) return 0;
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                if (x is null || !x.HasValue) return (y is null || y.HasValue) ? 0 : -1;
                if (y is null || !y.HasValue) return 1;
#else
                if (x is null || !x.HasValue) return (y is null || !y.HasValue) ? 0 : -1;
                if (y is null || !y.HasValue) return 1;
#endif
                if (x is SynchronizedTextBuilderCharSequence stbcs1)
                    return J2N.Globalization.Ordinal.CompareString(stbcs1.Value, y);
                if (x is StringBuffer sbuf1)
                    return J2N.Globalization.Ordinal.CompareString(sbuf1, y);
                if (y is SynchronizedTextBuilderCharSequence stbcs2)
                    return J2N.Globalization.Ordinal.CompareString(x, stbcs2.Value);
                if (y is StringBuffer sbuf2)
                    return J2N.Globalization.Ordinal.CompareString(x, sbuf2);

                if (x is StringBuilderCharSequence sbcs1)
                {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                    if (x is null) return (y is null || y.HasValue) ? 0 : -1;
                    if (y is null || !y.HasValue) return 1;
#endif
                    return J2N.Globalization.Ordinal.CompareString(sbcs1.Value, y);
                }
                if (y is StringBuilderCharSequence sbcs2)
                    return Compare(x, sbcs2.Value);
                if (x is ISpannable<char> spannable1)
                    return Compare(spannable1.AsSpan(), y);
                if (y is ISpannable<char> spannable2)
                    return Compare(x, spannable2.AsSpan());

                int length = Math.Min(x.Length, y.Length);
                int result;
                for (int i = 0; i < length; i++)
                {
                    if ((result = x[i] - y[i]) != 0)
                        return result;
                }

                // At this point, we have compared all the characters in at least one string.
                // The longer string will be larger.
                return x.Length - y.Length;
            }

            public override int Compare(ICharSequence? x, char[]? y)
            {
                if (x is null || !x.HasValue) return (y is null) ? 0 : -1;
                if (y is null) return 1;

                return Compare(x, y.AsSpan());
            }

            public override int Compare(ICharSequence? x, string? y)
            {
                if (x is null || !x.HasValue) return (y is null) ? 0 : -1;
                if (y is null) return 1;

                return Compare(x, y.AsSpan());
            }

            public override int Compare(ICharSequence? x, StringBuilder? y)
            {
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                if (x == null || !x.HasValue) return -1;
                if (y == null) return 1;
#endif

                return J2N.Globalization.Ordinal.CompareString(x, y);
            }




            public override bool Equals(object? x, object? y)
            {
                if (x == y) return true;
#if FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                if (x is null || y is null) return false;
#endif

                if (x is SynchronizedTextBuilder stb1)
                    return Equals(stb1, y, ExceptionArgument.y);
                if (y is SynchronizedTextBuilder stb2)
                    return Equals(x, stb2, ExceptionArgument.x);

                if (x is SynchronizedTextBuilderCharSequence stbcs1)
                    return Equals(stbcs1.Value, y, ExceptionArgument.y);
                if (y is SynchronizedTextBuilderCharSequence stbcs2)
                    return Equals(x, stbcs2.Value, ExceptionArgument.x);

                if (x is StringBuffer sbuf1)
                    return Equals(sbuf1, y, ExceptionArgument.y);
                if (y is StringBuffer sbuf2)
                    return Equals(x, sbuf2, ExceptionArgument.x);

                if (x is ICharSequence cs1)
                {
                    if (y is null)
                    {
                        return !cs1.HasValue;
                    }

                    if (y is ISpannable<char> otherSpannable)
                    {
                        if (!cs1.HasValue) return !otherSpannable.HasValue;
                        if (!otherSpannable.HasValue) return false;

                        return Equals(cs1, otherSpannable.AsSpan());
                    }

                    if (y is ICharSequence otherCharSequence)
                        return Equals(cs1, otherCharSequence);
                    else if (y is string otherString)
                        return Equals(cs1, otherString);
                    else if (y is StringBuilder otherStringBuilder)
                        return Equals(cs1, otherStringBuilder);
                    else if (y is char[] otherCharArray)
                        return Equals(cs1, otherCharArray);
                }

                if (y is ICharSequence cs2)
                {
#if !FEATURE_BROKEN_NULL_CHARSEQUENCE_COMPARISON
                    if (x is null)
                    {
                        return !cs2.HasValue;
                    }
#endif

                    if (x is ISpannable<char> otherSpannable)
                    {
                        if (!otherSpannable.HasValue) return !cs2.HasValue;
                        if (!cs2.HasValue) return false;

                        return Equals(otherSpannable.AsSpan(), cs2);
                    }
                    // x is ICharSequence tried above

                    if (x is string otherString)
                        return Equals(otherString, cs2);
                    else if (x is StringBuilder otherStringBuilder)
                        return J2N.Globalization.Ordinal.Equal(otherStringBuilder, cs2);
                    else if (x is char[] otherCharArray)
                        return Equals(otherCharArray, cs2);
                }

                if (x is ISpannable<char> spannable1)
                {
                    if (y is null)
                    {
                        return !spannable1.HasValue;
                    }

                    if (y is ISpannable<char> yspannable2)
                    {
                        if (!spannable1.HasValue) return !yspannable2.HasValue;
                        if (!yspannable2.HasValue) return false;

                        return spannable1.AsSpan().SequenceEqual(yspannable2.AsSpan());
                    }

                    if (!spannable1.HasValue) return false; // y cannot be null here

                    if (y is string ys2)
                        return spannable1.AsSpan().SequenceEqual(ys2);
                    if (y is char[] yca2)
                        return spannable1.AsSpan().SequenceEqual(yca2);
                    if (y is StringBuilder ysb2)
                        return J2N.Globalization.Ordinal.Equal(spannable1.AsSpan(), ysb2);
                }
                if (y is ISpannable<char> spannable2)
                {
                    if (x is null) return !spannable2.HasValue;
                    if (!spannable2.HasValue) return false;
                    // x is ISpannable<char> tried above
                    if (x is string xs1)
                        return xs1.AsSpan().SequenceEqual(spannable2.AsSpan());
                    if (x is char[] xca1)
                        return xca1.AsSpan().SequenceEqual(spannable2.AsSpan());
                    if (x is StringBuilder xsb1)
                        return J2N.Globalization.Ordinal.Equal(xsb1, spannable2.AsSpan());
                }

                if (x is string s1)
                {
                    if (y is null) return false;
                    if (y is string ys2)
                        return s1.AsSpan().SequenceEqual(ys2);
                    if (y is char[] yca2)
                        return s1.AsSpan().SequenceEqual(yca2);
                    if (y is StringBuilder ysb2)
                        return J2N.Globalization.Ordinal.Equal(s1, ysb2);
                }
                if (y is string s2)
                {
                    if (x is null) return false;
                    // x is string tried above
                    if (x is char[] xca2)
                        return xca2.AsSpan().SequenceEqual(s2);
                    if (x is StringBuilder xsb2)
                        return J2N.Globalization.Ordinal.Equal(xsb2, s2);
                }

                if (x is char[] ca1)
                {
                    if (y is null) return false;
                    // y is string tried above
                    if (y is char[] yca2)
                        return ca1.AsSpan().SequenceEqual(yca2);
                    if (y is StringBuilder ysb2)
                        return J2N.Globalization.Ordinal.Equal(ca1, ysb2);
                }
                if (y is char[] ca2)
                {
                    if (x is null) return false;
                    // x is string tried above
                    // x is char[] tried above
                    if (x is StringBuilder xsb2)
                        return J2N.Globalization.Ordinal.Equal(xsb2, ca2);
                }

                if (x is StringBuilder sb1)
                {
                    if (y is null) return false;
                    // y is string tried above
                    // y is char[] tried above
                    if (y is StringBuilder ysb2)
                        return J2N.Globalization.Ordinal.Equal(sb1, ysb2);
                }
                if (y is StringBuilder sb2)
                {
                    if (x is null) return false;
                    // x is string tried above
                    // x is char[] tried above
                    // x is StringBuilder tried above
                }

#if FEATURE_BROKEN_CHARSEQENCE_EXCEPTION_HANDLING
                return x.Equals(y);
#else
                return false;
#endif
            }

            private bool Equals(SynchronizedTextBuilder? x, object? y, ExceptionArgument exceptionArgument)
            {
                if (x == y) return true;
                if (x is not null && y is null) return false;

                if (y is SynchronizedTextBuilder stb)
                {
                    return J2N.Globalization.Ordinal.Equal(x, stb);
                }
                if (y is ICharSequence cs)
                {
                    return J2N.Globalization.Ordinal.Equal(x, cs);
                }
                if (y is ISpannable<char> spannable)
                {
                    if (x is null) return !spannable.HasValue;
                    if (!spannable.HasValue) return false;

                    lock (x.SyncRoot)
                        return x.AsSpan().SequenceEqual(spannable.AsSpan());
                }
                if (y is string s)
                {
                    if (x is null) return false;

                    lock (x.SyncRoot)
                        return x.AsSpan().SequenceEqual(s);
                }
                if (y is char[] charArray)
                {
                    if (x is null) return false;

                    lock (x.SyncRoot)
                        return x.AsSpan().SequenceEqual(charArray);
                }
                if (y is StringBuilder sb)
                {
                    if (x is null) return false;

                    lock (x.SyncRoot)
                        return J2N.Globalization.Ordinal.Equal(x.AsSpan(), sb);
                }

                return false;
            }

            private bool Equals(object? x, SynchronizedTextBuilder? y, ExceptionArgument exceptionArgument)
                => Equals(y, x, exceptionArgument);

            private bool Equals(StringBuffer? x, object? y, ExceptionArgument exceptionArgument)
            {
                if (x == y) return true;
                if (x is not null && y is null) return false;

                if (y is SynchronizedTextBuilder stb)
                {
                    return J2N.Globalization.Ordinal.Equal(x, stb);
                }
                if (y is ICharSequence cs)
                {
                    return J2N.Globalization.Ordinal.Equal(x, cs);
                }
                if (y is ISpannable<char> spannable)
                {
                    if (x is null) return !spannable.HasValue;
                    if (!spannable.HasValue) return false;

                    lock (x.SyncRoot)
                        return J2N.Globalization.Ordinal.Equal(x.builder, spannable.AsSpan());
                }
                if (y is string s)
                {
                    if (x is null) return false;

                    lock (x.SyncRoot)
                        return J2N.Globalization.Ordinal.Equal(x.builder, s);
                }
                if (y is char[] charArray)
                {
                    if (x is null) return false;

                    lock (x.SyncRoot)
                        return J2N.Globalization.Ordinal.Equal(x.builder, charArray);
                }
                if (y is StringBuilder sb)
                {
                    if (x is null) return false;

                    lock (x.SyncRoot)
                        return J2N.Globalization.Ordinal.Equal(x.builder, sb);
                }

                return false;
            }

            private bool Equals(object? x, StringBuffer? y, ExceptionArgument exceptionArgument)
                => Equals(y, x, exceptionArgument);

            private bool Equals(ICharSequence? x, ReadOnlySpan<char> y)
            {
                if (x is null || !x.HasValue) return false;

                if (x is ISpannable<char> spannable)
                {
                    return spannable.AsSpan().SequenceEqual(y);
                }
                if (x is StringBuilderCharSequence sb)
                {
                    return J2N.Globalization.Ordinal.Equal(sb.Value, y);
                }
                if (x is SynchronizedTextBuilderCharSequence stb)
                {
                    lock (stb.SyncRoot)
                    {
                        return stb.Value.AsSpan().SequenceEqual(y);
                    }
                }
                if (x is StringBuffer stringBuffer)
                {
                    lock (stringBuffer.SyncRoot)
                    {
                        return J2N.Globalization.Ordinal.Equal(stringBuffer.builder, y);
                    }
                }

                int len = x.Length;
                if (len != y.Length) return false;
                for (int i = 0; i < len; i++)
                {
                    if (x[i] != y[i]) return false;
                }
                return true;
            }

            private bool Equals(ReadOnlySpan<char> x, ICharSequence? y)
                => Equals(y, x);


            public override bool Equals(ICharSequence? x, ICharSequence? y)
            {
                if (x == y) return true;
                if (x is null || !x.HasValue)
                    return y is null || !y.HasValue;
                if (y is null || !y.HasValue)
                    return false;

                if (x is SynchronizedTextBuilderCharSequence stbcs1)
                    return J2N.Globalization.Ordinal.Equal(stbcs1.Value, y);
                if (x is StringBuffer sbuf1)
                    return J2N.Globalization.Ordinal.Equal(sbuf1, y);
                if (y is SynchronizedTextBuilderCharSequence stbcs2)
                    return J2N.Globalization.Ordinal.Equal(x, stbcs2.Value);
                if (y is StringBuffer sbuf2)
                    return J2N.Globalization.Ordinal.Equal(x, sbuf2);

                if (x is StringBuilderCharSequence sbcs1)
                {
                    return J2N.Globalization.Ordinal.Equal(sbcs1.Value, y);
                }
                if (y is StringBuilderCharSequence sbcs2)
                    return Equals(x, sbcs2.Value);
                if (x is ISpannable<char> spannable1)
                    return Equals(spannable1.AsSpan(), y);
                if (y is ISpannable<char> spannable2)
                    return Equals(x, spannable2.AsSpan());

                int len = x.Length;
                if (len != y.Length) return false;
                for (int i = 0; i < len; i++)
                {
                    if (x[i] != y[i]) return false;
                }
                return true;
            }

            public override bool Equals(ICharSequence? x, char[]? y)
            {
                if (x is null || !x.HasValue)
                    return y is null;
                if (y is null)
                    return false;

                return Equals(x, y.AsSpan());
            }

            public override bool Equals(ICharSequence? x, StringBuilder? y)
            {
                return J2N.Globalization.Ordinal.Equal(x, y);
            }

            public override bool Equals(ICharSequence? x, string? y)
            {
                if (x is null || !x.HasValue)
                    return y is null;
                if (y is null)
                    return false;

                return Equals(x, y.AsSpan());
            }


            private int GetHashCode(ReadOnlySpan<char> value)
            {
                // From Apache Harmony
                int length = value.Length;
                if (length == 0)
                    return 0;

                unchecked
                {
                    int hash = 0;
                    for (int i = 0; i < length; i++)
                    {
                        hash = value[i] + ((hash << 5) - hash);
                    }
                    return hash;
                }
            }

            public override int GetHashCode(object? obj)
            {
                if (obj is null)
                    return int.MaxValue;

                if (obj is string otherString)
                    return GetHashCode(otherString);
                else if (obj is StringBuilder otherStringBuilder)
                    return GetHashCode(otherStringBuilder);
                else if (obj is char[] otherCharArray)
                    return GetHashCode(otherCharArray);
                else if (obj is ISpannable<char> otherSpannable)
                {
                    if (!otherSpannable.HasValue)
                        return int.MaxValue;

                    return GetHashCode(otherSpannable.AsSpan());
                }
                else if (obj is ICharSequence otherCharSequence)
                    return GetHashCode(otherCharSequence);
                else if (obj is SynchronizedTextBuilder otherSynchronizedTextBuilder)
                {
                    lock (otherSynchronizedTextBuilder.SyncRoot)
                        return GetHashCode(otherSynchronizedTextBuilder.AsSpan());
                }

#if FEATURE_BROKEN_CHARSEQENCE_EXCEPTION_HANDLING
                return obj.GetHashCode();
#else
                ThrowHelper.ThrowArgumentException(ExceptionResource.NotSupported_StringComparison);
                return 0; // unreachable
#endif
            }


            public override int GetHashCode(ICharSequence? obj)
            {
                if (obj is null ||!obj.HasValue)
                    return int.MaxValue;

                if (obj is ISpannable<char> spannable)
                    return GetHashCode(spannable.AsSpan());
                if (obj is StringBuilderCharSequence yStringBuilder)
                    return GetHashCode(yStringBuilder.Value);
                if (obj is SynchronizedTextBuilderCharSequence synchronizedTextBuilderCharSequence)
                {
                    lock (synchronizedTextBuilderCharSequence.SyncRoot)
                        return GetHashCode(synchronizedTextBuilderCharSequence.Value.AsSpan());
                }
                if (obj is StringBuffer yStringBuffer)
                {
                    lock (yStringBuffer.SyncRoot)
                        return GetHashCode(yStringBuffer.builder);
                }

                // From Apache Harmony
                int length = obj.Length;
                if (length == 0)
                    return 0;

                unchecked
                {
                    int hash = 0;
                    for (int i = 0; i < length; i++)
                    {
                        hash = obj[i] + ((hash << 5) - hash);
                    }
                    return hash;
                }
            }

            public override int GetHashCode(char[]? obj)
            {
                if (obj is null)
                    return int.MaxValue;

                // From Apache Harmony
                int length = obj.Length;
                if (length == 0)
                    return 0;

                unchecked
                {
                    int hash = 0;
                    for (int i = 0; i < length; i++)
                    {
                        hash = obj[i] + ((hash << 5) - hash);
                    }
                    return hash;
                }
            }

            public override int GetHashCode(StringBuilder? obj)
            {
                if (obj is null)
                    return int.MaxValue;

                int length = obj.Length;
                if (length == 0)
                    return 0;

#if FEATURE_STRINGBUILDER_GETCHUNKS
                unchecked
                {
                    // From Apache Harmony
                    int hash = 0;
                    foreach (ReadOnlyMemory<char> chunk in obj.GetChunks())
                    {
                        ReadOnlySpan<char> chars = chunk.Span;
                        for (int i = 0; i < chars.Length; i++)
                        {
                            hash = chars[i] + ((hash << 5) - hash);
                        }
                    }
                    return hash;
                }
#else
                char[]? arrayToReturnToPool = null;
                try
                {
#if FEATURE_STRINGBUILDER_COPYTO_SPAN // If this method isn't supported, we are buffering to an array pool to get to the stack, anyway.
                    Span<char> objChars = length > CharStackBufferSize
                        ? (arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length))
                        : stackalloc char[length];
                    obj.CopyTo(0, objChars, length);
#else
                    Span<char> objChars = arrayToReturnToPool = ArrayPool<char>.Shared.Rent(length);
                    obj.CopyTo(0, arrayToReturnToPool, 0, length);
#endif
                    unchecked
                    {
                        // From Apache Harmony
                        int hash = 0;
                        for (int i = 0; i < length; i++)
                        {
                            hash = objChars[i] + ((hash << 5) - hash);
                        }
                        return hash;
                    }
                }
                finally
                {
                    ArrayPool<char>.Shared.ReturnIfNotNull(arrayToReturnToPool);
                }
#endif
            }

            public override int GetHashCode(string? obj)
            {
                if (obj is null)
                    return int.MaxValue;

                // From Apache Harmony
                int length = obj.Length;
                if (length == 0)
                    return 0;

                unchecked
                {
                    int hash = 0;
                    for (int i = 0; i < length; i++)
                    {
                        hash = obj[i] + ((hash << 5) - hash);
                    }
                    return hash;
                }
            }
        }
    }
}
