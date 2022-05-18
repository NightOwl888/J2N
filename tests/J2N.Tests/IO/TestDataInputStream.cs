using J2N.IO;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace J2N.IO
{
    public class TestDataInputStream : TestCase
    {
        private static readonly Encoding defaultJavaStringEncoding = Encoding.GetEncoding("ISO-8859-1");

        private DataOutputStream os;

        private DataInputStream dis;

        private MemoryStream bos;

        private const string unihw = "\u0048\u0065\u006C\u006C\u006F\u0020\u0057\u006F\u0072\u006C\u0064";

        public string fileString = "Test_All_Tests\nTest_java_io_BufferedInputStream\nTest_java_io_BufferedOutputStream\nTest_java_io_ByteArrayInputStream\nTest_java_io_ByteArrayOutputStream\nTest_DataInputStream\n";

        /**
         * @tests java.io.DataInputStream#DataInputStream(java.io.InputStream)
         */
        [Test]
        public void Test_ConstructorLjava_io_InputStream()
        {
            try
            {
                os.WriteChar('t');
                os.Dispose();
                openDataInputStream();
            }
            finally
            {
                dis.Dispose();
            }
        }

        /**
         * @tests java.io.DataInputStream#read(byte[])
         */
        [Test]
        public void Test_read_B()
        {
            os.Write(defaultJavaStringEncoding.GetBytes(fileString));
            os.Dispose();
            openDataInputStream();
            byte[] rbytes = new byte[fileString.Length];
            dis.Read(rbytes);
            assertTrue("Incorrect data read", defaultJavaStringEncoding.GetString(rbytes, 0, fileString
                    .Length).Equals(fileString));
        }

        /**
         * @tests java.io.DataInputStream#read(byte[], int, int)
         */
        [Test]
        public void Test_read_BII()
        {
            os.Write(defaultJavaStringEncoding.GetBytes(fileString));
            os.Dispose();
            openDataInputStream();
            byte[] rbytes = new byte[fileString.Length];
            dis.Read(rbytes, 0, rbytes.Length);
            assertTrue("Incorrect data read", defaultJavaStringEncoding.GetString(rbytes, 0, fileString
                    .Length).Equals(fileString));
        }

        /**
         * @tests java.io.DataInputStream#readBoolean()
         */
        [Test]
        public void Test_readBoolean()
        {
            os.WriteBoolean(true);
            os.Dispose();
            openDataInputStream();
            assertTrue("Incorrect boolean written", dis.ReadBoolean());
        }

        /**
         * @tests java.io.DataInputStream#readByte()
         */
        [Test]
        public void Test_readByte()
        {
            os.WriteByte((byte)127);
            os.Dispose();
            openDataInputStream();
            assertTrue("Incorrect byte read", dis.ReadByte() == (byte)127);
        }

        /**
         * @tests java.io.DataInputStream#readChar()
         */
        [Test]
        public void Test_readChar()
        {
            os.WriteChar('t');
            os.Dispose();
            openDataInputStream();
            assertEquals("Incorrect char read", 't', dis.ReadChar());
        }

        /**
         * @tests java.io.DataInputStream#readDouble()
         */
        [Test]
        public void Test_readDouble()
        {
            os.WriteDouble(2345.76834720202);
            os.Dispose();
            openDataInputStream();
            assertEquals("Incorrect double read", 2345.76834720202, dis
                        .ReadDouble());
        }

        /**
         * @tests java.io.DataInputStream#readFloat()
         */
        [Test]
        public void Test_readFloat()
        {
            os.WriteSingle(29.08764f);
            os.Dispose();
            openDataInputStream();
            assertTrue("Incorrect float read", dis.ReadSingle() == 29.08764f);
        }

        /**
         * @tests java.io.DataInputStream#readFully(byte[])
         */
        [Test]
        public void Test_readFully_B()
        {
            os.Write(defaultJavaStringEncoding.GetBytes(fileString));
            os.Dispose();
            openDataInputStream();
            byte[] rbytes = new byte[fileString.Length];
            dis.ReadFully(rbytes);
            assertTrue("Incorrect data read", defaultJavaStringEncoding.GetString(rbytes, 0, fileString
                    .Length).Equals(fileString));
        }

        /**
         * @tests java.io.DataInputStream#readFully(byte[], int, int)
         */
        [Test]
        public void Test_readFully_BII()
        {
            os.Write(defaultJavaStringEncoding.GetBytes(fileString));
            os.Dispose();
            openDataInputStream();
            byte[] rbytes = new byte[fileString.Length];
            dis.ReadFully(rbytes, 0, fileString.Length);
            assertTrue("Incorrect data read", defaultJavaStringEncoding.GetString(rbytes, 0, fileString.Length).Equals(fileString));
        }

        /**
         * @tests java.io.DataInputStream#readFully(byte[], int, int)
         */
        [Test]
        public void Test_readFully_BII_Exception()
        {
            DataInputStream @is = new DataInputStream(new MemoryStream(
                        new byte[fileString.Length]));

            byte[] byteArray = new byte[fileString.Length];

            try
            {
                @is.ReadFully(byteArray, -1, -1);
                fail("should throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            try
            {
                @is.ReadFully(byteArray, 0, -1);
                fail("should throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            try
            {
                @is.ReadFully(byteArray, 1, -1);
                fail("should throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            @is.ReadFully(byteArray, -1, 0);
            @is.ReadFully(byteArray, 0, 0);
            @is.ReadFully(byteArray, 1, 0);

            try
            {
                @is.ReadFully(byteArray, -1, 1);
                fail("should throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            @is.ReadFully(byteArray, 0, 1);
            @is.ReadFully(byteArray, 1, 1);
            try
            {
                @is.ReadFully(byteArray, 0, int.MaxValue);
                fail("should throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }
        }

        /**
         * @tests java.io.DataInputStream#readFully(byte[], int, int)
         */
         [Test]
        public void Test_readFully_BII_NullArray()
        {
            DataInputStream @is = new DataInputStream(new MemoryStream(
                        new byte[fileString.Length]));

            byte[] nullByteArray = null;

            try
            {
                @is.ReadFully(nullByteArray, -1, -1);
                fail("should throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            try
            {
                @is.ReadFully(nullByteArray, 0, -1);
                fail("should throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            try
            {
                @is.ReadFully(nullByteArray, 1, -1);
                fail("should throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // expected
            }

            @is.ReadFully(nullByteArray, -1, 0);
            @is.ReadFully(nullByteArray, 0, 0);
            @is.ReadFully(nullByteArray, 1, 0);

            try
            {
                @is.ReadFully(nullByteArray, -1, 1);
                fail("should throw ArgumentNullException");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }

            try
            {
                @is.ReadFully(nullByteArray, 0, 1);
                fail("should throw ArgumentNullException");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }

            try
            {
                @is.ReadFully(nullByteArray, 1, 1);
                fail("should throw ArgumentNullException");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }

            try
            {
                @is.ReadFully(nullByteArray, 0, int.MaxValue);
                fail("should throw ArgumentNullException");
            }
            catch (ArgumentNullException e)
            {
                // expected
            }
        }

        // J2N: Not applicable, as we followed the .NET convention of putting a guard clause on the constructor
        // so a null stream is disallowed.
        /////**
        //// * @tests java.io.DataInputStream#readFully(byte[], int, int)
        //// */
        ////[Test]
        ////public void Test_readFully_BII_NullStream()
        ////{
        ////    DataInputStream @is = new DataInputStream(null);
        ////    byte[] byteArray = new byte[fileString.Length];

        ////    try
        ////    {
        ////        @is.ReadFully(byteArray, -1, -1);
        ////        fail("should throw ArgumentOutOfRangeException");
        ////    }
        ////    catch (ArgumentOutOfRangeException e)
        ////    {
        ////        // expected
        ////    }

        ////    try
        ////    {
        ////        @is.ReadFully(byteArray, 0, -1);
        ////        fail("should throw ArgumentOutOfRangeException");
        ////    }
        ////    catch (ArgumentOutOfRangeException e)
        ////    {
        ////        // expected
        ////    }

        ////    try
        ////    {
        ////        @is.ReadFully(byteArray, 1, -1);
        ////        fail("should throw ArgumentOutOfRangeException");
        ////    }
        ////    catch (ArgumentOutOfRangeException e)
        ////    {
        ////        // expected
        ////    }

        ////    @is.ReadFully(byteArray, -1, 0);
        ////    @is.ReadFully(byteArray, 0, 0);
        ////    @is.ReadFully(byteArray, 1, 0);

        ////    try
        ////    {
        ////        @is.ReadFully(byteArray, -1, 1);
        ////        fail("should throw ArgumentNullException");
        ////    }
        ////    catch (ArgumentNullException e)
        ////    {
        ////        // expected
        ////    }

        ////    try
        ////    {
        ////        @is.ReadFully(byteArray, 0, 1);
        ////        fail("should throw ArgumentNullException");
        ////    }
        ////    catch (ArgumentNullException e)
        ////    {
        ////        // expected
        ////    }

        ////    try
        ////    {
        ////        @is.ReadFully(byteArray, 1, 1);
        ////        fail("should throw ArgumentNullException");
        ////    }
        ////    catch (ArgumentNullException e)
        ////    {
        ////        // expected
        ////    }

        ////    try
        ////    {
        ////        @is.ReadFully(byteArray, 0, int.MaxValue);
        ////        fail("should throw ArgumentNullException");
        ////    }
        ////    catch (ArgumentNullException e)
        ////    {
        ////        // expected
        ////    }
        ////}

        // J2N: Not applicable, as we followed the .NET convention of putting a guard clause on the constructor
        // so a null stream is disallowed.
        /////**
        //// * @tests java.io.DataInputStream#readFully(byte[], int, int)
        //// */
        ////[Test]
        ////public void Test_readFully_BII_NullStream_NullArray()
        ////{
        ////    DataInputStream @is = new DataInputStream(null);
        ////    byte[] nullByteArray = null;

        ////    try
        ////    {
        ////        @is.ReadFully(nullByteArray, -1, -1);
        ////        fail("should throw ArgumentOutOfRangeException");
        ////    }
        ////    catch (ArgumentOutOfRangeException e)
        ////    {
        ////        // expected
        ////    }

        ////    try
        ////    {
        ////        @is.ReadFully(nullByteArray, 0, -1);
        ////        fail("should throw ArgumentOutOfRangeException");
        ////    }
        ////    catch (ArgumentOutOfRangeException e)
        ////    {
        ////        // expected
        ////    }

        ////    try
        ////    {
        ////        @is.ReadFully(nullByteArray, 1, -1);
        ////        fail("should throw ArgumentOutOfRangeException");
        ////    }
        ////    catch (ArgumentOutOfRangeException e)
        ////    {
        ////        // expected
        ////    }

        ////    @is.ReadFully(nullByteArray, -1, 0);
        ////    @is.ReadFully(nullByteArray, 0, 0);
        ////    @is.ReadFully(nullByteArray, 1, 0);

        ////    try
        ////    {
        ////        @is.ReadFully(nullByteArray, -1, 1);
        ////        fail("should throw ArgumentNullException");
        ////    }
        ////    catch (ArgumentNullException e)
        ////    {
        ////        // expected
        ////    }

        ////    try
        ////    {
        ////        @is.ReadFully(nullByteArray, 0, 1);
        ////        fail("should throw ArgumentNullException");
        ////    }
        ////    catch (ArgumentNullException e)
        ////    {
        ////        // expected
        ////    }

        ////    try
        ////    {
        ////        @is.ReadFully(nullByteArray, 1, 1);
        ////        fail("should throw ArgumentNullException");
        ////    }
        ////    catch (ArgumentNullException e)
        ////    {
        ////        // expected
        ////    }

        ////    try
        ////    {
        ////        @is.ReadFully(nullByteArray, 0, int.MaxValue);
        ////        fail("should throw ArgumentNullException");
        ////    }
        ////    catch (ArgumentNullException e)
        ////    {
        ////        // expected
        ////    }
        ////}

        /**
         * @tests java.io.DataInputStream#readInt()
         */
        [Test]
        public void Test_readInt()
        {
            os.WriteInt32(768347202);
            os.Dispose();
            openDataInputStream();
            assertEquals("Incorrect int read", 768347202, dis.ReadInt32());
        }

        /**
         * @tests java.io.DataInputStream#readLine()
         */
        [Test]
        public void Test_readLine()
        {
            os.WriteBytes("Hello");
            os.Dispose();
            openDataInputStream();
            String line = dis.ReadLine();
            assertTrue("Incorrect line read: " + line, line.Equals("Hello"));
        }

        /**
         * @tests java.io.DataInputStream#readLong()
         */
        [Test]
        public void Test_readLong()
        {
            os.WriteInt64(9875645283333L);
            os.Dispose();
            openDataInputStream();
            assertEquals("Incorrect long read", 9875645283333L, dis.ReadInt64());
        }

        /**
         * @tests java.io.DataInputStream#readShort()
         */
        [Test]
        public void Test_readShort()
        {
            os.WriteInt16(9875);
            os.Dispose();
            openDataInputStream();
            assertTrue("Incorrect short read", dis.ReadInt16() == (short)9875);
        }

        /**
         * @tests java.io.DataInputStream#readUnsignedByte()
         */
        [Test]
        public void Test_readUnsignedByte()
        {
            os.WriteByte(unchecked((byte)-127));
            os.Dispose();
            openDataInputStream();
            assertEquals("Incorrect byte read", 129, dis.ReadByte());
        }

        /**
         * @tests java.io.DataInputStream#readUnsignedShort()
         */
        [Test]
        public void Test_readUnsignedShort()
        {
            os.WriteInt16(9875);
            os.Dispose();
            openDataInputStream();
            assertEquals("Incorrect short read", 9875, dis.ReadUInt16());
        }

        /**
         * @tests java.io.DataInputStream#readUTF()
         */
        [Test]
        public void Test_readUTF()
        {
            os.WriteUTF(unihw);
            os.Dispose();
            openDataInputStream();
            assertTrue("Failed to write string in UTF format",
                        dis.Available == unihw.Length + 2);
            assertTrue("Incorrect string read", dis.ReadUTF().Equals(unihw));
        }

        internal class FakeDataInputStream : IDataInput
        {
            public bool ReadBoolean()
            {
                return false;
            }

            public int ReadSByte()
            {
                return (byte)0;
            }

            public char ReadChar()
            {
                return (char)0;
            }

            public double ReadDouble()
            {
                return 0.0;
            }

            public float ReadSingle()
            {
                return (float)0.0;
            }

            public void ReadFully(byte[] buffer)
            {
            }

            public void ReadFully(byte[] buffer, int offset, int count)
            {
            }

            public int ReadInt32()
            {
                return 0;
            }

            public String ReadLine()
            {
                return null;
            }

            public long ReadInt64()
            {
                return (long)0;
            }

            public short ReadInt16()
            {
                return (short)0;
            }

            public int ReadByte()
            {
                return 0;
            }

            public int ReadUInt16()
            {
                return 0;
            }

            public String ReadUTF()
            {
                return DataInputStream.ReadUTF(this);
            }

            public int SkipBytes(int count)
            {
                return 0;
            }
        }

        /**
         * @tests java.io.DataInputStream#readUTF(java.io.DataInput)
         */
        [Test]
        public void Test_readUTFLjava_io_DataInput()
        {
            os.WriteUTF(unihw);
            os.Dispose();
            openDataInputStream();
            assertTrue("Failed to write string in UTF format",
                        dis.Available == unihw.Length + 2);
            assertTrue("Incorrect string read", DataInputStream.ReadUTF(dis)
                        .Equals(unihw));

            // Regression test for HARMONY-5336
            new FakeDataInputStream().ReadUTF();
        }

        /**
         * @tests java.io.DataInputStream#skipBytes(int)
         */
        [Test]
        public void Test_skipBytesI()
        {
            byte[] fileBytes = defaultJavaStringEncoding.GetBytes(fileString);
            os.Write(fileBytes);
            os.Dispose();
            openDataInputStream();
            dis.SkipBytes(100);
            byte[] rbytes = new byte[fileString.Length];
            dis.Read(rbytes, 0, 50);
            dis.Dispose();
            assertTrue("Incorrect data read", defaultJavaStringEncoding.GetString(rbytes, 0, 50)
                    .Equals(fileString.Substring(100, 150 - 100))); // J2N: end - start

            int skipped = 0;
            openDataInputStream();
            try
            {
                skipped = dis.SkipBytes(50000);
            }
            catch (EndOfStreamException e)
            {
            }
            assertTrue("Skipped should report " + fileString.Length + " not "
                    + skipped, skipped == fileString.Length);
        }

        private void openDataInputStream()
        {
            dis = new DataInputStream(new MemoryStream(bos.ToArray()));
        }

        /**
         * Sets up the fixture, for example, open a network connection. This method
         * is called before a test is executed.
         */
        public override void SetUp()
        {
            bos = new MemoryStream();
            os = new DataOutputStream(bos);
        }

        /**
         * Tears down the fixture, for example, close a network connection. This
         * method is called after a test is executed.
         */
        public override void TearDown()
        {
            try
            {
                os.Dispose();
            }
            catch (Exception e)
            {
            }
            try
            {
                dis.Dispose();
            }
            catch (Exception e)
            {
            }
        }



        [Test]
        public void TestReadFully()
        {
            const string READFULLY_TEST_FILE = "ReadFully.txt";
            int fileLength;
            Stream @in;

            // Read one time to measure the length of the file (it may be different 
            // on different operating systems because of line endings)
            using (@in = GetType().getResourceAsStream(READFULLY_TEST_FILE))
            {
                using (var ms = new MemoryStream())
                {
                    @in.CopyTo(ms);
                    fileLength = ms.ToArray().Length;
                }
            }

            // Declare the buffer one byte too large
            byte[] buffer = new byte[fileLength + 1];
            @in = GetType().getResourceAsStream(READFULLY_TEST_FILE);
            DataInputStream dis;
            using (dis = new DataInputStream(@in))
            {
                // Read once for real (to the exact length)
                dis.ReadFully(buffer, 0, fileLength);
            }

            // Read past the end of the stream
            @in = GetType().getResourceAsStream(READFULLY_TEST_FILE);
            dis = new DataInputStream(@in);
            bool caughtException = false;
            try
            {
                // Using the buffer length (that is 1 byte too many)
                // should generate EndOfStreamException.
                dis.ReadFully(buffer, 0, buffer.Length);
            }
            catch (EndOfStreamException ie)
            {
                caughtException = true;
            }
            finally
            {
                dis.Dispose();
                if (!caughtException)
                    fail("Test failed");
            }

            // Ensure we get an IndexOutOfRangeException exception when length is negative
            @in = GetType().getResourceAsStream(READFULLY_TEST_FILE);
            dis = new DataInputStream(@in);
            caughtException = false;
            try
            {
                dis.ReadFully(buffer, 0, -20);
            }
            catch (ArgumentOutOfRangeException ie)
            {
                caughtException = true;
            }
            finally
            {
                dis.Dispose();
                if (!caughtException)
                    fail("Test failed");
            }
        }

        [Test]
        public void TestReadLinePushback()
        {
            using (MemoryStream pis = new MemoryStream(Encoding.UTF8.GetBytes("\r")))
            {
                DataInputStream dis = new DataInputStream(pis);
                string line = dis.ReadLine();
                if (line == null)
                {
                    fail("Got null, should return empty line");
                }

                long count = pis.Length - (line.Length + 1 /*account for the newline*/);

                if (count != 0)
                {
                    fail("Test failed: available() returns "
                                         + count + " when the file is empty");
                }
            }
        }

        [Test]
        public void TestReadUTF()
        {
            for (int i = 0; i < TEST_ITERATIONS; i++)
            {
                try
                {
                    WriteAndReadAString();
                }
                catch (FormatException utfdfe)
                {
                    if (utfdfe.Message == null)
                        fail("vague exception thrown");
                }
                catch (EndOfStreamException eofe)
                {
                    // These are rare and beyond the scope of the test
                }
            }
        }


        private static readonly int TEST_ITERATIONS = 1000;

        private static readonly int A_NUMBER_NEAR_65535 = 60000;

        private static readonly int MAX_CORRUPTIONS_PER_CYCLE = 3;

        private static void WriteAndReadAString()
        {
            // Write out a string whose UTF-8 encoding is quite possibly
            // longer than 65535 bytes
            int length = Random.Next(A_NUMBER_NEAR_65535) + 1;
            MemoryStream baos = new MemoryStream();
            StringBuilder testBuffer = new StringBuilder();
            for (int i = 0; i < length; i++)
                testBuffer.Append((char)Random.Next());
            string testString = testBuffer.ToString();
            DataOutputStream dos = new DataOutputStream(baos);
            dos.WriteUTF(testString);

            // Corrupt the data to produce malformed characters
            byte[] testBytes = baos.ToArray();
            int dataLength = testBytes.Length;
            int corruptions = Random.Next(MAX_CORRUPTIONS_PER_CYCLE);
            for (int i = 0; i < corruptions; i++)
            {
                int index = Random.Next(dataLength);
                testBytes[index] = (byte)Random.Next();
            }

            // Pay special attention to mangling the end to produce
            // partial characters at end
            testBytes[dataLength - 1] = (byte)Random.Next();
            testBytes[dataLength - 2] = (byte)Random.Next();

            // Attempt to decode the bytes back into a String
            MemoryStream bais = new MemoryStream(testBytes);
            DataInputStream dis = new DataInputStream(bais);
            dis.ReadUTF();
        }

        [Test]
        public void TestSkipBytes()
        {
            DataInputStream dis = new DataInputStream(new MyInputStream());
            dotest(dis, 0, 11, -1, 0);
            dotest(dis, 0, 11, 5, 5);
            Console.WriteLine("\n***CAUTION**** - may go into an infinite loop");
            dotest(dis, 5, 11, 20, 6);
        }


        private static void dotest(DataInputStream dis, int pos, int total,
                               int toskip, int expected)
        {

            try
            {
                if (VERBOSE)
                {
                    Console.WriteLine("\n\nTotal bytes in the stream = " + total);
                    Console.WriteLine("Currently at position = " + pos);
                    Console.WriteLine("Bytes to skip = " + toskip);
                    Console.WriteLine("Expected result = " + expected);
                }
                int skipped = dis.SkipBytes(toskip);
                if (VERBOSE)
                {
                    Console.WriteLine("Actual skipped = " + skipped);
                }
                if (skipped != expected)
                {
                    fail("DataInputStream.skipBytes does not return expected value");
                }
            }
            catch (EndOfStreamException e)
            {
                fail("DataInputStream.skipBytes throws unexpected EOFException");
            }
            catch (IOException e)
            {
                Console.WriteLine("IOException is thrown - possible result");
            }
        }

        internal class MyInputStream : MemoryStream
        {

            private int readctr = 0;


            public override int ReadByte()
            {

                if (readctr > 10)
                {
                    return -1;
                }
                else
                {
                    readctr++;
                    return 0;
                }

            }

        }
    }
}

