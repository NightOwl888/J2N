using NUnit.Framework;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace J2N.IO.MemoryMappedFiles
{
    public class TestMemoryMappedViewByteBuffer : TestCase
    {
        FileInfo tmpFile, emptyFile;

        /**
         * A regression test for failing to correctly set capacity of underlying
         * wrapped buffer from a mapped byte buffer.
         */
        [Test]
        public void TestasIntBuffer()
        {
            // Map file
            //FileStream fis = new FileStream(tmpFile.FullName, FileMode.Open);
            //FileChannel fc = fis.getChannel();
            //MappedByteBuffer mmb = fc.map(FileChannel.MapMode.READ_ONLY, 0, fc
            //        .size());

            //using (var fis = MemoryMappedFile.CreateFromFile(tmpFile.FullName))
            using (var fis = new FileStream(tmpFile.FullName, FileMode.Open))
            using (var fc = MemoryMappedFile.CreateFromFile(fis, null, 0, MemoryMappedFileAccess.Read,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                null,
#endif
                HandleInheritability.None, false))
            {
                var mmb = fc.CreateViewByteBuffer(0, (int)fis.Length, MemoryMappedFileAccess.Read);

                int len = mmb.Capacity;
                assertEquals("Got wrong number of bytes", 46, len); //$NON-NLS-1$

                // Read in our 26 bytes
                for (int i = 0; i < 26; i++)
                {
                    byte b = mmb.Get();
                    assertEquals("Got wrong byte value", (byte)'A' + i, b); //$NON-NLS-1$
                }

                // Now convert to an IntBuffer to read our ints
                Int32Buffer ibuffer = mmb.AsInt32Buffer();
                for (int i = 0; i < 5; i++)
                {
                    int val = ibuffer.Get();
                    assertEquals("Got wrong int value", i + 1, val); //$NON-NLS-1$
                }
            }
        }

        [Test]
        public void TestReadOnly()
        {
            using (var fis = new FileStream(tmpFile.FullName, FileMode.Open, FileAccess.Read))
            using (var fc = MemoryMappedFile.CreateFromFile(fis, null, 0, MemoryMappedFileAccess.Read,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                null,
#endif
                HandleInheritability.None, false))
            {
                // Read should work...
                var mmb = fc.CreateViewByteBuffer(0, (int)fis.Length, MemoryMappedFileAccess.Read);

                // test expected exceptions thrown
                try
                {
                    //mmb = fc.map(FileChannel.MapMode.READ_WRITE, 0, fc.size());
                    mmb = fc.CreateViewByteBuffer(0, (int)fis.Length, MemoryMappedFileAccess.ReadWrite);
                    fail("Expected NonWritableChannelException to be thrown");
                }
                catch (UnauthorizedAccessException e)
                {
                    // expected behaviour
                }

                // private (copy on write) works in .NET, but not in Java
                mmb = fc.CreateViewByteBuffer(0, (int)fis.Length, MemoryMappedFileAccess.CopyOnWrite);

                try
                {
                    mmb = fc.CreateViewByteBuffer(0, (int)fis.Length, MemoryMappedFileAccess.Write);
                    fail("Expected NonWritableChannelException to be thrown");
                }
                catch (UnauthorizedAccessException e)
                {
                    // expected behaviour
                }

                try
                {
                    mmb = fc.CreateViewByteBuffer(0, (int)fis.Length, MemoryMappedFileAccess.ReadWriteExecute);
                    fail("Expected NonWritableChannelException to be thrown");
                }
                catch (UnauthorizedAccessException e)
                {
                    // expected behaviour
                }

                try
                {
                    mmb = fc.CreateViewByteBuffer(0, (int)fis.Length, MemoryMappedFileAccess.ReadExecute);
                    fail("Expected NonWritableChannelException to be thrown");
                }
                catch (UnauthorizedAccessException e)
                {
                    // expected behaviour
                }
            }
        }

        /**
         * Regression for HARMONY-6315 - FileChannel.map throws IOException
         * when called with size 0
         * 
         * @throws IOException
         */
        [Test]
        public void TestEmptyBuffer()
        {
            // Map empty file
            //    FileInputStream fis = new FileInputStream(emptyFile);
            //FileChannel fc = fis.getChannel();
            //MappedByteBuffer mmb = fc.map(FileChannel.MapMode.READ_ONLY, 0, fc.size());

            int initialCapacity = 1;
            using (var fis = new FileStream(emptyFile.FullName, FileMode.Open))
            using (var fc = MemoryMappedFile.CreateFromFile(fis, null, initialCapacity, MemoryMappedFileAccess.ReadWrite,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                null,
#endif
                HandleInheritability.None, false))
            {
                var mmb = fc.CreateViewByteBuffer(0, (int)fis.Length, MemoryMappedFileAccess.Read);

                // check non-null
                assertNotNull("MappedByteBuffer created from empty file should not be null", mmb);

                // check capacity is 0
                int len = mmb.Capacity;
                // .NET Port: Capacity cannot be set to 0, or we get an exception when creating the MemoryMappedFile.
                // So, this test is just to ensure we get the same capacity that we set when we created it.
                assertEquals("MappedByteBuffer created from empty file should have capacity the same that was passed into MemoryMappedFile.CreateFromFile()",
                                initialCapacity, len);

                assertFalse("MappedByteBuffer from empty file shouldn't be backed by an array ",
                        mmb.HasArray);

                try
                {
                    byte b = mmb.Get();
                    b = mmb.Get();
                    fail("Calling MappedByteBuffer.Get() past the capacity should throw a BufferUnderflowException");
                }
                catch (BufferUnderflowException e)
                {
                    // expected behaviour
                }
            }
        }

        /**
         * @tests {@link java.nio.MappedByteBuffer#force()}
         */
        [Test]
        public void Test_Flush()
        {
            MemoryMappedViewByteBuffer mmbRead;

            // buffer was not mapped in read/write mode
            using (FileStream fileInputStream = new FileStream(tmpFile.FullName, FileMode.Open, FileAccess.Read))
            {
                using (var fileChannelRead = MemoryMappedFile.CreateFromFile(fileInputStream, null, 0, MemoryMappedFileAccess.Read,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                null,
#endif
                HandleInheritability.None, true))
                {
                    //FileChannel fileChannelRead = fileInputStream.getChannel();
                    //MappedByteBuffer mmbRead = fileChannelRead.map(MapMode.READ_ONLY, 0,
                    //        fileChannelRead.size());

                    mmbRead = fileChannelRead.CreateViewByteBuffer(0, (int)fileInputStream.Length, MemoryMappedFileAccess.Read);
                    mmbRead.Flush();
                }

                //using (FileStream inputStream = new FileStream(tmpFile.FullName, FileMode.Open, FileAccess.Read))
                using (var fileChannelR = MemoryMappedFile.CreateFromFile(fileInputStream, null, 0, MemoryMappedFileAccess.Read,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                null,
#endif
                HandleInheritability.None, false))
                {
                    //FileInputStream inputStream = new FileInputStream(tmpFile);
                    //FileChannel fileChannelR = inputStream.getChannel();
                    //MappedByteBuffer resultRead = fileChannelR.map(MapMode.READ_ONLY, 0,
                    //        fileChannelR.size());
                    MemoryMappedViewByteBuffer resultRead = fileChannelR.CreateViewByteBuffer(0, (int)fileInputStream.Length, MemoryMappedFileAccess.Read);

                    //If this buffer was not mapped in read/write mode, then invoking this method has no effect.
                    assertEquals(
                                    "Invoking force() should have no effect when this buffer was not mapped in read/write mode",
                            mmbRead, resultRead);
                }
            }

            MemoryMappedViewByteBuffer mmbReadWrite;

            // Buffer was mapped in read/write mode
            using (FileStream randomFile = new FileStream(tmpFile.FullName, FileMode.Open, FileAccess.ReadWrite))
            {
                using (var fileChannelReadWrite = MemoryMappedFile.CreateFromFile(randomFile, null, 0, MemoryMappedFileAccess.ReadWrite,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                null,
#endif
                HandleInheritability.None, true))
                {
                    //RandomAccessFile randomFile = new RandomAccessFile(tmpFile, "rw");
                    //FileChannel fileChannelReadWrite = randomFile.getChannel();
                    //MappedByteBuffer mmbReadWrite = fileChannelReadWrite.map(
                    //        FileChannel.MapMode.READ_WRITE, 0, fileChannelReadWrite.size());
                    mmbReadWrite = fileChannelReadWrite.CreateViewByteBuffer(0, (int)randomFile.Length, MemoryMappedFileAccess.ReadWrite);
                    mmbReadWrite.Put((byte)'o');
                    mmbReadWrite.Flush();
                }


                //using (FileStream random = new FileStream(tmpFile.FullName, FileMode.Open, FileAccess.ReadWrite))
                using (var fileChannelRW = MemoryMappedFile.CreateFromFile(randomFile, null, 0, MemoryMappedFileAccess.ReadWrite,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                null,
#endif
                HandleInheritability.None, false))
                {

                    //RandomAccessFile random = new RandomAccessFile(tmpFile, "rw");
                    //FileChannel fileChannelRW = random.getChannel();
                    //MappedByteBuffer resultReadWrite = fileChannelRW.map(
                    //        FileChannel.MapMode.READ_WRITE, 0, fileChannelRW.size());

                    MemoryMappedViewByteBuffer resultReadWrite = fileChannelRW.CreateViewByteBuffer(0, (int)randomFile.Length, MemoryMappedFileAccess.ReadWrite);

                    // Invoking force() will change the buffer
                    assertFalse(mmbReadWrite.Equals(resultReadWrite));
                }
            }

                //fileChannelRead.close();
                //fileChannelR.close();
                //fileChannelReadWrite.close();
                //fileChannelRW.close();
            //}
        }

        //    /**
        //     * @tests {@link java.nio.MappedByteBuffer#load()}
        //     */
        //[Test]
        //    public void Test_load()
        //{
        //    FileInputStream fileInputStream = new FileInputStream(tmpFile);
        //FileChannel fileChannelRead = fileInputStream.getChannel();
        //MappedByteBuffer mmbRead = fileChannelRead.map(MapMode.READ_ONLY, 0,
        //        fileChannelRead.size());

        //assertEquals(mmbRead, mmbRead.load());

        //RandomAccessFile randomFile = new RandomAccessFile(tmpFile, "rw");
        //FileChannel fileChannelReadWrite = randomFile.getChannel();
        //MappedByteBuffer mmbReadWrite = fileChannelReadWrite.map(
        //        FileChannel.MapMode.READ_WRITE, 0, fileChannelReadWrite.size());

        //assertEquals(mmbReadWrite, mmbReadWrite.load());

        //fileChannelRead.close();
        //        fileChannelReadWrite.close();
        //    }

        public override void SetUp()
        {

            tmpFile = new FileInfo(Path.GetTempFileName());

            // Create temp file with 26 bytes and 5 ints
            //tmpFile = File.createTempFile("harmony", "test");  //$NON-NLS-1$//$NON-NLS-2$
            //tmpFile.deleteOnExit();
            using (FileStream fileOutputStream = new FileStream(tmpFile.FullName, FileMode.Open))
            {
                //FileChannel fileChannel = fileOutputStream.getChannel();
                ByteBuffer byteBuffer = ByteBuffer.Allocate(26 + 20);
                for (int i = 0; i < 26; i++)
                {
                    byteBuffer.Put((byte)('A' + i));
                }
                for (int i = 0; i < 5; i++)
                {
                    byteBuffer.PutInt32(i + 1);
                }
                byteBuffer.Rewind();
                fileOutputStream.Write(byteBuffer);

                //fileChannel.write(byteBuffer);
                //fileChannel.close();
            }

            emptyFile = new FileInfo(Path.GetTempFileName());

            //emptyFile = File.createTempFile("harmony", "test");  //$NON-NLS-1$//$NON-NLS-2$
            //emptyFile.deleteOnExit();
        }

        public override void TearDown()
        {
            DeleteFile(tmpFile);
            DeleteFile(emptyFile);
        }


        [Test]
        public void Test_Position()
        {
            var tmp = new FileInfo(Path.GetTempFileName());

            //File tmp = File.createTempFile("hmy", "tmp");
            //tmp.deleteOnExit();
            //RandomAccessFile f = new RandomAccessFile(tmp, "rw");
            //FileChannel ch = f.getChannel();
            //MappedByteBuffer mbb = ch.map(MapMode.READ_WRITE, 0L, 100L);
            //ch.close();

            MemoryMappedViewByteBuffer mbb;
            using (var fis = new FileStream(tmp.FullName, FileMode.Open))
            using (var f = MemoryMappedFile.CreateFromFile(fis, null, 100L, MemoryMappedFileAccess.ReadWrite,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                null,
#endif
                HandleInheritability.None, false))
            {
                mbb = f.CreateViewByteBuffer(0L, 100L, MemoryMappedFileAccess.ReadWrite);
            }

            mbb.PutInt32(1, 1);
            mbb.SetPosition(50);
            mbb.PutInt32(50);

            mbb.Flip();
            mbb.Get();
            assertEquals(1, mbb.GetInt32());

            mbb.SetPosition(50);
            assertEquals(50, mbb.GetInt32());
        }

        public class JDK7 : TestCase
        {
            [Test]
            public void TestBasic()
            {
                byte[] srcData = new byte[20];
                for (int i = 0; i < 20; i++)
                    srcData[i] = 3;
                //File blah = File.createTempFile("blah", null);
                //blah.deleteOnExit();

                var blah = new FileInfo(Path.GetTempFileName());

                using (FileStream fos = new FileStream(blah.FullName, FileMode.Open, FileAccess.ReadWrite))
                {
                    //FileChannel fc = fos.getChannel();
                    //fc.write(ByteBuffer.wrap(srcData));
                    //fc.close();
                    fos.Write(ByteBuffer.Wrap(srcData));
                }

                MemoryMappedViewByteBuffer mbb;

                using (FileStream fis = new FileStream(blah.FullName, FileMode.Open, FileAccess.Read))
                using (var fc = MemoryMappedFile.CreateFromFile(fis, null, 0, MemoryMappedFileAccess.Read,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                    null,
#endif
                    HandleInheritability.None, false))
                {
                    //fc = fis.getChannel();
                    //MappedByteBuffer mbb = fc.map(FileChannel.MapMode.READ_ONLY, 0, 10);
                    mbb = fc.CreateViewByteBuffer(0, 10, MemoryMappedFileAccess.Read);
                    //mbb.load();
                    //mbb.isLoaded();
                    mbb.Flush();
                    assertTrue("Incorrect isReadOnly", mbb.IsReadOnly);

                    // repeat with unaligned position in file
                    //mbb = fc.map(FileChannel.MapMode.READ_ONLY, 1, 10);
                    mbb = fc.CreateViewByteBuffer(0, 10, MemoryMappedFileAccess.Read);
                    //mbb.load();
                    //mbb.isLoaded();
                    mbb.Flush();
                }

                using (FileStream raf = new FileStream(blah.FullName, FileMode.Open, FileAccess.Read))
                using (var fc = MemoryMappedFile.CreateFromFile(raf, null, 0, MemoryMappedFileAccess.Read,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                    null,
#endif
                    HandleInheritability.None, false))
                {
                    //RandomAccessFile raf = new RandomAccessFile(blah, "r");
                    //fc = raf.getChannel();
                    //mbb = fc.map(FileChannel.MapMode.READ_ONLY, 0, 10);
                    mbb = fc.CreateViewByteBuffer(0, 10, MemoryMappedFileAccess.Read);
                    assertTrue("Incorrect isReadOnly", mbb.IsReadOnly);
                }

                using (FileStream raf = new FileStream(blah.FullName, FileMode.Open, FileAccess.ReadWrite))
                using (var fc = MemoryMappedFile.CreateFromFile(raf, null, 0, MemoryMappedFileAccess.ReadWrite,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                    null,
#endif
                    HandleInheritability.None, false))
                {
                    //raf = new RandomAccessFile(blah, "rw");
                    //fc = raf.getChannel();
                    //mbb = fc.map(FileChannel.MapMode.READ_WRITE, 0, 10);
                    assertTrue("Incorrect isReadOnly", mbb.IsReadOnly);
                }

                // clean-up
                mbb = null;
                //System.gc();
                GC.Collect();
                Thread.Sleep(500);
                DeleteFile(blah);
            }

            [Test]
            public void TestFlush() // Force.java
            {
                long fileSize = Random.Next(3 * 1024 * 1024);
                int cut = Random.Next((int)fileSize);
                var file = new FileInfo(Path.GetTempFileName());

                using (FileStream raf = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite))
                using (var fc = MemoryMappedFile.CreateFromFile(raf, null, fileSize, MemoryMappedFileAccess.ReadWrite,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                    null,
#endif
                    HandleInheritability.None, false))
                {
                    MemoryMappedViewByteBuffer mbb = fc.CreateViewByteBuffer(cut, fileSize - cut, MemoryMappedFileAccess.ReadWrite);
                    mbb.Flush();
                }

                // improve chance that mapped buffer will be unmapped
                GC.Collect();
                DeleteFile(file);
            }

            //[Test]
            //public void TestTruncate()
            //{
            //    var blah = new FileInfo(Path.GetTempFileName());



            //    DeleteFile(blah);
            //}

            [Test]
            public void TestZeroMap()
            {
                long fileSize = Random.Next(1024 * 1024);
                int cut = Random.Next((int)fileSize);
                var file = new FileInfo(Path.GetTempFileName());

                using (FileStream raf = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite))
                using (var fc = MemoryMappedFile.CreateFromFile(raf, null, fileSize, MemoryMappedFileAccess.ReadWrite,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                    null,
#endif
                    HandleInheritability.None, false))
                {
                    MemoryMappedViewByteBuffer mbb = fc.CreateViewByteBuffer(cut, 0, MemoryMappedFileAccess.ReadWrite);
                    mbb.Flush();
                }

                // improve chance that mapped buffer will be unmapped
                GC.Collect();
                Thread.Sleep(500);

                DeleteFile(file);
            }
        }

        [Test] // J2N specific
        public void TestDispose_Duplicate()
        {
            long fileSize = Random.Next(1024 * 1024);
            int cut = Random.Next((int)fileSize);
            var file = new FileInfo(Path.GetTempFileName());

            using (FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 2048, FileOptions.DeleteOnClose))
            using (var memoryMappedFile = MemoryMappedFile.CreateFromFile(stream, null, fileSize, MemoryMappedFileAccess.ReadWrite,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                    null,
#endif
                HandleInheritability.None, false))
            {
                MemoryMappedViewByteBuffer mbb = memoryMappedFile.CreateViewByteBuffer(cut, 0, MemoryMappedFileAccess.ReadWrite);

                MemoryMappedViewByteBuffer clone1 = (MemoryMappedViewByteBuffer)mbb.Duplicate();
                MemoryMappedViewByteBuffer clone2 = (MemoryMappedViewByteBuffer)mbb.Slice();
                MemoryMappedViewByteBuffer clone3 = (MemoryMappedViewByteBuffer)clone1.Duplicate();


                Assert.IsTrue(mbb.IsOpen);
                Assert.IsTrue(clone1.IsOpen);
                Assert.IsTrue(clone2.IsOpen);
                Assert.IsTrue(clone3.IsOpen);

                clone1.Dispose();

                Assert.IsTrue(mbb.IsOpen);
                Assert.IsFalse(clone1.IsOpen);
                Assert.IsTrue(clone2.IsOpen);
                Assert.IsTrue(clone3.IsOpen);

                clone3.Dispose();

                Assert.IsTrue(mbb.IsOpen);
                Assert.IsFalse(clone1.IsOpen);
                Assert.IsTrue(clone2.IsOpen);
                Assert.IsFalse(clone3.IsOpen);


                mbb.Dispose();

                Assert.IsFalse(mbb.IsOpen);
                Assert.IsFalse(clone1.IsOpen);
                Assert.IsFalse(clone2.IsOpen);
                Assert.IsFalse(clone3.IsOpen);

                clone2.Dispose();

                Assert.IsFalse(mbb.IsOpen);
                Assert.IsFalse(clone1.IsOpen);
                Assert.IsFalse(clone2.IsOpen);
                Assert.IsFalse(clone3.IsOpen);
            }
        }

        [TestCase(MemoryMappedFileAccess.ReadWrite)] // J2N specific
        [TestCase(MemoryMappedFileAccess.Read)]
        public void TestDispose_Exceptions(MemoryMappedFileAccess memoryMappedFileAccess)
        {
            long fileSize = Random.Next(1024 * 1024);
            int cut = Random.Next((int)fileSize);
            var file = new FileInfo(Path.GetTempFileName());

            using (StreamWriter writer = new StreamWriter(file.FullName, true, Encoding.UTF8))
            {
                for (int i = 0; i < 2048; i++)
                {
                    writer.Write((byte)0);
                }
            }

            using (FileStream stream = new FileStream(file.FullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 2048, FileOptions.DeleteOnClose))
            using (var memoryMappedFile = MemoryMappedFile.CreateFromFile(stream, null, fileSize, MemoryMappedFileAccess.ReadWrite,
#if FEATURE_MEMORYMAPPEDFILESECURITY
                    null,
#endif
                HandleInheritability.None, false))
            {
                MemoryMappedViewByteBuffer mbb = memoryMappedFile.CreateViewByteBuffer(cut, 2048, memoryMappedFileAccess);

                MemoryMappedViewByteBuffer duplicate = (MemoryMappedViewByteBuffer)mbb.Duplicate();
                MemoryMappedViewByteBuffer slice = (MemoryMappedViewByteBuffer)mbb.Slice();
                CharBuffer charBuffer = mbb.AsCharBuffer();
                Int16Buffer int16Buffer = mbb.AsInt16Buffer();
                Int32Buffer int32Buffer = mbb.AsInt32Buffer();
                Int64Buffer int64Buffer = mbb.AsInt64Buffer();
                SingleBuffer singleBuffer = mbb.AsSingleBuffer();
                DoubleBuffer doubleBuffer = mbb.AsDoubleBuffer();

                mbb.Dispose();
                Assert.IsFalse(mbb.IsOpen);

                CheckDisposedByteBuffer(mbb);
                CheckDisposedByteBuffer(duplicate);
                CheckDisposedByteBuffer(slice);

                CheckDisposedCharBuffer(charBuffer);
                CheckDisposedInt16Buffer(int16Buffer);
                CheckDisposedInt32Buffer(int32Buffer);
                CheckDisposedInt64Buffer(int64Buffer);
                CheckDisposedSingleBuffer(singleBuffer);
                CheckDisposedDoubleBuffer(doubleBuffer);
            }
        }


        private static void CheckDisposedBuffer(Buffer buffer)
        {
            //Assert.Throws<ObjectDisposedException>(() => buffer.Clear());
            Assert.Throws<ObjectDisposedException>(() => buffer.Equals(buffer));
            //Assert.Throws<ObjectDisposedException>(() => buffer.Flip());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetHashCode());
            //Assert.Throws<ObjectDisposedException>(() => buffer.Mark());
            //Assert.Throws<ObjectDisposedException>(() => buffer.Reset());
            //Assert.Throws<ObjectDisposedException>(() => buffer.Rewind());
        }


        private static void CheckDisposedByteBuffer(MemoryMappedViewByteBuffer buffer)
        {
            CheckDisposedByteBuffer((ByteBuffer)buffer);
            Assert.Throws<ObjectDisposedException>(() => buffer.Flush());
        }


        private static void CheckDisposedByteBuffer(ByteBuffer buffer)
        {
            Assert.Throws<ObjectDisposedException>(() => buffer.AsCharBuffer());
            Assert.Throws<ObjectDisposedException>(() => buffer.AsDoubleBuffer());
            Assert.Throws<ObjectDisposedException>(() => buffer.AsInt16Buffer());
            Assert.Throws<ObjectDisposedException>(() => buffer.AsInt32Buffer());
            Assert.Throws<ObjectDisposedException>(() => buffer.AsInt64Buffer());
            Assert.Throws<ObjectDisposedException>(() => buffer.AsReadOnlyBuffer());
            Assert.Throws<ObjectDisposedException>(() => buffer.AsSingleBuffer());
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.Compact());
            Assert.Throws<ObjectDisposedException>(() => buffer.CompareTo(buffer));
            Assert.Throws<ObjectDisposedException>(() => buffer.Duplicate());
            Assert.Throws<ObjectDisposedException>(() => buffer.Get());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetChar());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetDouble());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetInt16());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetInt32());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetInt64());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetSingle());
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.PutChar('\0'));
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.PutDouble(0D));
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.PutInt16(0));
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.PutInt32(0));
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.PutInt64(0L));
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.PutSingle(0F));
            Assert.Throws<ObjectDisposedException>(() => buffer.Slice());

            CheckDisposedBuffer(buffer);
        }



        private static void CheckDisposedCharBuffer(CharBuffer buffer)
        {
            Assert.Throws<ObjectDisposedException>(() => buffer.AsReadOnlyBuffer());
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.Compact());
            Assert.Throws<ObjectDisposedException>(() => buffer.CompareTo(buffer));
            Assert.Throws<ObjectDisposedException>(() => buffer.Duplicate());
            Assert.Throws<ObjectDisposedException>(() => buffer.Get());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetHashCode());
            Assert.Throws<ObjectDisposedException>(() => buffer.Slice());

            CheckDisposedBuffer(buffer);
        }

        private static void CheckDisposedDoubleBuffer(DoubleBuffer buffer)
        {
            Assert.Throws<ObjectDisposedException>(() => buffer.AsReadOnlyBuffer());
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.Compact());
            Assert.Throws<ObjectDisposedException>(() => buffer.CompareTo(buffer));
            Assert.Throws<ObjectDisposedException>(() => buffer.Duplicate());
            Assert.Throws<ObjectDisposedException>(() => buffer.Get());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetHashCode());
            Assert.Throws<ObjectDisposedException>(() => buffer.Slice());

            CheckDisposedBuffer(buffer);
        }

        private static void CheckDisposedInt16Buffer(Int16Buffer buffer)
        {
            Assert.Throws<ObjectDisposedException>(() => buffer.AsReadOnlyBuffer());
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.Compact());
            Assert.Throws<ObjectDisposedException>(() => buffer.CompareTo(buffer));
            Assert.Throws<ObjectDisposedException>(() => buffer.Duplicate());
            Assert.Throws<ObjectDisposedException>(() => buffer.Get());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetHashCode());
            Assert.Throws<ObjectDisposedException>(() => buffer.Slice());

            CheckDisposedBuffer(buffer);
        }

        private static void CheckDisposedInt32Buffer(Int32Buffer buffer)
        {
            Assert.Throws<ObjectDisposedException>(() => buffer.AsReadOnlyBuffer());
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.Compact());
            Assert.Throws<ObjectDisposedException>(() => buffer.CompareTo(buffer));
            Assert.Throws<ObjectDisposedException>(() => buffer.Duplicate());
            Assert.Throws<ObjectDisposedException>(() => buffer.Get());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetHashCode());
            Assert.Throws<ObjectDisposedException>(() => buffer.Slice());

            CheckDisposedBuffer(buffer);
        }

        private static void CheckDisposedInt64Buffer(Int64Buffer buffer)
        {
            Assert.Throws<ObjectDisposedException>(() => buffer.AsReadOnlyBuffer());
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.Compact());
            Assert.Throws<ObjectDisposedException>(() => buffer.CompareTo(buffer));
            Assert.Throws<ObjectDisposedException>(() => buffer.Duplicate());
            Assert.Throws<ObjectDisposedException>(() => buffer.Get());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetHashCode());
            Assert.Throws<ObjectDisposedException>(() => buffer.Slice());

            CheckDisposedBuffer(buffer);
        }

        private static void CheckDisposedSingleBuffer(SingleBuffer buffer)
        {
            Assert.Throws<ObjectDisposedException>(() => buffer.AsReadOnlyBuffer());
            AssertThrowsObjectDisposedOrNotSupportedException(() => buffer.Compact());
            Assert.Throws<ObjectDisposedException>(() => buffer.CompareTo(buffer));
            Assert.Throws<ObjectDisposedException>(() => buffer.Duplicate());
            Assert.Throws<ObjectDisposedException>(() => buffer.Get());
            Assert.Throws<ObjectDisposedException>(() => buffer.GetHashCode());
            Assert.Throws<ObjectDisposedException>(() => buffer.Slice());

            CheckDisposedBuffer(buffer);
        }

        private static void AssertThrowsObjectDisposedOrNotSupportedException(TestDelegate code)
        {
            var ex = Assert.Catch<Exception>(code);
            Assert.That(ex, Is.InstanceOf<ObjectDisposedException>().Or.InstanceOf<NotSupportedException>());
        }
    }
}
