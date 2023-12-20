#if FEATURE_STRINGBUILDER_GETCHUNKS
using NUnit.Framework;
using System;
using System.Text;
using ChunkBoundsPacker = J2N.Text.ValueStringBuilderChunkIndexer.ChunkBoundsPacker;

namespace J2N.Text
{
    public class TestValueStringBuilderChunkIndexer
    {
        [Test]
        public void AccessingOutOfRangeIndex_ThrowsException()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello, World!");
            var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act & Assert
            try
            {
                char c = indexer[-1];
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }

            try
            {
                char c = indexer[stringBuilder.Length];
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }

        [Test]
        public void AccessingValidIndex_ReturnsCorrectValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello").Append(", ").Append("World!");
            var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act & Assert
            Assert.AreEqual('H', indexer[0]);
            Assert.AreEqual('e', indexer[1]);
            Assert.AreEqual('o', indexer[4]);
            Assert.AreEqual(',', indexer[5]);
            Assert.AreEqual('W', indexer[7]);
            Assert.AreEqual('d', indexer[11]);
        }

        [Test]
        public void ModifyingValueAtValidIndex_ChangesValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello").Append(", ").Append("World!");
            var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act
            indexer[7] = 'X';

            // Assert
            Assert.AreEqual("Hello, Xorld!", stringBuilder.ToString());
        }


        [Test]
        public void SwitchingToRandomAccessAndBackToSequentialAccess_ReturnsCorrectValues()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello").Append(", ").Append("World!");
            var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act
            indexer.Reset(iterateForward: false);

            // Assert
            Assert.AreEqual('d', indexer[11]);
            Assert.AreEqual('l', indexer[2]);
            Assert.AreEqual('o', indexer[4]);

            // Act
            indexer.Reset();

            // Assert sequential access
            Assert.AreEqual('H', indexer[0]);
            Assert.AreEqual('e', indexer[1]);
            Assert.AreEqual('l', indexer[2]);
            Assert.AreEqual('l', indexer[3]);
            Assert.AreEqual('o', indexer[4]);
            Assert.AreEqual(',', indexer[5]);
            Assert.AreEqual(' ', indexer[6]);
            Assert.AreEqual('W', indexer[7]);
            Assert.AreEqual('o', indexer[8]);
            Assert.AreEqual('r', indexer[9]);
            Assert.AreEqual('l', indexer[10]);
            Assert.AreEqual('d', indexer[11]);

            // Assert random access
            Assert.AreEqual('W', indexer[7]);
            Assert.AreEqual('H', indexer[0]);

            // Assert sequential access
            Assert.AreEqual('H', indexer[0]);
            Assert.AreEqual('e', indexer[1]);
            Assert.AreEqual('l', indexer[2]);
            Assert.AreEqual('l', indexer[3]);
            Assert.AreEqual('o', indexer[4]);
            Assert.AreEqual(',', indexer[5]);
            Assert.AreEqual(' ', indexer[6]);
            Assert.AreEqual('W', indexer[7]);
            Assert.AreEqual('o', indexer[8]);
            Assert.AreEqual('r', indexer[9]);
            Assert.AreEqual('l', indexer[10]);
            Assert.AreEqual('d', indexer[11]);
        }

        [Test]
        public void IterateForwardProperty_ReturnsCorrectValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello").Append(", ").Append("World!");
            var indexerForward = new ValueStringBuilderChunkIndexer(stringBuilder, iterateForward: true);
            var indexerBackward = new ValueStringBuilderChunkIndexer(stringBuilder, iterateForward: false);

            // Act & Assert
            Assert.IsTrue(indexerForward.IterateForward);
            Assert.IsFalse(indexerBackward.IterateForward);
        }

        [Test]
        public void AccessingIndexInSingleChunk_ReturnsCorrectValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello");
            var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act & Assert
            Assert.AreEqual('H', indexer[0]);
            Assert.AreEqual('e', indexer[1]);
            Assert.AreEqual('l', indexer[2]);
            Assert.AreEqual('l', indexer[3]);
            Assert.AreEqual('o', indexer[4]);
        }

        [Test]
        public void AccessingIndexInTwoChunks_ReturnsCorrectValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Hello, ");
            stringBuilder.Append("World!");
            var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act & Assert
            Assert.AreEqual('W', indexer[7]);
            Assert.AreEqual('d', indexer[11]);
        }

        [Test]
        public void AccessingIndexInMoreThanTwoChunks_ReturnsCorrectValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Hello, ");
            stringBuilder.Append("Beautiful, ");
            stringBuilder.Append("Amazing, ");
            stringBuilder.Append("World!");
            var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act & Assert
            Assert.AreEqual('W', indexer[27]);
            Assert.AreEqual('!', indexer[32]);
        }

        [Test]
        public void AccessingIndexInMoreThan16Chunks_ReturnsCorrectValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < 20; i++)
            {
                stringBuilder.Append(new string((char)('a' + i), 50));
            }
            var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act & Assert
            Assert.AreEqual('b', indexer[99]);
            Assert.AreEqual('r', indexer[860]);
            Assert.AreEqual('t', indexer[999]);
        }

        [Test]
        public void AccessingIndexMoreThan65535_ReturnsCorrectValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder(new string('a', 70000));
            var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act & Assert
            Assert.AreEqual('a', indexer[69999]);
        }

        [Test]
        public void ModifyingValuesInMultipleChunks_ChangesValues()
        {
            // Arrange
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Hello, ");
            stringBuilder.Append("Beautiful, ");
            stringBuilder.Append("Amazing, ");
            stringBuilder.Append("World!");
            var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act
            indexer[7] = 'X';
            indexer[14] = 'V';
            indexer[20] = 'A';
            indexer[27] = 'Z';
            indexer[32] = '?';

            // Assert
            Assert.AreEqual("Hello, XeautifVl, AmAzing, Zorld?", stringBuilder.ToString());
        }

        #region ChunkBoundsPacker

        [Test]
        public void SetBoundsAndGetBounds_SingleChunk_Success()
        {
            // Arrange
            var packer = new ChunkBoundsPacker();

            // Act
            packer.SetBounds(0, 0, ChunkBoundsPacker.MaxUpperBound);

            // Assert
            packer.GetBounds(0, out var lowerBound, out var upperBound);
            Assert.AreEqual(0, lowerBound);
            Assert.AreEqual(ChunkBoundsPacker.MaxUpperBound, upperBound);
        }

        [Test]
        public void SetBoundsAndGetBounds_MultipleChunks_Success()
        {
            // Arrange
            var packer = new ChunkBoundsPacker();

            // Act
            for (int i = 0; i < ChunkBoundsPacker.TotalChunks; i++)
            {
                packer.SetBounds(i, i * 100, (i + 1) * 100);
            }

            // Assert
            for (int i = 0; i < ChunkBoundsPacker.TotalChunks; i++)
            {
                packer.GetBounds(i, out var lowerBound, out var upperBound);
                Assert.AreEqual(i * 100, lowerBound);
                Assert.AreEqual((i + 1) * 100, upperBound);
            }
        }

        [Test]
        public void SetBoundsAndGetBounds_MinimumValues_Success()
        {
            // Arrange
            var packer = new ChunkBoundsPacker();

            // Act
            for (int i = 0; i < ChunkBoundsPacker.TotalChunks; i++)
            {
                packer.SetBounds(i, 0, 0);
            }

            // Assert
            for (int i = 0; i < ChunkBoundsPacker.TotalChunks; i++)
            {
                packer.GetBounds(i, out var lowerBound, out var upperBound);
                Assert.AreEqual(0, lowerBound);
                Assert.AreEqual(0, upperBound);
            }
        }

        [Test]
        public void SetBoundsAndGetBounds_MaximumValues_Success()
        {
            // Arrange
            var packer = new ChunkBoundsPacker();

            // Act
            for (int i = 0; i < ChunkBoundsPacker.TotalChunks; i++)
            {
                packer.SetBounds(i, 0, ChunkBoundsPacker.MaxUpperBound);
            }

            // Assert
            for (int i = 0; i < ChunkBoundsPacker.TotalChunks; i++)
            {
                packer.GetBounds(i, out var lowerBound, out var upperBound);
                Assert.AreEqual(0, lowerBound);
                Assert.AreEqual(ChunkBoundsPacker.MaxUpperBound, upperBound);
            }
        }

        [Test]
        public void SetBounds_InvalidChunkIndex_ThrowsException()
        {
            // Arrange
            var packer = new ChunkBoundsPacker();

            // Act & Assert
            try
            {
                packer.SetBounds(-1, 0, 100);
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }

            try
            {
                packer.SetBounds(ChunkBoundsPacker.TotalChunks, 0, 100);
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }

        [Test]
        public void SetBounds_InvalidBounds_ThrowsException()
        {
            // Arrange
            var packer = new ChunkBoundsPacker();

            // Act & Assert
            try
            {
                packer.SetBounds(0, -1, 100);
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }

            try
            {
                packer.SetBounds(0, 0, ChunkBoundsPacker.MaxUpperBound + 1);
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }

            try
            {
                packer.SetBounds(0, 100, -1);
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }

            try
            {
                packer.SetBounds(0, ChunkBoundsPacker.MaxUpperBound + 1, 100);
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }

        [Test]
        public void GetBounds_InvalidChunkIndex_ThrowsException()
        {
            // Arrange
            var packer = new ChunkBoundsPacker();

            // Act & Assert
            try
            {
                packer.GetBounds(-1, out _, out _);
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }

            try
            {
                packer.GetBounds(ChunkBoundsPacker.TotalChunks, out _, out _);
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }

        #endregion ChunkBoundsPacker
    }
}
#endif