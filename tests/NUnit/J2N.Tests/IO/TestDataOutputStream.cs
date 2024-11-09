using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace J2N.IO
{
    public class TestDataOutputStream : TestCase
    {
        private static readonly Encoding defaultJavaStringEncoding = Encoding.GetEncoding("ISO-8859-1");

        private DataOutputStream os;

        private DataInputStream dis;

        private MemoryStream bos;

        String unihw = "\u0048\u0065\u006C\u006C\u006F\u0020\u0057\u006F\u0072\u006C\u0064";

        public String fileString = "Test_All_Tests\nTest_java_io_BufferedInputStream\nTest_java_io_BufferedOutputStream\nTest_java_io_ByteArrayInputStream\nTest_java_io_ByteArrayOutputStream\nTest_java_io_DataInputStream\n";

        /**
         * @tests java.io.DataOutputStream#DataOutputStream(java.io.OutputStream)
         */
        [Test]
        public void Test_ConstructorLjava_io_OutputStream()
        {
            assertTrue("Used in all tests", true);
        }

        /**
         * @tests java.io.DataOutputStream#flush()
         */
        [Test]
        public void Test_flush()
        {
            os.WriteInt32(9087589);
            os.Flush();
            openDataInputStream();
            int c = dis.ReadInt32();
            dis.Dispose();
            assertEquals("Failed to flush correctly", 9087589, c);
        }

        /**
         * @tests java.io.DataOutputStream#size()
         */
        [Test]
        public void Test_size()
        {
            os.Write(defaultJavaStringEncoding.GetBytes(fileString), 0, 150);
            os.Dispose();
            openDataInputStream();
            byte[]
            rbuf = new byte[150];
            dis.Read(rbuf, 0, 150);
            dis.Dispose();
            assertEquals("Incorrect size returned", 150, os.Length);
        }

        /**
         * @tests java.io.DataOutputStream#write(byte[], int, int)
         */
        [Test]
        public void Test_write_BII()
        {
            os.Write(defaultJavaStringEncoding.GetBytes(fileString), 0, 150);
            os.Dispose();
            openDataInputStream();
            byte[]
        rbuf = new byte[150];
            dis.Read(rbuf, 0, 150);
            dis.Dispose();
            assertTrue("Incorrect bytes written", defaultJavaStringEncoding.GetString(rbuf, 0, 150)
                    .Equals(fileString.Substring(0, 150 - 0))); // J2N: end - start
        }

        /**
         * @tests java.io.DataOutputStream#write(int)
         */
        [Test]
        public void Test_writeI()
        {
            os.Write((int)'t');
            os.Dispose();
            openDataInputStream();
            int c = dis.Read();
            dis.Dispose();
            assertTrue("Incorrect int written", (int)'t' == c);
        }

        /**
         * @tests java.io.DataOutputStream#writeBoolean(boolean)
         */
        [Test]
        public void Test_writeBooleanZ()
        {
            os.WriteBoolean(true);
            os.Dispose();
            openDataInputStream();
            bool c = dis.ReadBoolean();
            dis.Dispose();
            assertTrue("Incorrect boolean written", c);
        }

        /**
         * @tests java.io.DataOutputStream#writeByte(int)
         */
        [Test]
        public void Test_writeByteI()
        {
            os.WriteByte((sbyte)127);
            os.Dispose();
            openDataInputStream();
            sbyte c = (sbyte)dis.ReadByte();
            dis.Dispose();
            assertTrue("Incorrect byte written", c == (sbyte)127);
        }

        /**
         * @tests java.io.DataOutputStream#writeBytes(java.lang.String)
         */
        [Test]
        public void Test_writeBytesLjava_lang_String()
        {
            os.Write(defaultJavaStringEncoding.GetBytes(fileString));
            os.Dispose();
            openDataInputStream();
            byte[] rbuf = new byte[4000];
            dis.Read(rbuf, 0, fileString.Length);
            dis.Dispose();
            assertTrue("Incorrect bytes written", defaultJavaStringEncoding.GetString(rbuf, 0, fileString
                    .Length).Equals(fileString));

            // J2N: Not applicable, as we followed the .NET convention of putting a guard clause on the constructor
            // so a null stream is disallowed.

            // regression test for HARMONY-1101
            //new DataOutputStream(null).WriteBytes(""); // J2N: In .NET we have a guard clause 
        }

        /**
         * @tests java.io.DataOutputStream#writeChar(int)
         */
        [Test]
        public void Test_writeCharI()
        {
            os.WriteChar('T');
            os.Dispose();
            openDataInputStream();
            char c = dis.ReadChar();
            dis.Dispose();
            assertEquals("Incorrect char written", 'T', c);
        }

        /**
         * @tests java.io.DataOutputStream#writeChars(java.lang.String)
         */
        [Test]
        public void Test_writeCharsLjava_lang_String()
        {
            os.WriteChars("Test String");
            os.Dispose();
            openDataInputStream();
            char[] chars = new char[50];
            int i, a = dis.Available / 2;
            for (i = 0; i < a; i++)
                chars[i] = dis.ReadChar();
            assertEquals("Incorrect chars written", "Test String", new String(
                    chars, 0, i));
        }

        /**
         * @tests java.io.DataOutputStream#writeDouble(double)
         */
        [Test]
        public void Test_writeDoubleD()
        {
            os.WriteDouble(908755555456.98);
            os.Dispose();
            openDataInputStream();
            double c = dis.ReadDouble();
            dis.Dispose();
            assertEquals("Incorrect double written", 908755555456.98, c);
        }

        /**
         * @tests java.io.DataOutputStream#writeFloat(float)
         */
        [Test]
        public void Test_writeFloatF()
        {
            os.WriteSingle(9087.456f);
            os.Dispose();
            openDataInputStream();
            float c = dis.ReadSingle();
            dis.Dispose();
            assertTrue("Incorrect float written", c == 9087.456f);
        }

        /**
         * @tests java.io.DataOutputStream#writeInt(int)
         */
        [Test]
        public void Test_writeIntI()
        {
            os.WriteInt32(9087589);
            os.Dispose();
            openDataInputStream();
            int c = dis.ReadInt32();
            dis.Dispose();
            assertEquals("Incorrect int written", 9087589, c);
        }

        /**
         * @tests java.io.DataOutputStream#writeLong(long)
         */
        [Test]
        public void Test_writeLongJ()
        {
            os.WriteInt64(908755555456L);
            os.Dispose();
            openDataInputStream();
            long c = dis.ReadInt64();
            dis.Dispose();
            assertEquals("Incorrect long written", 908755555456L, c);
        }

        /**
         * @tests java.io.DataOutputStream#writeShort(int)
         */
        [Test]
        public void Test_writeShortI()
        {
            os.WriteInt16((short)9087);
            os.Dispose();
            openDataInputStream();
            short c = dis.ReadInt16();
            dis.Dispose();
            assertEquals("Incorrect short written", 9087, c);
        }

        /**
         * @tests java.io.DataOutputStream#writeUTF(java.lang.String)
         */
        [Test]
        public void Test_writeUTFLjava_lang_String()
        {
            os.WriteUTF(unihw);
            os.Dispose();
            openDataInputStream();
            //assertTrue("Failed to write string in UTF format", // J2N: In .NET Available is not supported
            //            dis.available() == unihw.Length + 2);
            assertTrue("Incorrect string returned", dis.ReadUTF().Equals(unihw));
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
                if (os != null)
                    os.Dispose();
                if (dis != null)
                    dis.Dispose();
            }
            catch (IOException e)
            {
            }
        }


        [Test]
        public void TestCounterOverflow()
        {
            var output = new MemoryStream();
            CounterOverflow dataOut = new CounterOverflow(output);

            dataOut.WriteByte(1);
            if (dataOut.Length < 0)
            {
                fail("Internal counter less than 0.");
            }
        }

        private class CounterOverflow : DataOutputStream
        {
            public CounterOverflow(Stream output)
                : base(output)
            {
                base.written = int.MaxValue;
            }
        }

        [Test]
        public void TestWriteUTF()
        {
            //ByteArrayOutputStream baos = new ByteArrayOutputStream();
            var baos = new MemoryStream();
            DataOutputStream dos = new DataOutputStream(baos);
            dos.WriteUTF("Hello, World!");  // 15
            dos.Flush();
            if (baos.Length != dos.Length)
                fail("Miscounted bytes in DataOutputStream.");
        }

        [Test]
        public void TestBoundsCheck()
        {
            byte[] data = { 90, 91, 92, 93, 94, 95, 96, 97, 98, 99 };
            //ByteArrayOutputStream bos = new ByteArrayOutputStream();
            var bos = new MemoryStream();
            DummyFilterStream dfs = new DummyFilterStream(bos);
            bool caughtException = false;

            // -ve length
            try
            {
                dfs.Write(data, 0, -5);
            }
            catch (ArgumentOutOfRangeException ie)
            {
                caughtException = true;
            }
            finally
            {
                if (!caughtException)
                    fail("Test failed");
            }

            // -ve offset
            caughtException = false;
            try
            {
                dfs.Write(data, -2, 5);
            }
            catch (ArgumentOutOfRangeException ie)
            {
                caughtException = true;
            }
            finally
            {
                if (!caughtException)
                    fail("Test failed");
            }

            // off + len > data.length
            caughtException = false;
            try
            {
                dfs.Write(data, 6, 5);
            }
            catch (ArgumentException ie)
            {
                caughtException = true;
            }
            finally
            {
                if (!caughtException)
                    fail("Test failed");
            }

            // null data
            caughtException = false;
            try
            {
                dfs.Write(null, 0, 5);
            }
            catch (ArgumentNullException re)
            {
                caughtException = true;
            }
            finally
            {
                if (!caughtException)
                    fail("Test failed");
            }
        }

        private class DummyFilterStream : DataOutputStream
        {

            public DummyFilterStream(Stream o)
                    : base(o)
            {
            }

            public override void Write(int val)
            {
                base.Write(val + 1);
            }
        }

        [Test]
        public void TestWrite()
        {
            IDataOutput f = new F(new Sink());
            f.Write(new byte[] { 1, 2, 3 }, 0, 3);
        }

        private class F : DataOutputStream
        {

            public F(Stream o)
                    : base(o)
            {
            }

            public override void Write(int b)
            {
                Debug.WriteLine("Ignoring write of " + b);
            }

        }

        private class Sink : MemoryStream
        {

            public override void WriteByte(byte b)
            {
                throw new Exception("DataOutputStream directly invoked"
                                           + " Write(byte) method of underlying"
                                           + " stream");
            }

        }
    }
}
