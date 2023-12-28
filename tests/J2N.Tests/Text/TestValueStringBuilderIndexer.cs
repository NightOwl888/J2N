using NUnit.Framework;
using System;
using System.Text;

namespace J2N.Text
{
    public class TestValueStringBuilderIndexer
    {
        [Test]
        public void SequentialAccess_ReadsCorrectly()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            using var indexer = new ValueStringBuilderIndexer(sb);
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
            var indexer = new ValueStringBuilderIndexer(sb);
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
            using var indexer = new ValueStringBuilderIndexer(sb);
            Assert.AreEqual('4', indexer[3]);
            Assert.AreEqual('8', indexer[7]);
        }

        [Test]
        public void RandomAccess_WritesCorrectly()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            var indexer = new ValueStringBuilderIndexer(sb);
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
        public void InvalidIndex_ThrowsException()
        {
            StringBuilder sb = new StringBuilder("1234567890");
            using var indexer = new ValueStringBuilderIndexer(sb);

            try
            {
                char _ = indexer[-1];
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<IndexOutOfRangeException>(ex);
            }
            try
            {
                char _ = indexer[sb.Length];
                Assert.Fail("Expected exception not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<IndexOutOfRangeException>(ex);
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
            var indexer = new ValueStringBuilderIndexer(stringBuilder);
            try
            {
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

                Assert.AreEqual('X', stringBuilder[7 + 1024]);
                Assert.AreEqual('V', stringBuilder[14 + 1024]);
                Assert.AreEqual('A', stringBuilder[20 + 2048]);
                Assert.AreEqual('Z', stringBuilder[27 + 3072]);
                Assert.AreEqual('?', stringBuilder[32 + 3072]);
            }
            finally
            {
                indexer.Dispose();
            }

            Assert.AreEqual($"Hello, {new string('a', 1024)}XeautifVl, {new string('a', 1024)}AmAzing, {new string('a', 1024)}Zorld?", stringBuilder.ToString());
        }

        [Test]
        public void SwitchingToRandomAccessAndBackToSequentialAccess_ReturnsCorrectValues()
        {
            // Arrange
            var stringBuilder = new StringBuilder("Hello").Append(", ").Append("World!");
            using var indexer = new ValueStringBuilderIndexer(stringBuilder);

            // Act
            // Assert random access
            Assert.AreEqual('d', indexer[11]);
            Assert.AreEqual('l', indexer[2]);
            Assert.AreEqual('o', indexer[4]);

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
