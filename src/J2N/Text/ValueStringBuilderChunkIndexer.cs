#if FEATURE_STRINGBUILDER_GETCHUNKS
using J2N.Memory;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Text
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// A decorator for <see cref="StringBuilder"/> to track access to the last used chunk so it doesn't have to be looked up
    /// when iterating forward or reverse through the <see cref="StringBuilder"/>. Supports both reads and writes to the underlying
    /// memory. The purpose is to provide a unified view of the values returned by <see cref="StringBuilder.GetChunks()"/> so
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
    /// Do not call any operations that change the number of chunks in the <see cref="StringBuilder"/>, such as <see cref="StringBuilder.Append(string?)"/>,
    /// <see cref="StringBuilder.Insert(int, string?)"/>, or <see cref="StringBuilder.EnsureCapacity(int)"/>. If the number of chunks changes, the behavior
    /// is undefined and you must create a new instance of <see cref="ValueStringBuilderChunkIndexer"/> to read the changes.
    /// <para/>
    /// This type is disposable and the user is responsible for calling <see cref="Dispose()"/> after use.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Structs have performance issues with readonly fields.")]
    internal ref struct ValueStringBuilderChunkIndexer
    {
        private /*readonly*/ StringBuilder stringBuilder;
        private /*readonly*/ ValueInt32Packer packer;
        private /*readonly*/ bool usePacker;
        private /*readonly*/ int[]? upperBounds;
        private ReadOnlyMemory<char> currentChunk;
        private int chunkCount;
        private int currentChunkIndex;
        private int currentLowerBound;
        private int currentUpperBound;
        private bool iterateForward;

        public ValueStringBuilderChunkIndexer(StringBuilder stringBuilder, bool iterateForward = true)
        {
            this.stringBuilder = stringBuilder ?? throw new ArgumentNullException(nameof(stringBuilder));
            this.iterateForward = iterateForward;

            packer = new ValueInt32Packer();
            upperBounds = null;
            bool first = true;
            bool bound0Set = false;
            int lowerBound = 0;
            int upperBound = -1;
            chunkCount = 0;
            currentChunkIndex = 0;
            currentLowerBound = 0;
            currentUpperBound = upperBound;
            currentChunk = null;

            foreach (var _ in stringBuilder.GetChunks())
                chunkCount++;

            usePacker = chunkCount <= ValueInt32Packer.TotalSlots && stringBuilder.Length < ValueInt32Packer.MaxValue;

            int chunkIndex = 0;
            int prevChunkLength = 0;
            foreach (var chunk in stringBuilder.GetChunks())
            {
                lowerBound += prevChunkLength;
                upperBound += chunk.Length;
                prevChunkLength = chunk.Length;
                if (first)
                {
                    currentChunkIndex = 0;
                    currentLowerBound = 0;
                    currentUpperBound = upperBound;
                    currentChunk = chunk;
                    first = false;
                }
                else
                {
                    if (!bound0Set)
                    {
                        SetUpperBound(0, currentUpperBound);
                        bound0Set = true;
                    }
                    SetUpperBound(chunkIndex, upperBound);
                }
                chunkIndex++;
            }
            Reset();
        }

        public void Dispose()
        {
            if (upperBounds != null)
                ArrayPool<int>.Shared.Return(upperBounds);
        }

        private void SetUpperBound(int chunkIndex, int upperBound)
        {
            if (usePacker)
            {
                packer.SetValue(chunkIndex, upperBound);
            }
            else
            {
                upperBounds ??= ArrayPool<int>.Shared.Rent(chunkCount);
                upperBounds[chunkIndex] = upperBound;
            }
        }

        private void GetBounds(int chunkIndex, out int lowerBound, out int upperBound)
        {
            if (usePacker)
            {
                lowerBound = chunkIndex == 0 ? 0 : packer.GetValue(chunkIndex - 1) + 1;
                upperBound = packer.GetValue(chunkIndex);
            }
            else
            {
                Debug.Assert(upperBounds != null);

                lowerBound = chunkIndex == 0 ? 0 : upperBounds![chunkIndex - 1] + 1;
                upperBound = upperBounds[chunkIndex];
            }
            
        }

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private bool IsWithinBounds(int chunkIndex, int index)
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

            int index = -1;
            foreach (var chunk in stringBuilder.GetChunks())
            {
                index++;
                if (index == chunkIndex)
                {
                    currentChunk = chunk;
                    GetBounds(chunkIndex, out int lowerBound, out int upperBound);
                    currentLowerBound = lowerBound;
                    currentUpperBound = upperBound;
                    currentChunkIndex = chunkIndex;
                    return;
                }
            }
        }

#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void Reset()
        {
            int chunkIndex = iterateForward ? 0 : chunkCount - 1;
            SetChunk(chunkIndex);
        }

        public void Reset(bool iterateForward)
        {
            this.iterateForward = iterateForward;
            Reset();
        }

        public bool IterateForward => iterateForward;

        public int Length => stringBuilder.Length;

        public char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

                return currentChunk.Span[index - currentLowerBound];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

                unsafe
                {
                    using var handle = currentChunk.Pin();
                    char* pointer = (char*)handle.Pointer;
                    pointer[index - currentLowerBound] = value;
                }
            }
        }
    }
}
#endif