#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
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

namespace J2N.Text
{
    /// <summary>
    /// Tracks the current position in a parsed string. In case of an error the error
    /// index can be set to the position where the error occurred without having to
    /// change the parse position.
    /// </summary>
    public class ParsePosition
    {
        /// <summary>
        /// Constructs a new <see cref="ParsePosition"/> with the specified index.
        /// </summary>
        /// <param name="index">The index to begin parsing.</param>
        public ParsePosition(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Compares the specified object to this <see cref="ParsePosition"/> and indicates
        /// if they are equal. In order to be equal, <paramref name="obj"/> must be an
        /// instance of <see cref="ParsePosition"/> and it must have the same index and
        /// error index.
        /// </summary>
        /// <param name="obj">The object to compare with this object.</param>
        /// <returns><c>true</c> if the specified object is equal to this
        /// <see cref="ParsePosition"/>; <c>false</c> otherwise.</returns>
        /// <seealso cref="GetHashCode()"/>
        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (!(obj is ParsePosition pos))
            {
                return false;
            }
            return Index == pos.Index
                    && ErrorIndex == pos.ErrorIndex;
        }

        /// <summary>
        /// Gets or sets the index at which the parse could not continue.
        /// <para/>
        /// Returns -1 if there is no error.
        /// </summary>
        public int ErrorIndex { get; set; } = -1;

        /// <summary>
        /// Gets or sets the current parse position.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets the hash code for this instance.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return Index + ErrorIndex;
        }

        /// <summary>
        /// Returns the string representation of this parse position.
        /// </summary>
        /// <returns>The string representation of this parse position.</returns>
        public override string ToString()
        {
            return GetType().FullName + "[index=" + Index //$NON-NLS-1$
                    + ", errorIndex=" + ErrorIndex + "]"; //$NON-NLS-1$ //$NON-NLS-2$
        }
    }

}
