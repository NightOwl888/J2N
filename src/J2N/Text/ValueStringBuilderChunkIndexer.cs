using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace J2N.Text
{
    using SR = J2N.Resources.Strings;

#if FEATURE_STRINGBUILDER_GETCHUNKS
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
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Structs have performance issues with readonly fields.")]
    internal ref struct ValueStringBuilderChunkIndexer // J2N TODO: Make public? This may be useful in ICU4N and Lucene.NET
    {
        private /*readonly*/ StringBuilder stringBuilder;
        private /*readonly*/ ChunkBoundsPacker boundsPacker;
        private /*readonly*/ bool usePacker;
        private /*readonly*/ int[]? lowerBounds;
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

            boundsPacker = new ChunkBoundsPacker();
            lowerBounds = null;
            upperBounds = null;
            bool first = true;
            bool bound0Set = false;
            int lowerBound = 0;
            int upperBound = -1;
            chunkCount = 0;
            currentChunkIndex = 0;
            currentLowerBound = lowerBound;
            currentUpperBound = upperBound;
            currentChunk = null;

            foreach (var _ in stringBuilder.GetChunks())
                chunkCount++;

            usePacker = chunkCount <= ChunkBoundsPacker.TotalChunks && stringBuilder.Length < ChunkBoundsPacker.MaxUpperBound;

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
                    currentLowerBound = lowerBound;
                    currentUpperBound = upperBound;
                    currentChunk = chunk;
                    first = false;
                }
                else
                {
                    if (!bound0Set)
                    {
                        SetBounds(0, currentLowerBound, currentUpperBound);
                        bound0Set = true;
                    }
                    SetBounds(chunkIndex, lowerBound, upperBound);
                }
                chunkIndex++;
            }
            Reset();
        }

        private void SetBounds(int chunkIndex, int lowerBound, int upperBound)
        {
            if (usePacker)
            {
                boundsPacker.SetBounds(chunkIndex, lowerBound, upperBound);
            }
            else
            {
                lowerBounds ??= new int[chunkCount];
                upperBounds ??= new int[chunkCount];
                lowerBounds[chunkIndex] = lowerBound;
                upperBounds[chunkIndex] = upperBound;
            }
        }

        private void GetBounds(int chunkIndex, out int lowerBound, out int upperBound)
        {
            if (usePacker)
            {
                boundsPacker.GetBounds(chunkIndex, out lowerBound, out upperBound);
            }
            else
            {
                Debug.Assert(lowerBounds != null);
                Debug.Assert(upperBounds != null);

                lowerBound = lowerBounds![chunkIndex];
                upperBound = upperBounds![chunkIndex];
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

        public char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if ((uint)index >= (uint)stringBuilder.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, SR.ArgumentOutOfRange_Index);
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

        internal ref struct ChunkBoundsPacker
        {
            public const int BitsPerChunk = 16; // Number of bits allocated for each chunk
            public const int ChunksPerULong = 64 / BitsPerChunk; // Number of chunks per ulong variable
            public const int TotalChunks = 16;
            public const int MaxUpperBound = (1 << BitsPerChunk) - 1; // Maximum value that can be stored with BitsPerChunk bits

            private ulong lowerBound1;
            private ulong upperBound1;
            private ulong lowerBound2;
            private ulong upperBound2;
            private ulong lowerBound3;
            private ulong upperBound3;
            private ulong lowerBound4;
            private ulong upperBound4;

            public void SetBounds(int chunkIndex, int lowerBound, int upperBound)
            {
                if (chunkIndex < 0 || chunkIndex >= TotalChunks)
                {
                    throw new ArgumentOutOfRangeException(nameof(chunkIndex));
                }

                if (lowerBound < 0 || lowerBound > MaxUpperBound)
                {
                    throw new ArgumentOutOfRangeException(nameof(lowerBound));
                }

                if (upperBound < 0 || upperBound > MaxUpperBound)
                {
                    throw new ArgumentOutOfRangeException(nameof(upperBound));
                }

                // Calculate the bit offset for the chunk within the ulong variables
                int bitOffset = (chunkIndex % ChunksPerULong) * BitsPerChunk;

                // Clear the existing bits for the chunk
                switch (chunkIndex / ChunksPerULong)
                {
                    case 0:
                        lowerBound1 &= ~((ulong)MaxUpperBound << bitOffset);
                        upperBound1 &= ~((ulong)MaxUpperBound << bitOffset);
                        break;
                    case 1:
                        lowerBound2 &= ~((ulong)MaxUpperBound << bitOffset);
                        upperBound2 &= ~((ulong)MaxUpperBound << bitOffset);
                        break;
                    case 2:
                        lowerBound3 &= ~((ulong)MaxUpperBound << bitOffset);
                        upperBound3 &= ~((ulong)MaxUpperBound << bitOffset);
                        break;
                    case 3:
                        lowerBound4 &= ~((ulong)MaxUpperBound << bitOffset);
                        upperBound4 &= ~((ulong)MaxUpperBound << bitOffset);
                        break;
                }

                // Pack the new bounds into the ulong
                switch (chunkIndex / ChunksPerULong)
                {
                    case 0:
                        lowerBound1 |= ((ulong)lowerBound & (ulong)MaxUpperBound) << bitOffset;
                        upperBound1 |= ((ulong)upperBound & (ulong)MaxUpperBound) << bitOffset;
                        break;
                    case 1:
                        lowerBound2 |= ((ulong)lowerBound & (ulong)MaxUpperBound) << bitOffset;
                        upperBound2 |= ((ulong)upperBound & (ulong)MaxUpperBound) << bitOffset;
                        break;
                    case 2:
                        lowerBound3 |= ((ulong)lowerBound & (ulong)MaxUpperBound) << bitOffset;
                        upperBound3 |= ((ulong)upperBound & (ulong)MaxUpperBound) << bitOffset;
                        break;
                    case 3:
                        lowerBound4 |= ((ulong)lowerBound & (ulong)MaxUpperBound) << bitOffset;
                        upperBound4 |= ((ulong)upperBound & (ulong)MaxUpperBound) << bitOffset;
                        break;
                }
            }

            public void GetBounds(int chunkIndex, out int lowerBound, out int upperBound)
            {
                if (chunkIndex < 0 || chunkIndex >= TotalChunks)
                {
                    throw new ArgumentOutOfRangeException(nameof(chunkIndex));
                }

                // Calculate the bit offset for the chunk within the ulong variables
                int bitOffset = (chunkIndex % ChunksPerULong) * BitsPerChunk;

                // Extract the packed bounds for the given chunk
                switch (chunkIndex / ChunksPerULong)
                {
                    case 0:
                        lowerBound = (int)((lowerBound1 >> bitOffset) & (ulong)MaxUpperBound);
                        upperBound = (int)((upperBound1 >> bitOffset) & (ulong)MaxUpperBound);
                        break;
                    case 1:
                        lowerBound = (int)((lowerBound2 >> bitOffset) & (ulong)MaxUpperBound);
                        upperBound = (int)((upperBound2 >> bitOffset) & (ulong)MaxUpperBound);
                        break;
                    case 2:
                        lowerBound = (int)((lowerBound3 >> bitOffset) & (ulong)MaxUpperBound);
                        upperBound = (int)((upperBound3 >> bitOffset) & (ulong)MaxUpperBound);
                        break;
                    case 3:
                        lowerBound = (int)((lowerBound4 >> bitOffset) & (ulong)MaxUpperBound);
                        upperBound = (int)((upperBound4 >> bitOffset) & (ulong)MaxUpperBound);
                        break;
                    default:
                        lowerBound = upperBound = 0; // This should not happen
                        break;
                }
            }

            public void PrintPackedBounds()
            {
                Console.WriteLine($"Lower Bounds 1: {Convert.ToString((long)lowerBound1, 2).PadLeft(64, '0')}");
                Console.WriteLine($"Upper Bounds 1: {Convert.ToString((long)upperBound1, 2).PadLeft(64, '0')}");
                Console.WriteLine($"Lower Bounds 2: {Convert.ToString((long)lowerBound2, 2).PadLeft(64, '0')}");
                Console.WriteLine($"Upper Bounds 2: {Convert.ToString((long)upperBound2, 2).PadLeft(64, '0')}");
                Console.WriteLine($"Lower Bounds 3: {Convert.ToString((long)lowerBound3, 2).PadLeft(64, '0')}");
                Console.WriteLine($"Upper Bounds 3: {Convert.ToString((long)upperBound3, 2).PadLeft(64, '0')}");
                Console.WriteLine($"Lower Bounds 4: {Convert.ToString((long)lowerBound4, 2).PadLeft(64, '0')}");
                Console.WriteLine($"Upper Bounds 4: {Convert.ToString((long)upperBound4, 2).PadLeft(64, '0')}");
            }
        }
    }
#endif
}
