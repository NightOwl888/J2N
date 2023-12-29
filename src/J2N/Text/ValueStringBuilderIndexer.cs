using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// A decorator for <see cref="StringBuilder"/> to track access to the last used chunk so it doesn't have to be looked up
    /// when iterating forward or reverse through the <see cref="StringBuilder"/>. Supports both reads and writes.
    /// The purpose is to chunk access to the <see cref="char"/>s from the <see cref="StringBuilder"/> and hold a reference to them
    /// so iterating is fast. The interface is similar to the <see cref="StringBuilder"/> so
    /// business logic doesn't have to be rewritten to iterate a <see cref="StringBuilder"/> via index. The performance of
    /// the <see cref="StringBuilder.this[int]"/> indexer is very poor and this is intended as a direct replacement.
    /// <para/>
    /// Usage Note: This works as a drop-in replacement for <see cref="StringBuilder"/> in cases where we are looping either forward
    /// or backward through the <see cref="char"/>s, without the need to add additional business logic to deal with switching chunks.
    /// <para/>
    /// Both sequential and random access are supported, however, sequential access is optimized. If the business logic simultaneously
    /// iterates both forward and backward, it is recommended to create 2 separate instances (one specifying iterateForward=false)
    /// to ensure looking up chunks in the <see cref="StringBuilder"/> is a rare operation. Writes are done to the underlying memory of the
    /// <see cref="StringBuilder"/>, so using multiple instances for tracking both directions will immediately show the changes.
    /// <para/>
    /// Do not call any operations that mutate the <see cref="StringBuilder"/>, such as <see cref="StringBuilder.Append(string?)"/>,
    /// <see cref="StringBuilder.Insert(int, string?)"/>. If the state of the <see cref="StringBuilder"/> changes, the behavior
    /// is undefined and you must create a new instance of <see cref="ValueStringBuilderIndexer"/> to read the changes.
    /// <para/>
    /// This type is disposable and the user is responsible for calling <see cref="Dispose()"/> after use.
    /// </summary>
    /// <remarks>
    /// This implementation uses <c>StringBuilder.GetChunks()</c> when supported to read the chars. If not, it degrades to using
    /// <see cref="StringBuilder.CopyTo(int, char[], int, int)"/> to move chunks of the string to an array, which is used for
    /// indexing a specific chunk. When supported, the array is obtained from the <c>ArrayPool&lt;T&gt;</c>; otherwise the array
    /// is allocated directly on the heap with no reuse. Chunks are switched automatically when iterating before or after the bounds
    /// of the current chunk.
    /// </remarks>
    public ref struct ValueStringBuilderIndexer
    {
#if FEATURE_STRINGBUILDER_GETCHUNKS
        private /*readonly*/ ValueStringBuilderChunkIndexer indexer;
#else
        private /*readonly*/ ValueStringBuilderChunkedArrayIndexer indexer;
#endif
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of <see cref="ValueStringBuilderIndexer"/> with the specified
        /// <see cref="StringBuilder"/> instance and iteration direction.
        /// <para/>
        /// If the <see cref="StringBuilder"/> is edited after construction, the behavior of
        /// <see cref="ValueStringBuilderIndexer"/> is undefined. If this occurs, a new instance
        /// of <see cref="ValueStringBuilderIndexer"/> should be created to account for the
        /// <see cref="StringBuilder"/> edits.
        /// </summary>
        /// <param name="stringBuilder">A <see cref="StringBuilder"/> instance containing the text to iterate through.</param>
        /// <param name="iterateForward"><c>true</c> to optimize iteration for forward order;
        /// <c>false</c> to optimize iteration for reverse order.</param>
        public ValueStringBuilderIndexer(StringBuilder stringBuilder, bool iterateForward = true)
        {
#if FEATURE_STRINGBUILDER_GETCHUNKS
            indexer = new ValueStringBuilderChunkIndexer(stringBuilder, iterateForward);
#else
            indexer = new ValueStringBuilderChunkedArrayIndexer(stringBuilder, iterateForward);
#endif
            disposed = false;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="ValueStringBuilderIndexer"/>.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                indexer.Dispose();
                disposed = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this is a forward or reverse instance.
        /// </summary>
        public bool IterateForward => indexer.IterateForward;

        /// <summary>
        /// Gets the length of the <see cref="StringBuilder"/>.
        /// </summary>
        public int Length => indexer.Length;

        /// <summary>
        /// Gets or sets the character at the specified character position in the <see cref="StringBuilder"/>
        /// that is passed into the constructor.
        /// </summary>
        /// <param name="index">The position of the character.</param>
        /// <returns>The Unicode character at position <paramref name="index"/>.</returns>
        /// <exception cref="ObjectDisposedException">The struct has already been disposed.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is outside the bounds of this instance while setting a character.</exception>
        /// <exception cref="IndexOutOfRangeException"><paramref name="index"/> is outside the bounds of this instance while getting a character.</exception>
        public char this[int index]
        {
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get
            {
                EnsureOpen();
                return indexer[index];
            }
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            set
            {
                EnsureOpen();
                indexer[index] = value;
            }
        }

        private void EnsureOpen()
        {
            if (disposed)
                throw new ObjectDisposedException(nameof(ValueStringBuilderIndexer));
        }
    }
}
