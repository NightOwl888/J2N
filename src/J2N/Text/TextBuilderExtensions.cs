#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

namespace J2N.Text
{
    /// <summary>
    /// Extensions to <see cref="TextBuilder"/>.
    /// </summary>
    public static partial class TextBuilderExtensions
    {
        #region AsCharSequence

        /// <summary>
        /// Convenience method to wrap a string in an adapter
        /// so a <see cref="TextBuilder"/> can be used as <see cref="ICharSequence"/>.
        /// </summary>
        /// <remarks>
        /// This API is being provided for compatibility. It is recommended to use
        /// <see cref="AsSpan(TextBuilder?)"/> or <see cref="AsMemory(TextBuilder?)"/>
        /// as alternatives for new development even though it may be quicker to port
        /// from Java using <see cref="ICharSequence"/>.
        /// </remarks>
        public static ICharSequence AsCharSequence(this TextBuilder? text)
        {
            return new MutableTextBufferCharSequence(text?.buffer);
        }

        #endregion AsCharSequence

        #region GetChunks

        /// <summary>
        /// Returns an object that can be used to iterate through the chunks of characters represented in a
        /// <see cref="ReadOnlyMemory{Char}" /> created from this <see cref="TextBuilder" /> instance.
        /// </summary>
        /// <returns>An enumerator for the chunks in the <see cref="ReadOnlyMemory{Char}" />.</returns>
        /// <remarks>
        /// This API is for compatibility with the <c>System.Text.StringBuilder.GetChuncks()</c> method.
        /// <see cref="TextBuilder" /> will never have more than a single chunk of memory so it is generally
        /// more efficient to use <see cref="AsSpan(TextBuilder?)" /> or <see cref="AsMemory(TextBuilder?)" />
        /// when you need to access the underlying memory than calling this method.
        /// </remarks>
        public static TextBuilderChunkEnumerator GetChunks(this TextBuilder text)
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            return new TextBuilderChunkEnumerator(text.buffer);
        }

        #endregion GetChunks
    }
}
