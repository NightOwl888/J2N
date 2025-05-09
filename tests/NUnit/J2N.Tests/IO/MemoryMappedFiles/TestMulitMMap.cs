#if NET5_0_OR_GREATER
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Index.Extensions;
using Lucene.Net.Store;
using Lucene.Net.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.IO.MemoryMappedFiles
{
    public class TestMulitMMap : LuceneTestCase
    {
        [Test]
        public virtual void TestRandomChunkSizes()
        {
            int num = AtLeast(10);
            for (int i = 0; i < num; i++)
            {
                AssertChunking(Random, TestUtil.NextInt32(Random, 20, 100));
            }
        }

        private void AssertChunking(Random random, int chunkSize)
        {
            DirectoryInfo path = CreateTempDir("mmap" + chunkSize);
            MMapDirectory mmapDir = new MMapDirectory(path, null, chunkSize);
            // LUCENENET specific - unmap hack not needed
            //// we will map a lot, try to turn on the unmap hack
            //if (MMapDirectory.UNMAP_SUPPORTED)
            //{
            //    mmapDir.UseUnmap = true;
            //}
            MockDirectoryWrapper dir = new MockDirectoryWrapper(random, mmapDir);
            RandomIndexWriter writer = new RandomIndexWriter(random, dir, NewIndexWriterConfig(TEST_VERSION_CURRENT, new MockAnalyzer(random)).SetMergePolicy(NewLogMergePolicy()));
            Document doc = new Document();
            Field docid = NewStringField("docid", "0", Field.Store.YES);
            Field junk = NewStringField("junk", "", Field.Store.YES);
            doc.Add(docid);
            doc.Add(junk);

            int numDocs = 100;
            for (int i = 0; i < numDocs; i++)
            {
                docid.SetStringValue("" + i);
                junk.SetStringValue(TestUtil.RandomUnicodeString(random));
                writer.AddDocument(doc);
            }
            IndexReader reader = writer.GetReader();
            writer.Dispose();

            int numAsserts = AtLeast(100);
            for (int i = 0; i < numAsserts; i++)
            {
                int docID = random.Next(numDocs);
                Assert.AreEqual("" + docID, reader.Document(docID).Get("docid"));
            }
            reader.Dispose();
            dir.Dispose();
        }

    }
}
#endif
