#if FEATURE_ARRAYPOOL
using NUnit.Framework;
using System;
using System.Text;

namespace J2N.Text
{
    public class TestValueStringBuilderArrayPoolIndexer
    {
        [Test]
        public void SequentialAccess_ReadsCorrectly()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            using var indexer = new ValueStringArrayPoolIndexer(sb);
            for (int i = 0; i < sb.Length; i++)
            {
                char expected = sb[i];
                char actual = indexer[i];
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void SequentialAccess_WritesCorrectly()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            var indexer = new ValueStringArrayPoolIndexer(sb);
            try
            {
                for (int i = 0; i < sb.Length; i++)
                {
                    indexer[i] = 'X';
                    char actual = sb[i];
                    Assert.AreEqual('X', actual);
                }
            }
            finally
            {
                indexer.Dispose();
            }
        }

        [Test]
        public void RandomAccess_ReadsCorrectly()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            using var indexer = new ValueStringArrayPoolIndexer(sb);
            Assert.AreEqual('4', indexer[3]);
            Assert.AreEqual('8', indexer[7]);
        }

        [Test]
        public void RandomAccess_WritesCorrectly()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            var indexer = new ValueStringArrayPoolIndexer(sb);
            try
            {
                indexer[3] = 'X';
                indexer[7] = 'Y';

                Assert.AreEqual('X', sb[3]);
                Assert.AreEqual('Y', sb[7]);
            }
            finally
            {
                indexer.Dispose();
            }
        }

        [Test]
        public void ResettingIterators_SwitchingDirections_Successful()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            using var indexer = new ValueStringArrayPoolIndexer(sb);
            Assert.AreEqual(true, indexer.IterateForward);

            // Switching direction from forward to backward
            indexer.Reset(false);
            Assert.AreEqual(false, indexer.IterateForward);

            // Switching direction from backward to forward
            indexer.Reset(true);
            Assert.AreEqual(true, indexer.IterateForward);
        }

        [Test]
        public void InvalidIndex_ThrowsException()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            using var indexer = new ValueStringArrayPoolIndexer(sb);

            try
            {
                char _ = indexer[-1];
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
            try
            {
                char _ = indexer[sb.Length];
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ArgumentOutOfRangeException>(ex);
            }
        }

        [Test]
        public void GetBounds_SingleChunk_ReturnsCorrectBounds()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            using (var indexer = new ValueStringArrayPoolIndexer(sb))
            {
                indexer.Reset(iterateForward: true);
                //indexer.IterateForward = true;

                // Single chunk scenario
                indexer.GetBounds(0, out int lowerBound, out int upperBound);

                Assert.AreEqual(0, lowerBound);
                Assert.AreEqual(sb.Length - 1, upperBound);
            }
        }

        [Test]
        public void GetBounds_MultipleChunks_ReturnsCorrectBounds()
        {
            StringBuilder sb = new StringBuilder("12345678901234567890").Append(new string('a', 1024));
            using (var indexer = new ValueStringArrayPoolIndexer(sb, iterateForward: true))
            {
                int expectedChunkCount = (int)Math.Ceiling((double)sb.Length / ValueStringArrayPoolIndexer.ChunkLength);
                Assert.AreEqual(expectedChunkCount, indexer.ChunkCount);


                for (int i = 0; i < expectedChunkCount; i++)
                {
                    indexer.GetBounds(i, out int lowerBound, out int upperBound);
                    if (i < expectedChunkCount - 1)
                    {
                        Assert.AreEqual(ValueStringArrayPoolIndexer.ChunkLength * i, lowerBound);
                        Assert.AreEqual((ValueStringArrayPoolIndexer.ChunkLength * (i + 1)) - 1, upperBound);
                    }
                    else // Last chunk
                    {
                        Assert.AreEqual(ValueStringArrayPoolIndexer.ChunkLength * i, lowerBound);
                        Assert.AreEqual(sb.Length - 1, upperBound);
                    }
                }
            }
        }

        [Test]
        public void IsWithinBounds_SingleChunk_ReturnsTrueForValidIndex()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            using (var indexer = new ValueStringArrayPoolIndexer(sb))
            {
                indexer.Reset(iterateForward: true);
                //indexer.IterateForward = true;

                // Single chunk scenario
                bool result = indexer.IsWithinBounds(0, 5);
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void IsWithinBounds_SingleChunk_ReturnsFalseForInvalidIndex()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            using (var indexer = new ValueStringArrayPoolIndexer(sb))
            {
                indexer.Reset(iterateForward: true);
                //indexer.IterateForward = true;

                // Single chunk scenario
                bool result = indexer.IsWithinBounds(0, sb.Length);
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void IsWithinBounds_MultipleChunks_ReturnsTrueForValidIndex()
        {
            StringBuilder sb = new StringBuilder("12345678901234567890").Append(new string('a', 1024));
            using (var indexer = new ValueStringArrayPoolIndexer(sb, iterateForward: true))
            {
                // Multiple chunks scenario
                bool result = indexer.IsWithinBounds(1, ValueStringArrayPoolIndexer.ChunkLength * 2 - 10);
                Assert.IsTrue(result);
            }
        }

        [Test]
        public void IsWithinBounds_MultipleChunks_ReturnsFalseForInvalidIndex()
        {
            StringBuilder sb = new StringBuilder("12345678901234567890").Append(new string('a', 1024));
            using (var indexer = new ValueStringArrayPoolIndexer(sb))
            {
                indexer.Reset(iterateForward: true);
                //indexer.IterateForward = true;

                // Multiple chunks scenario
                bool result = indexer.IsWithinBounds(0, sb.Length);
                Assert.IsFalse(result);
            }
        }

        [Test]
        public void ModifyingValuesInMultipleChunks_ChangesValues()
        {
            // Arrange
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Hello, ").Append(new string('a', 1024));
            stringBuilder.Append("Beautiful, ").Append(new string('a', 1024));
            stringBuilder.Append("Amazing, ").Append(new string('a', 1024));
            stringBuilder.Append("World!");
            var indexer = new ValueStringArrayPoolIndexer(stringBuilder);

            // Act
            indexer[7 + 1024] = 'X';
            indexer[14 + 1024] = 'V';
            indexer[20 + 2048] = 'A';
            indexer[27 + 3072] = 'Z';
            indexer[32 + 3072] = '?';

            // Assert
            Assert.AreEqual('X', indexer[7 + 1024]);
            Assert.AreEqual('V', indexer[14 + 1024]);
            Assert.AreEqual('A', indexer[20 + 2048]);
            Assert.AreEqual('Z', indexer[27 + 3072]);
            Assert.AreEqual('?', indexer[32 + 3072]);

            Assert.AreEqual($"Hello, {new string('a', 1024)}XeautifVl, {new string('a', 1024)}AmAzing, {new string('a', 1024)}Zorld?", stringBuilder.ToString());
        }


        [Test]
        public void SwitchingToRandomAccessAndBackToSequentialAccess_ReturnsCorrectValues()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello").Append(", ").Append("World!");
            var indexer = new ValueStringArrayPoolIndexer(stringBuilder);

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
    }
}
#endif
