#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using System;


namespace J2N.IO
{
    /// <summary>
    /// Defines an interface for classes that are able to read typed data from some
    /// source. Typically, this data has been written by a class which implements
    /// <see cref="IDataOutput"/>. Types that can be read include byte, 16-bit short, 32-bit
    /// int, 32-bit float, 64-bit long, 64-bit double, byte strings, and MUTF-8
    /// strings.
    /// <para/>
    /// <h3>MUTF-8 (Modified UTF-8) Encoding</h3>
    /// <para/>
    /// When encoding strings as UTF, implementations of <see cref="IDataInput"/> and
    /// <see cref="IDataOutput"/> use a slightly modified form of UTF-8, hereafter referred
    /// to as MUTF-8. This form is identical to standard UTF-8, except:
    /// <list type="bullet">
    ///     <item><description>Only the one-, two-, and three-byte encodings are used.</description></item>
    ///     <item><description>Code points in the range <code>U+10000</code> &#8230;
    ///         <code>U+10ffff</code> are encoded as a surrogate pair, each of which is
    ///         represented as a three-byte encoded value.</description></item>
    ///     <item><description>The code point <code>U+0000</code> is encoded in two-byte form.</description></item>
    /// </list>
    /// <para/>
    /// Please refer to <a href="http://unicode.org">The Unicode Standard</a> for
    /// further information about character encoding. MUTF-8 is actually closer to
    /// the (relatively less well-known) encoding <a
    /// href="http://www.unicode.org/reports/tr26/">CESU-8</a> than to UTF-8 per se.
    /// <para/>
    /// Equivalent to Java's DataInput interface
    /// </summary>
    /// <seealso cref="DataInputStream"/>
    public interface IDataInput
    {
        /// <summary>
        /// Reads a boolean.
        /// </summary>
        /// <returns>The next boolean value.</returns>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        /// <seealso cref="IDataOutput.WriteBoolean(bool)"/>
        bool ReadBoolean();

        /// <summary>
        /// Reads an 8-bit byte value and returns it as an <see cref="int"/>.
        /// <para/>
        /// NOTE: This was readByte() in Java
        /// </summary>
        /// <returns>The next 8-bit <see cref="byte"/> value.</returns>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        /// <seealso cref="IDataOutput.WriteByte(int)"/>
        int ReadSByte();

        /// <summary>
        /// Reads an unsigned 8-bit <see cref="byte"/> value and returns it as an <see cref="int"/>.
        /// <para/>
        /// NOTE: This was readUnsignedByte() in Java
        /// </summary>
        /// <returns>The next unsigned <see cref="byte"/> value.</returns>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        /// <seealso cref="IDataOutput.WriteByte(int)"/>
        int ReadByte();

        /// <summary>
        /// Reads a 16-bit character value.
        /// </summary>
        /// <returns>The next <see cref="char"/> value.</returns>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        /// <seealso cref="IDataOutput.WriteChar(int)"/>
        char ReadChar();

        /// <summary>
        /// Reads a 64-bit <see cref="double"/> value.
        /// </summary>
        /// <returns>The next <see cref="double"/> value.</returns>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        /// <seealso cref="IDataOutput.WriteDouble(double)"/>
        double ReadDouble();

        /// <summary>
        /// Reads a 32-bit <see cref="float"/> value.
        /// <para/>
        /// NOTE: This was readFloat() in Java
        /// </summary>
        /// <returns>The next <see cref="float"/> value.</returns>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        /// <seealso cref="IDataOutput.WriteSingle(float)"/>
        float ReadSingle();

        /// <summary>
        /// Reads bytes into the byte array <paramref name="buffer"/>. This method will block
        /// until <c><paramref name="buffer"/>.Length</c> number of bytes have been read.
        /// </summary>
        /// <param name="buffer">The buffer to read bytes into.</param>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="buffer"/> is <c>null</c>.</exception>
        /// <seealso cref="IDataOutput.Write(byte[])"/>
        /// <seealso cref="IDataOutput.Write(byte[], int, int)"/>
        void ReadFully(byte[] buffer);

        /// <summary>
        /// Reads bytes and stores them in the byte array <paramref name="buffer"/> starting at
        /// offset <paramref name="offset"/>. This method blocks until <paramref name="count"/> number of
        /// bytes have been read.
        /// </summary>
        /// <param name="buffer">The byte array in which to store the bytes read.</param>
        /// <param name="offset">The initial position in <paramref name="buffer"/> to store the bytes
        /// read.</param>
        /// <param name="count">The maximum number of bytes to store in <paramref name="buffer"/>.</param>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="buffer"/> is <c>null</c>.</exception>
        /// <seealso cref="IDataOutput.Write(byte[])"/>
        /// <seealso cref="IDataOutput.Write(byte[], int, int)"/>
        void ReadFully(byte[] buffer, int offset, int count);

        /// <summary>
        /// NOTE: This was readShort() in Java
        /// </summary>
        short ReadInt16();

        /// <summary>
        /// Reads a 16-bit <see cref="ushort"/> value.
        /// <para/>
        /// NOTE: This was readUnsignedShort() in Java
        /// </summary>
        /// <returns>The next <see cref="ushort"/> value.</returns>
        int ReadUInt16();

        /// <summary>
        /// Reads a 32-bit integer value.
        /// <para/>
        /// NOTE: This was readInt() in Java
        /// </summary>
        /// <returns>The next <see cref="int"/> value.</returns>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        /// <seealso cref="IDataOutput.WriteInt32(int)"/>
        int ReadInt32();


        /// <summary>
        /// Reads a 64-bit <see cref="long"/> value.
        /// <para/>
        /// NOTE: This was readLong() in Java
        /// </summary>
        /// <returns>The next <see cref="long"/> value.</returns>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        /// <seealso cref="IDataOutput.WriteInt64(long)"/>
        long ReadInt64();

        /// <summary>
        /// Returns a string containing the next line of text available from this
        /// stream. A line is made of zero or more characters followed by
        /// <c>'\n'</c>, <c>'\r'</c>, <c>"\r\n"</c> or the end of the stream. The string
        /// does not include the newline sequence.
        /// </summary>
        /// <returns>The contents of the line or <c>null</c> if no characters have been read
        /// before the end of the stream.</returns>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        string? ReadLine();

        /// <summary>
        /// Reads a string encoded with <see cref="IDataInput"/> modified UTF-8.
        /// </summary>
        /// <returns>The next string encoded with <see cref="IDataInput"/> modified UTF-8.</returns>
        /// <exception cref="System.IO.EndOfStreamException">If the end of the input is reached before the read
        /// request can be satisfied.</exception>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        /// <seealso cref="IDataOutput.WriteUTF(string)"/>
        string ReadUTF();

        /// <summary>
        /// Skips <paramref name="count"/> number of bytes. This method will not throw an
        /// <see cref="System.IO.EndOfStreamException"/> if the end of the input is reached before
        /// <paramref name="count"/> bytes were skipped.
        /// </summary>
        /// <param name="count">The number of bytes to skip.</param>
        /// <returns>The number of bytes actually skipped.</returns>
        /// <exception cref="System.IO.IOException">If an I/O error occurs while reading.</exception>
        int SkipBytes(int count);
    }
}
