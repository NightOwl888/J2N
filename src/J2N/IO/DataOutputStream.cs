using System;
using System.IO;
#nullable enable

namespace J2N.IO
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// Wraps an existing <see cref="Stream"/> and writes typed data to it.
    /// Typically, this stream can be read in by <see cref="DataInputStream"/>. Types that can be
    /// written include byte, 16-bit short, 32-bit int, 32-bit float, 64-bit long,
    /// 64-bit double, byte strings, and DataInput MUTF-8 encoded strings.
    /// <para/>
    /// Java's <see cref="DataOutputStream"/> is similar to .NET's <see cref="BinaryWriter"/>. However, it writes
    /// in a modified UTF-8 format that is written in big-endian byte order.
    /// This is a port of <see cref="DataOutputStream"/> that is fully compatible with Java's <see cref="DataInputStream"/>.
    /// <para>
    /// Usage Note: Always favor <see cref="BinaryWriter"/> over <see cref="DataOutputStream"/> unless you specifically need
    /// the modified UTF-8 format and/or the <see cref="WriteUTF(string)"/> method.
    /// </para>
    /// </summary>
    /// <seealso cref="DataInputStream"/>
    public class DataOutputStream : IDataOutput, IDisposable
    {
        private readonly object _lock = new object();

        /// <summary>
        /// The number of bytes written out so far.
        /// </summary>
        protected int written;
        private readonly byte[] buff;
        private readonly bool leaveOpen;


        private readonly Stream output;

        /// <summary>
        /// Constructs a new <see cref="DataOutputStream"/> on the <see cref="Stream"/>
        /// <paramref name="output"/>. Note that data written by this stream is not in a human
        /// readable form but can be reconstructed by using a <see cref="DataInputStream"/>
        /// on the resulting output.
        /// </summary>
        /// <param name="output">The target stream for writing.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="output"/> is <c>null</c>.</exception>
        /// <seealso cref="DataInputStream"/>
        public DataOutputStream(Stream output)
            : this(output, false)
        { }

        /// <summary>
        /// Constructs a new <see cref="DataOutputStream"/> on the <see cref="Stream"/>
        /// <paramref name="output"/>. Note that data written by this stream is not in a human
        /// readable form but can be reconstructed by using a <see cref="DataInputStream"/>
        /// on the resulting output.
        /// </summary>
        /// <param name="output">The target stream for writing.</param>
        /// <param name="leaveOpen"><c>true</c> to leave the stream open after the <see cref="DataOutputStream"/> object is disposed; otherwise, <c>false</c>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="output"/> is <c>null</c>.</exception>
        /// <seealso cref="DataInputStream"/>
        public DataOutputStream(Stream output, bool leaveOpen)
        {
            this.output = output ?? throw new ArgumentNullException(nameof(output));
            this.leaveOpen = leaveOpen;
            buff = new byte[8];
        }

        /// <summary>
        /// Flushes this stream to ensure all pending data is sent out to the target
        /// stream. This implementation then also flushes the target stream.
        /// </summary>
        /// <exception cref="IOException">If an error occurs attempting to flush this stream.</exception>
        public virtual void Flush() => output.Flush();

        /// <summary>
        /// Gets the total number of bytes written to the target stream so far.
        /// </summary>
        public int Length
        {
            get
            {
                if (written < 0)
                {
                    written = int.MaxValue;
                }
                return written;
            }
        }

        /// <summary>
        /// Writes <paramref name="count"/> bytes from the byte array <paramref name="buffer"/> starting at
        /// <paramref name="offset"/> to the target stream.
        /// </summary>
        /// <param name="buffer">The buffer to write to the target stream.</param>
        /// <param name="offset">The index of the first byte in <paramref name="buffer"/> to write.</param>
        /// <param name="count">The number of bytes from the <paramref name="buffer"/> to write.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="buffer"/> is <c>null</c>.</exception>
        /// <seealso cref="DataInputStream.ReadFully(byte[])"/>
        /// <seealso cref="DataInputStream.ReadFully(byte[], int, int)"/>
        public virtual void Write(byte[] buffer, int offset, int count)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            lock (_lock)
            {
                output.Write(buffer, offset, count);
                written += count;
            }
        }

        /// <summary>
        /// Writes a <see cref="byte"/> to the target stream. Only the least significant byte of
        /// the integer <paramref name="oneByte"/> is written.
        /// </summary>
        /// <param name="oneByte">The <see cref="byte"/> to write to the target stream.</param>
        public virtual void Write(int oneByte)
        {
            lock (_lock)
            {
                output.WriteByte((byte)oneByte);
                written++;
            }
        }

        /// <summary>
        /// Writes a <see cref="bool"/> to the target stream.
        /// </summary>
        /// <param name="value">The <see cref="bool"/> value to write to the target stream.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <seealso cref="DataInputStream.ReadBoolean()"/>
        public void WriteBoolean(bool value)
        {
            lock (_lock)
            {
                output.WriteByte((byte)(value ? 1 : 0));
                written++;
            }
        }

        /// <summary>
        /// Writes an 8-bit <see cref="byte"/> to the target stream. Only the least significant
        /// byte of the integer <paramref name="value"/> is written.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> value to write to the target stream.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <seealso cref="DataInputStream.ReadSByte()"/>
        /// <seealso cref="DataInputStream.ReadByte()"/>
        public void WriteByte(int value)
        {
            lock (_lock)
            {
                output.WriteByte((byte)value);
                written++;
            }
        }

        /// <summary>
        /// Writes the low order bytes from a string to the target stream.
        /// </summary>
        /// <param name="value">The string containing the bytes to write to the target stream.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <seealso cref="DataInputStream.ReadFully(byte[])"/>
        /// <seealso cref="DataInputStream.ReadFully(byte[], int, int)"/>
        public void WriteBytes(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            lock (_lock)
            {
                if (value.Length == 0)
                {
                    return;
                }
                byte[] bytes = new byte[value.Length];
                for (int index = 0; index < value.Length; index++)
                {
                    bytes[index] = (byte)value[index];
                }
                output.Write(bytes, 0, bytes.Length);
                written += bytes.Length;
            }
        }

        /// <summary>
        /// Writes a 16-bit character to the target stream. Only the two lower bytes
        /// of the integer <paramref name="value"/> are written, with the higher one written
        /// are written, with the higher one written <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The character to write to the target stream.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <seealso cref="DataInputStream.ReadChar()"/>
        public void WriteChar(int value)
        {
            lock (_lock)
            {
                buff[0] = (byte)(value >> 8);
                buff[1] = (byte)value;
                output.Write(buff, 0, 2);
                written += 2;
            }
        }

        /// <summary>
        /// Writes the 16-bit characters contained in <paramref name="value"/> to the target
        /// stream.
        /// </summary>
        /// <param name="value">The string that contains the characters to write to this
        /// stream.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <seealso cref="DataInputStream.ReadChar()"/>
        public void WriteChars(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            lock (_lock)
            {
                byte[] newBytes = new byte[value.Length * 2];
                for (int index = 0; index < value.Length; index++)
                {
                    int newIndex = index == 0 ? index : index * 2;
                    newBytes[newIndex] = (byte)(value[index] >> 8);
                    newBytes[newIndex + 1] = (byte)value[index];
                }
                output.Write(newBytes, 0, newBytes.Length);
                written += newBytes.Length;
            }
        }

        /// <summary>
        /// Writes a 64-bit <see cref="double"/> to the target stream. The resulting output is the
        /// eight bytes resulting from calling <see cref="BitConversion.DoubleToInt64Bits(double)"/>.
        /// </summary>
        /// <param name="value">The <see cref="double"/> to write to the target stream.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <seealso cref="DataInputStream.ReadDouble()"/>
        public void WriteDouble(double value)
        {
            WriteInt64(BitConversion.DoubleToInt64Bits(value));
        }

        /// <summary>
        /// Writes a 32-bit <see cref="float"/> to the target stream. The resulting output is the
        /// four bytes resulting from calling <see cref="BitConversion.SingleToInt32Bits(float)"/>.
        /// <para/>
        /// NOTE: This was writeFloat() in Java
        /// </summary>
        /// <param name="value">The <see cref="float"/> to write to the target stream.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <seealso cref="DataInputStream.ReadSingle()"/>
        public void WriteSingle(float value)
        {
            WriteInt32(BitConversion.SingleToInt32Bits(value));
        }

        /// <summary>
        /// Writes a 32-bit <see cref="int"/> to the target stream. The resulting output is the
        /// four bytes, highest order first, of <paramref name="value"/>.
        /// <para/>
        /// NOTE: This was writeInt() in Java
        /// </summary>
        /// <param name="value">The <see cref="int"/> to write to the target stream.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <seealso cref="DataInputStream.ReadInt32()"/>
        public void WriteInt32(int value)
        {
            lock (_lock)
            {
                buff[0] = (byte)(value >> 24);
                buff[1] = (byte)(value >> 16);
                buff[2] = (byte)(value >> 8);
                buff[3] = (byte)value;
                output.Write(buff, 0, 4);
                written += 4;
            }
        }

        /// <summary>
        /// Writes a 64-bit <see cref="long"/> to the target stream. The resulting output is the
        /// eight bytes, highest order first, of <paramref name="value"/>.
        /// <para/>
        /// NOTE: This was writeLong() in Java
        /// </summary>
        /// <param name="value">The <see cref="long"/> to write to the target stream.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <seealso cref="DataInputStream.ReadInt64()"/>
        public void WriteInt64(long value)
        {
            lock (_lock)
            {
                buff[0] = (byte)(value >> 56);
                buff[1] = (byte)(value >> 48);
                buff[2] = (byte)(value >> 40);
                buff[3] = (byte)(value >> 32);
                buff[4] = (byte)(value >> 24);
                buff[5] = (byte)(value >> 16);
                buff[6] = (byte)(value >> 8);
                buff[7] = (byte)value;
                output.Write(buff, 0, 8);
                written += 8;
            }
        }

        private int WriteInt64ToBuffer(long value,
                          byte[] buffer, int offset)
        {
            buffer[offset++] = (byte)(value >> 56);
            buffer[offset++] = (byte)(value >> 48);
            buffer[offset++] = (byte)(value >> 40);
            buffer[offset++] = (byte)(value >> 32);
            buffer[offset++] = (byte)(value >> 24);
            buffer[offset++] = (byte)(value >> 16);
            buffer[offset++] = (byte)(value >> 8);
            buffer[offset++] = (byte)value;
            return offset;
        }

        /// <summary>
        /// Writes the specified 16-bit <see cref="short"/> to the target stream. Only the lower
        /// two bytes of the integer <paramref name="value"/> are written, with the higher one
        /// written first.
        /// <para/>
        /// NOTE: This was writeShort() in Java
        /// </summary>
        /// <param name="value">The <see cref="short"/> to write to the target stream.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <seealso cref="DataInputStream.ReadInt16()"/>
        public void WriteInt16(int value)
        {
            lock (_lock)
            {
                buff[0] = (byte)(value >> 8);
                buff[1] = (byte)value;
                output.Write(buff, 0, 2);
                written += 2;
            }
        }

        private int WriteInt16ToBuffer(int value,
                           byte[] buffer, int offset)
        {
            buffer[offset++] = (byte)(value >> 8);
            buffer[offset++] = (byte)value;
            return offset;
        }

        /// <summary>
        /// Writes the specified encoded in DataInput modified UTF-8 to this
        /// stream.
        /// </summary>
        /// <param name="value">The string to write to the target stream encoded in
        /// DataInput modified UTF-8.</param>
        /// <exception cref="IOException">If an error occurs while writing to the target stream.</exception>
        /// <exception cref="FormatException">If the encoded string is longer than 65535 bytes.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <c>null</c>.</exception>
        /// <seealso cref="DataInputStream.ReadUTF()"/>
        public void WriteUTF(string value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            long utfCount = CountUTFBytes(value);
            if (utfCount > 65535)
            {
                throw new FormatException(SR.Format_InvalidUTFTooLong); //$NON-NLS-1$
            }
            byte[] buffer = new byte[(int)utfCount + 2];
            int offset = 0;
            offset = WriteInt16ToBuffer((int)utfCount, buffer, offset);
            offset = WriteUTFBytesToBuffer(value, (int)utfCount, buffer, offset);
            Write(buffer, 0, offset);
        }

        private long CountUTFBytes(string value)
        {
            int utfCount = 0, length = value.Length;
            for (int i = 0; i < length; i++)
            {
                int charValue = value[i];
                if (charValue > 0 && charValue <= 127)
                {
                    utfCount++;
                }
                else if (charValue <= 2047)
                {
                    utfCount += 2;
                }
                else
                {
                    utfCount += 3;
                }
            }
            return utfCount;
        }

        private int WriteUTFBytesToBuffer(string value, long count,
                              byte[] buffer, int offset)
        {
            int length = value.Length;
            for (int i = 0; i < length; i++)
            {
                int charValue = value[i];
                if (charValue > 0 && charValue <= 127)
                {
                    buffer[offset++] = (byte)charValue;
                }
                else if (charValue <= 2047)
                {
                    buffer[offset++] = (byte)(0xc0 | (0x1f & (charValue >> 6)));
                    buffer[offset++] = (byte)(0x80 | (0x3f & charValue));
                }
                else
                {
                    buffer[offset++] = (byte)(0xe0 | (0x0f & (charValue >> 12)));
                    buffer[offset++] = (byte)(0x80 | (0x3f & (charValue >> 6)));
                    buffer[offset++] = (byte)(0x80 | (0x3f & charValue));
                }
            }
            return offset;
        }

        #region From FilterOutputStream

        /// <summary>
        /// Writes the entire contents of the byte array <paramref name="buffer"/> to this
        /// stream. This implementation writes the <paramref name="buffer"/> to the target
        /// stream.
        /// </summary>
        /// <param name="buffer">The buffer to be written.</param>
        /// <exception cref="IOException">If an I/O error occurs while writing to this stream.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="buffer"/> is <c>null</c>.</exception>
        public void Write(byte[] buffer)
        {
            if (buffer is null)
                throw new ArgumentNullException(nameof(buffer));

            Write(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!leaveOpen)
                    output.Dispose();
            }
        }

        #endregion
    }
}
