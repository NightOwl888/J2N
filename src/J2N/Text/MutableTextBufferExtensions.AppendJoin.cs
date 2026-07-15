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

using J2N.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace J2N.Text
{
    public static partial class MutableTextBufferExtensions
    {
#pragma warning disable CS1591 // J2N TODO: Finish docs

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendJoin<TBuilder>(this TBuilder text, string? separator, params object?[] values)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendJoinInternal(separator, values);
            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendJoin<TBuilder>(this TBuilder text, string? separator, params ReadOnlySpan<object?> values)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendJoinInternal(separator, values);
            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendJoin<TBuilder, T>(this TBuilder text, string? separator, IEnumerable<T> values)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendJoinInternal(separator, values);
            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendJoin<TBuilder>(this TBuilder text, string? separator, params string?[] values)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendJoinInternal(separator, values);
            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendJoin<TBuilder>(this TBuilder text, string? separator, params ReadOnlySpan<string?> values)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendJoinInternal(separator, values);
            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendJoin<TBuilder>(this TBuilder text, char separator, params object?[] values)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendJoinInternal(separator, values);
            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendJoin<TBuilder>(this TBuilder text, char separator, params ReadOnlySpan<object?> values)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendJoinInternal(separator, values);
            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendJoin<TBuilder, T>(this TBuilder text, char separator, IEnumerable<T> values)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendJoinInternal(separator, values);
            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendJoin<TBuilder>(this TBuilder text, char separator, params string?[] values)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendJoinInternal(separator, values);
            return text;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [CodeGenerationGenerateForwarder]
        internal static TBuilder AppendJoin<TBuilder>(this TBuilder text, char separator, params ReadOnlySpan<string?> values)
            where TBuilder : MutableTextBuffer
        {
            if (text is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);

            text.AppendJoinInternal(separator, values);
            return text;
        }

#pragma warning restore CS1591
    }
}
