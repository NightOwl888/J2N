// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace J2N.Text
{
    // TextBuilderChunkEnumerator supports both the IEnumerable and IEnumerator pattern so foreach
    // works (see GetChunks).  It needs to be public (so the compiler can use it
    // when building a foreach statement) but users typically don't use it explicitly.
    // This class was nested inside StringBuilder in the BCL, but we have moved it to the top
    // level so the same type can be shared across TextBuilder implementations.

    /// <summary>
    /// Supports simple iteration over the chunks of an <see cref="TextBuilder"/> instance.
    /// </summary>
    /// <remarks>
    /// A <see cref="TextBuilderChunkEnumerator"/> is returned by the <see cref="TextBuilderExtensions.GetChunks(TextBuilder?)"/>
    /// method. It supports both the <see cref="System.Collections.IEnumerable"/> and <see cref="System.Collections.IEnumerator"/>
    /// patterns so that the chunks can be enumerated with foreach in C# or For Each in Visual Basic.
    /// <para/>
    /// <see cref="TextBuilderChunkEnumerator"/> is a public structure so that language compilers can use it to build a
    /// foreach statement. However, developers typically don't use it explicitly.
    /// <para/>
    /// This implementation only supports a single chunk. It is being provided for API compatibility with .NET only. It
    /// is generally more practical to access the underlying memory through <see cref="TextBuilderExtensions.AsSpan(TextBuilder?)"/>
    /// or one of its overloads.
    /// </remarks>
    public struct TextBuilderChunkEnumerator
    {
        private readonly MutableTextBuffer _firstChunk;
        private MutableTextBuffer? _currentChunk;

        /// <summary>
        /// Provides an <see cref="System.Collections.IEnumerable.GetEnumerator()"/> implementation that
        /// returns <c>this</c> as the <see cref="System.Collections.IEnumerator"/>.
        /// </summary>
        /// <returns>An enumerator object that can be used to iterate through the chunks.</returns>
        [EditorBrowsable(EditorBrowsableState.Never)] // Only here to make foreach work
#pragma warning disable IDE0251 // Make member 'readonly'
        public TextBuilderChunkEnumerator GetEnumerator() => this;
#pragma warning restore IDE0251 // Make member 'readonly'

        /// <summary>
        /// Advances the enumerator to the next chunk in the collection.
        /// </summary>
        /// <returns><c>true</c> if the enumerator was successfully advanced to the next element;
        /// <c>false</c> if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            if (_currentChunk == _firstChunk)
            {
                return false;
            }

            _currentChunk = _firstChunk;
            return true;
        }

        /// <summary>
        /// Gets the chunk and the current position of the collection.
        /// </summary>
        /// <value>The chunk at the current position of the collection.</value>
        public ReadOnlyMemory<char> Current
        {
            get
            {
                if (_currentChunk == null)
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();

                return new ReadOnlyMemory<char>(_currentChunk.m_Chars, 0, _currentChunk.m_Position);
            }
        }

        internal TextBuilderChunkEnumerator(MutableTextBuffer stringBuilder)
        {
            Debug.Assert(stringBuilder != null);
            _firstChunk = stringBuilder!;
            _currentChunk = null;   // MoveNext will find the last chunk if we do this.
        }
    }
}
