using System;
using System.Collections.Generic;
using Xunit;
#nullable enable

namespace J2N.Text.Tests
{
    public class OpenStringBuilderBufferTests
    {
        /// <summary>
        /// A test subclass that:
        ///  • tracks which buffers were allocated
        ///  • tracks which buffers were released
        /// </summary>
        private sealed class TestBufferBuilder : MutableTextBuffer
        {
            private readonly HashSet<char[]> allocatedBuffers = new();
            private readonly HashSet<char[]> releasedBuffers = new();

            public int AllocateCount => allocatedBuffers.Count;
            public int ReleaseCount => releasedBuffers.Count;

            public char[]? LastAllocated { get; private set; }
            public char[]? LastReleased { get; private set; }

            public TestBufferBuilder() : base(new char[8]) { }
            public TestBufferBuilder(char[] initial) : base(initial) { }
            public TestBufferBuilder(char[] initial, int initialLength) : base(initial, initialLength) { }

            protected override char[] AllocateBuffer(int minimumLength)
            {
                var buffer = new char[minimumLength];
                allocatedBuffers.Add(buffer);
                LastAllocated = buffer;
                return buffer;
            }

            protected override void ReleaseBuffer(char[] buffer)
            {
                releasedBuffers.Add(buffer);
                LastReleased = buffer;
            }

            // Test helpers
            public bool IsAllocated(char[] buffer) => allocatedBuffers.Contains(buffer);
            public bool IsReleased(char[] buffer) => releasedBuffers.Contains(buffer);

            public int AllocatedCount => allocatedBuffers.Count;
            public int ReleasedCount => releasedBuffers.Count;
        }


        // -------------------------------------------------------------
        //  TESTS
        // -------------------------------------------------------------

        [Fact]
        public void Constructor_UsesInitialBuffer()
        {
            var initial = new char[16];
            var sb = new TestBufferBuilder(initial);

            Assert.Same(initial, sb.RawArray);
            //Assert.True(sb.IsTag(initial) == false, "Initial buffer should not be tagged.");
        }

        [Fact]
        public void Append_CausesAllocationAndRelease_WhenCapacityExceeded()
        {
            var sb = new TestBufferBuilder(new char[4]);

            sb.Append("abcd");
            Assert.Equal(0, sb.AllocatedCount);

            sb.Append("XYZ");   // Trigger growth

            Assert.Equal(1, sb.AllocatedCount);
            Assert.NotNull(sb.LastAllocated);
            Assert.True(sb.IsAllocated(sb.LastAllocated!));

            Assert.Equal(1, sb.ReleasedCount);
            Assert.Same(sb.LastAllocated, sb.RawArray);
        }

        [Fact]
        public void Insert_CausesAllocationAndRelease_WhenCapacityExceeded()
        {
            var sb = new TestBufferBuilder(new char[4]);
            sb.Append("abcd");

            sb.Insert(2, "XYZ"); // Will require new buffer

            Assert.Equal(1, sb.AllocateCount);
            Assert.NotNull(sb.LastAllocated);
            //Assert.True(sb.IsTag(sb.LastAllocated!));
            Assert.True(sb.IsAllocated(sb.LastAllocated!));

            Assert.Equal(1, sb.ReleaseCount);
            Assert.Same(sb.LastAllocated, sb.RawArray);
        }

        [Fact]
        public void Replace_CausesAllocationAndRelease_WhenCapacityExceeded()
        {
            var sb = new TestBufferBuilder(new char[4]);
            sb.Append("abcd");

            // Force replace to expand resulting length
            sb.Replace("b", "LONG");

            Assert.Equal(1, sb.AllocateCount);
            //Assert.True(sb.IsTag(sb.LastAllocated!));
            Assert.True(sb.IsAllocated(sb.LastAllocated!));

            Assert.Equal(1, sb.ReleaseCount);
        }

        [Fact]
        public void AllBuffersAfterFirst_AreFromAllocateBuffer()
        {
            var sb = new TestBufferBuilder(new char[2]);

            sb.Append("abc");
            sb.Append("defghijkl");

            Assert.True(sb.AllocatedCount >= 2);
            Assert.True(sb.IsAllocated(sb.RawArray));
        }

        [Fact]
        public void ReleaseBuffer_IsCalledExactlyOncePerReplacement()
        {
            var sb = new TestBufferBuilder(new char[2]);

            sb.Append("abcd");   // Resize once
            sb.Append("efghij"); // Resize twice
            sb.Append("klmnop"); // Resize thrice

            Assert.Equal(sb.AllocateCount, sb.ReleaseCount);
        }

        [Fact]
        public void InitialBufferIsNotReleased()
        {
            var initial = new char[8];
            var sb = new TestBufferBuilder(initial);

            sb.Append(new string('x', 40)); // Force growth

            Assert.NotSame(initial, sb.RawArray);
            Assert.Same(initial, sb.LastReleased);
        }

        [Fact]
        public void SubsequentBuffersAreReleased()
        {
            var sb = new TestBufferBuilder(new char[2]);

            sb.Append("abcd");   // Grow 1
            var old1 = sb.LastAllocated;

            sb.Append("efghij"); // Grow 2
            var old2 = sb.LastAllocated;

            Assert.Equal(2, sb.ReleaseCount);
            Assert.Same(old1, sb.LastReleased);
        }
    }
}
