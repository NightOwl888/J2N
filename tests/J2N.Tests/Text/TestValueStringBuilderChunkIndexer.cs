#if FEATURE_STRINGBUILDER_GETCHUNKS
using NUnit.Framework;
using System;
using System.Text;

namespace J2N.Text
{
    public class TestValueStringBuilderChunkIndexer
    {
        [Test]
        public void AccessingOutOfRangeIndex_ThrowsException()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello, World!");
            using var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act & Assert
            try
            {
                char c = indexer[-1];
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<IndexOutOfRangeException>(ex);
            }

            try
            {
                char c = indexer[stringBuilder.Length];
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<IndexOutOfRangeException>(ex);
            }
        }

        [Test]
        public void AccessingValidIndex_ReturnsCorrectValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello").Append(", ").Append("World!");
            using var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

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
            try
            {

                // Act
                indexer[7] = 'X';
            }
            finally
            {
                indexer.Dispose();
            }

            // Assert
            Assert.AreEqual("Hello, Xorld!", stringBuilder.ToString());
        }


        [Test]
        public void SwitchingToRandomAccessAndBackToSequentialAccess_ReturnsCorrectValues()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello").Append(", ").Append("World!");
            using var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

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
            using var indexerForward = new ValueStringBuilderChunkIndexer(stringBuilder, iterateForward: true);
            using var indexerBackward = new ValueStringBuilderChunkIndexer(stringBuilder, iterateForward: false);

            // Act & Assert
            Assert.IsTrue(indexerForward.IterateForward);
            Assert.IsFalse(indexerBackward.IterateForward);
        }

        [Test]
        public void AccessingIndexInSingleChunk_ReturnsCorrectValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello");
            using var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

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
            using var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

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
            using var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act & Assert
            Assert.AreEqual('W', indexer[27]);
            Assert.AreEqual('!', indexer[32]);
        }

        [Test]
        public void AccessingIndexInMoreThan32Chunks_ReturnsCorrectValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < 40; i++)
            {
                stringBuilder.Append(new string((char)('a' + i), 50));
            }
            using var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

            // Act & Assert
            Assert.AreEqual('b', indexer[99]);
            Assert.AreEqual('r', indexer[860]);
            Assert.AreEqual('t', indexer[999]);
            Assert.AreEqual('\u0081', indexer[32 * 50 + 1]);
            Assert.AreEqual('\u0088', indexer[39 * 50 + 1]);
        }

        [Test]
        public void AccessingIndexMoreThan65535_ReturnsCorrectValue()
        {
            // Arrange
            var stringBuilder = new StringBuilder(new string('a', 70000));
            using var indexer = new ValueStringBuilderChunkIndexer(stringBuilder);

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
            try
            {

                // Act
                indexer[7] = 'X';
                indexer[14] = 'V';
                indexer[20] = 'A';
                indexer[27] = 'Z';
                indexer[32] = '?';
            }
            finally
            {
                indexer.Dispose();
            }

            // Assert
            Assert.AreEqual("Hello, XeautifVl, AmAzing, Zorld?", stringBuilder.ToString());
        }
    }
}
#endif