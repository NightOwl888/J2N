using System;

namespace J2N.Text.Tests
{
    internal static class MutableTextBufferExtensions
    {
        public static char[] ToCharArray(this MutableTextBuffer buffer)
        {
            return buffer.AsSpan().ToArray();
        }
    }
}
