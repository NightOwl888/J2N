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

using System;
using System.Collections;

namespace J2N.Text
{
    /// <summary>
    /// An implementation of <see cref="ICharacterEnumerator"/> for strings.
    /// </summary>
    internal sealed class StringCharacterEnumerator : ICharacterEnumerator // J2N TODO: API Make this class public when the issues with converting iterator/enumerator are fixed
    {
        private string str;
        private int startIndex, length, position;

        /// <summary>
        /// Initializes a new <see cref="StringCharacterEnumerator"/> on the specified string.
        /// The begin and current indices are set to the beginning of <paramref name="value"/>, the
        /// length is set to the length of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The source string to iterate over.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public StringCharacterEnumerator(string value)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            => Reset(value);

        /// <summary>
        /// Constructs a new <see cref="StringCharacterEnumerator"/> on the specified string
        /// with the current index set to the specified <paramref name="value"/>. The begin index is set
        /// to the beginning of <paramref name="value"/>, the length is set to the length of <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The source string to iterate over.</param>
        /// <param name="position">The current index.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="position"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="position"/> is greater than the length of <paramref name="value"/>.
        /// </exception>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public StringCharacterEnumerator(string value, int position)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            => Reset(value, position);

        /// <summary>
        /// Constructs a new <see cref="StringCharacterEnumerator"/> on the specified string
        /// with the begin, end and current index set to the specified values.
        /// </summary>
        /// <param name="value">The source string to iterate over.</param>
        /// <param name="startIndex">The index of the first character to iterate.</param>
        /// <param name="length">The length of the characters to iterate.</param>
        /// <param name="position">The current index.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="length"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="length"/> indicates a position outside of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="position"/> is less that <paramref name="startIndex"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="position"/> is greater than <paramref name="startIndex"/> + <paramref name="length"/>.
        /// </exception>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public StringCharacterEnumerator(string value, int startIndex, int length, int position)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            => Reset(value, startIndex, length, position);

        /// <summary>
        /// Gets the position of the underlying character sequence where iteration begins.
        /// </summary>
        public int StartIndex => startIndex;

        /// <summary>
        /// Gets the position of the underlying character sequence where iteration ends.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the end index is inclusive,
        /// not exclusive as it would be in Java. To translate from Java, always use <c>EndIndex + 1</c>.
        /// </summary>
        public int EndIndex => Math.Max(startIndex + length - 1, 0);

        /// <summary>
        /// Gets the length of the span of characters that is being enumerated.
        /// </summary>
        public int Length => length;

        /// <summary>
        /// Gets or sets the index of the current position. The value must be between
        /// <see cref="StartIndex"/> and <see cref="EndIndex"/> inclusive.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value set indicates a position less than
        /// <see cref="StartIndex"/> or greater than <see cref="EndIndex"/>.</exception>
        public int Index
        {
            get => position;
            set
            {
                if (value < startIndex || value > EndIndex)
                    throw new ArgumentOutOfRangeException(nameof(value), "Invalid index");

                position = value;
            }
        }

        /// <summary>
        /// Attempts to set the <see cref="Index"/> to <paramref name="value"/>.
        /// <para/>
        /// Returns <c>true</c> if the <paramref name="value"/> passed is between
        /// <see cref="StartIndex"/> and <see cref="EndIndex"/> inclusive; otherwise, returns <c>false</c>.
        /// <para/>
        /// If <paramref name="value"/> is less than <see cref="StartIndex"/>, the
        /// <see cref="Index"/> will be set to <see cref="StartIndex"/>.
        /// <para/>
        /// If <paramref name="value"/> is greater than <see cref="EndIndex"/>, the
        /// <see cref="Index"/> will be set to <see cref="EndIndex"/>.
        /// </summary>
        /// <param name="value">The new index.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> passed is between
        /// <see cref="StartIndex"/> and <see cref="EndIndex"/> inclusive; otherwise, <c>false</c>.</returns>
        public bool TrySetIndex(int value)
        {
            if (value < startIndex)
            {
                position = startIndex;
                return false;
            }
            if (value > EndIndex)
            {
                position = EndIndex;
                return false;
            }
            position = value;
            return true;
        }

