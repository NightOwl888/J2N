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
        private static readonly bool IsLinux = LoadIsLinux();
        private static readonly bool IsWindows = LoadIsWindows();
        private static readonly bool IsFileStreamLockingPlatform = IsWindows || IsLinux;

        private static bool LoadIsLinux()
        {
#if NETSTANDARD
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
#if NETSTANDARD
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
            PlatformID pid = Environment.OSVersion.Platform;
            return pid == PlatformID.Win32NT || pid == PlatformID.Win32Windows;
#endif
        }

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
#if !NETSTANDARD1_4
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

#if !NETSTANDARD1_4
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
