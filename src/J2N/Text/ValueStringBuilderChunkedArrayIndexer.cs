using System;
#if FEATURE_ARRAYPOOL
using System.Buffers;
#endif
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Text
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// A decorator for <see cref="StringBuilder"/> to track access to the last used chunk so it doesn't have to be looked up
    /// when iterating forward or reverse through the <see cref="StringBuilder"/>. Supports both reads and writes.
    /// The purpose is to chunk the <see cref="char"/>s from the <see cref="StringBuilder"/> into an array, which is pooled
    /// if the platform supports array pooling. The interface is similar to the <see cref="StringBuilder"/> so
    /// business logic doesn't have to be rewritten to iterate a <see cref="StringBuilder"/> via index. The performance of
    /// the <see cref="StringBuilder.this[int]"/> indexer is very poor and this is intended as a direct replacement.
    /// <para/>
    /// Usage Note: This works as a drop-in replacement for <see cref="StringBuilder"/> in cases where we are looping either forward
    /// or backward through the <see cref="char"/>s, without the need to add additional business logic to deal with switching chunks.
    /// <para/>
    /// Both sequential and random access are supported, however, sequential access is optimized. If the business logic simultaneously
    /// iterates both forward and backward, it is recommended to create 2 separate instances (one specifying <see cref="iterateForward"/>=false)
    /// to ensure looking up chunks in the <see cref="StringBuilder"/> is a rare operation. Writes are done to the underlying memory of the
    /// <see cref="StringBuilder"/>, so using multiple instances for tracking both directions will immediately show the changes.
    /// <para/>
    /// Do not call any operations that mutate the <see cref="StringBuilder"/>, such as <see cref="StringBuilder.Append(string?)"/>,
    /// <see cref="StringBuilder.Insert(int, string?)"/>. If the state of the <see cref="StringBuilder"/> changes, the behavior
    /// is undefined and you must create a new instance of <see cref="ValueStringBuilderChunkedArrayIndexer"/> to read the changes.
    /// <para/>
    /// This type is disposable and the user is responsible for calling <see cref="Dispose()"/> after use.
    /// <para/>
    /// For .NET Core 3.x and higher, the ValueStringBuilderChunkIndexer should be favored over this approach.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Structs have performance issues with readonly fields.")]
    internal ref struct ValueStringBuilderChunkedArrayIndexer
    {
        public const int ChunkLength = 512;

        private /*readonly*/ StringBuilder stringBuilder;
        private /*readonly*/ int lastChunkLength;
        private char[] currentChunk;
        private int chunkCount;
        private int currentChunkIndex;
        private int currentLowerBound;
        private int currentUpperBound;
        private bool iterateForward;

        public ValueStringBuilderChunkedArrayIndexer(StringBuilder stringBuilder, bool iterateForward = true)
        {
            this.stringBuilder = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));
            this.iterateForward = iterateForward;

            chunkCount = 0;
            currentChunkIndex = -1;
            currentLowerBound = 0;
            currentUpperBound = 0;
            lastChunkLength = 0;
            

            int length = stringBuilder.Length;
            int chunkSize = length < ChunkLength ? Math.Max(1, length) : ChunkLength;
#if FEATURE_ARRAYPOOL
            currentChunk = ArrayPool<char>.Shared.Rent(chunkSize);
#else
            currentChunk = new char[chunkSize];
#endif
            chunkCount = (int)Math.Ceiling((double)length / ChunkLength);
            if (chunkCount > 0)
            {
                int remainder = (chunkCount * ChunkLength) - length;
                lastChunkLength = ChunkLength - remainder;
            }

            Reset();
        }

        internal int ChunkCount => chunkCount; // internal for testing
        internal int LastChunkLength => lastChunkLength; // internal for testing
        public void Dispose()
        {
#if FEATURE_ARRAYPOOL
            ArrayPool<char>.Shared.Return(currentChunk);
#endif
            // else no-op
        }

        internal void GetBounds(int chunkIndex, out int lowerBound, out int upperBound) // internal for testing
        {
            Debug.Assert(chunkIndex >= 0);
            Debug.Assert(chunkIndex < chunkCount || chunkIndex == 0);

            lowerBound = chunkIndex * ChunkLength;
            upperBound = ((chunkIndex + 1) * ChunkLength) - 1;

            // Fix last chunk upper bound
            if (chunkIndex == Math.Max(chunkCount - 1, 0))
            {
                int lastIndex = Math.Max(stringBuilder.Length - 1, 0);
                if (upperBound > lastIndex)
                    upperBound = lastIndex;
            }
        }

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        internal bool IsWithinBounds(int chunkIndex, int index)
        {
            GetBounds(chunkIndex, out int lowerBound, out int upperBound);
            return index >= lowerBound && index <= upperBound;
        }

        private void SetCurrentChunkFromIndex(int index)
        {
            if (iterateForward)
            {
                // Scan forward first
                for (int i = currentChunkIndex + 1; i < chunkCount; i++)
                {
                    if (IsWithinBounds(i, index))
                    {
                        SetChunk(i);
                        return;
                    }
                }
                for (int i = currentChunkIndex - 1; i >= 0; i--)
                {
                    if (IsWithinBounds(i, index))
                    {
                        SetChunk(i);
                        return;
                    }
                }
            }
            else
            {
                // Scan backward first
                for (int i = currentChunkIndex - 1; i >= 0; i--)
                {
                    if (IsWithinBounds(i, index))
                    {
                        SetChunk(i);
                        return;
                    }
                }
                for (int i = currentChunkIndex + 1; i < chunkCount; i++)
                {
                    if (IsWithinBounds(i, index))
                    {
                        SetChunk(i);
                        return;
                    }
                }
            }
        }

        private void SetChunk(int chunkIndex)
        {
            if (currentChunkIndex == chunkIndex)
                return;

            GetBounds(chunkIndex, out int lowerBound, out int upperBound);
            currentLowerBound = lowerBound;
            currentUpperBound = upperBound;
            currentChunkIndex = chunkIndex;
            int copyLength = chunkIndex == Math.Max(chunkCount - 1, 0) ? lastChunkLength : ChunkLength;
            if (copyLength > 0)
            {
                stringBuilder.CopyTo(lowerBound, currentChunk, 0, copyLength);
            }
        }

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Reset()
        {
            int chunkIndex = iterateForward ? 0 : Math.Max(chunkCount - 1, 0);
            SetChunk(chunkIndex);
        }

        public void Reset(bool iterateForward)
        {
            this.iterateForward = iterateForward;
            Reset();
        }

        public bool IterateForward => iterateForward;

        public char this[int index]
        {
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            get
            {
                if ((uint)index >= (uint)stringBuilder.Length)
                {
                    throw new IndexOutOfRangeException();
                }

                if (index < currentLowerBound || index > currentUpperBound)
                {
                    SetCurrentChunkFromIndex(index);
                }

                return currentChunk[index - currentLowerBound];
            }

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
            set
            {
                if ((uint)index >= (uint)stringBuilder.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_Index);
                }

                if (index < currentLowerBound || index > currentUpperBound)
                {
                    SetCurrentChunkFromIndex(index);
                }

                stringBuilder[index] = currentChunk[index - currentLowerBound] = value;
            }
        }
    }
}