        /// <summary>
        /// Gets the <see cref="char"/> at the current position.
        /// </summary>
        public char Current => str[position];

        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        public void Dispose()
        {
            // Nothing to do
        }

        /// <inheritdoc/>
        public bool MoveFirst()
        {
            if (length == 0)
                return false;

            position = startIndex;
            return true;
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            if (position >= EndIndex)
            {
                position = EndIndex;
                return false;
            }
            position++;
            return true;
        }

        /// <inheritdoc/>
        public bool MovePrevious()
        {
            if (position == startIndex)
            {
                return false;
            }
            position--;
            return true;
        }

        /// <inheritdoc/>
        public bool MoveLast()
        {
            if (length == 0)
                return false;

            position = EndIndex;
            return true;
        }

        void IEnumerator.Reset() => position = startIndex;

        /// <summary>
        /// Sets the source string to iterate over. The begin position and length are
        /// set to the start and length of <paramref name="value"/>.
        /// <para/>
        /// Usage Note: This corresponds to the setText(String) method in the JDK.
        /// </summary>
        /// <param name="value">The new source string.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        public void Reset(string value)
            => Reset(value, 0);

        /// <summary>
        /// Sets the source string to iterate over and initial position to start from.
        /// </summary>
        /// <param name="value">The new source string.</param>
        /// <param name="position">The current index.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="position"/> is less than 0.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="position"/> is greater than the length of <paramref name="value"/>.
        /// </exception>
        public void Reset(string value, int position)
            => Reset(value, 0, value != null ? value.Length : 0, position);

        /// <summary>
        /// Sets the source string, startIndex, and length to iterate over and initial position to start from.
        /// </summary>
        /// <param name="value">The new source string.</param>
        /// <param name="startIndex">The index of the first character to iterate.</param>
        /// <param name="length">The length of the characters to iterate.</param>
        /// <param name="position">The current index.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="startIndex"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="length"/> is less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="length"/> indicates a position outside of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="position"/> is less that <paramref name="startIndex"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="position"/> is greater than <paramref name="startIndex"/> + <paramref name="length"/>.
        /// </exception>
        public void Reset(string value, int startIndex, int length, int position)
        {
            ThrowHelper.ThrowIfNull(value, ExceptionArgument.value);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex > value.Length - length) // Checks for int overflow
                throw new ArgumentOutOfRangeException(nameof(length), SR.ArgumentOutOfRange_IndexLength);
            if (position < startIndex || position - startIndex > length)
                throw new ArgumentOutOfRangeException(nameof(position));

            str = value;
            this.startIndex = startIndex;
            this.length = length;
            this.position = position;
        }

        /// <summary>
        /// Compares the specified object with this <see cref="StringCharacterEnumerator"/>
        /// and indicates if they are equal. In order to be equal, <paramref name="obj"/>
        /// must be an instance of <see cref="StringCharacterEnumerator"/> that iterates over
        /// the same sequence of characters with the same index.
        /// </summary>
        /// <param name="obj">The object to compare with this object.</param>
        /// <returns><c>true</c> if the specified object is equal to this <see cref="StringCharacterEnumerator"/>; <c>false</c> otherwise.</returns>
        /// <seealso cref="GetHashCode()"/>
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (!(obj is StringCharacterEnumerator other))
            {
                return false;
            }
            return str.Equals(other.str) && startIndex == other.startIndex && EndIndex == other.EndIndex
                    && position == other.position;
        }

        /// <summary>
        /// Gets the hash code for this <see cref="StringCharacterEnumerator"/>.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return str.GetHashCode() + startIndex + EndIndex + position;
        }

        /// <summary>
        /// Returns a new <see cref="StringCharacterEnumerator"/> with the same source
        /// string, startIndex, length, and current index as this enumerator.
        /// </summary>
        /// <returns>A shallow copy of this enumerator.</returns>
        public object Clone()
        {
            return base.MemberwiseClone();
        }
    }
}
