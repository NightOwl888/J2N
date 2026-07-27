using System;

namespace J2N.Text.Tests
{
    internal static class MutableTextBufferHelpers
    {
        public static char[] ToCharArray(this MutableTextBuffer buffer)
        {
            return buffer.AsSpan().ToArray();
        }
    }
}
