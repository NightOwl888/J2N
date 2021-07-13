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

using J2N.IO;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace J2N
{
    /// <summary>
    /// Extensions to <see cref="System.IO.Stream"/>.
    /// </summary>
    public static class StreamExtensions
    {
#if FEATURE_FILESTREAM_LOCK
        private static readonly bool IsLinux = LoadIsLinux();
        private static readonly bool IsWindows = LoadIsWindows();
        private static readonly bool IsFileStreamLockingPlatform = IsWindows || IsLinux;

        private static bool LoadIsLinux()
        {
#if FEATURE_RUNTIMEINFORMATION
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#else
            // we use integers instead of enum tags because "MacOS"
            // requires 2.0 SP2, 3.0 SP2 or 3.5 SP1.
            // 128 is mono's old platform tag for Unix.
            // Reference: https://stackoverflow.com/a/5117005
            int id = (int)Environment.OSVersion.Platform;
            return id == 4 || id == 128;
#endif
        }

        private static bool LoadIsWindows()
        {
#if FEATURE_RUNTIMEINFORMATION
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
            PlatformID pid = Environment.OSVersion.Platform;
            return pid == PlatformID.Win32NT || pid == PlatformID.Win32Windows;
#endif
        }
#endif // FEATURE_FILESTREAM_LOCK

        /// <summary>
        /// Writes bytes from the given byte buffer to this <see cref="Stream"/>.
        /// <para/>
        /// The bytes are written starting at the current position, and after
        /// some number of bytes are written (up to the remaining number of bytes in
        /// the buffer) the file position is increased by the number of bytes
        /// actually written.
        /// </summary>
        /// <param name="stream">This stream.</param>
        /// <param name="source">The byte buffer containing the bytes to be written.</param>
        /// <returns>The number of bytes actually written.</returns>
        public static int Write(this Stream stream, ByteBuffer source) // TODO: Move to J2N.IO and port tests
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (source == null)
                return 0;

            const int bufferSize = 8192;
            int written = 0;
#if FEATURE_FILESTREAM_LOCK
            long lockPosition = 0;
            long lockLength = 0;
            FileStream fileStream = null;
            if (IsFileStreamLockingPlatform && stream is FileStream)
            {
                lockPosition = stream.Position;
                lockLength = source.Remaining;
                fileStream = stream as FileStream;
                fileStream.Lock(lockPosition, lockLength);
            }
            try
            {
#endif
            byte[] buffer = new byte[Math.Min(bufferSize, source.Remaining)];
                while (source.Remaining > 0)
                {
                    if (buffer.Length > source.Remaining)
                        buffer = new byte[source.Remaining];
                    source.Get(buffer);
                    stream.Write(buffer, 0, buffer.Length);
                    written += buffer.Length;
                }

#if FEATURE_FILESTREAM_LOCK
            }
            finally
            {
                fileStream?.Unlock(lockPosition, lockLength);
            }
#endif
            return written;
        }
    }
}
