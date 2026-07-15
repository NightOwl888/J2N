using System;

namespace J2N.Text.Tests
{
    internal static class MutableTextBufferExtensions
    {
        /// <summary>
        /// Helper to mimic the fluent API style of the MutableTextBuffer methods that return the buffer itself, but allows for any method to be used in the action.
        /// </summary>
        public static MutableTextBuffer Apply(this MutableTextBuffer buffer, Action<MutableTextBuffer> action)
        {
            action(buffer);
            return buffer;
        }

        public static char[] ToCharArray(this MutableTextBuffer buffer)
        {
            return buffer.AsSpan().ToArray();
        }
    }
}
