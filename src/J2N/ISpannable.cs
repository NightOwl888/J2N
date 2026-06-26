using System;

namespace J2N
{
    /// <summary>
    /// Contract that specifies this type is capable of exposing its memory as a
    /// <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of span element.</typeparam>
    internal interface ISpannable<T>
    {
        ReadOnlySpan<T> AsSpan();
        ReadOnlySpan<T> AsSpan(int start);
        ReadOnlySpan<T> AsSpan(int start, int length);
    }
}
