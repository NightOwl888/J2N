// Adapted from: https://github.com/dotnet/runtime/blob/v9.0.4/src/libraries/System.Private.CoreLib/src/System/Text/Encoding.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Buffers;
using J2N.Runtime.InteropServices;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// Extensions to <see cref="Encoding"/>.
    /// </summary>
    public static class EncodingExtensions
    {
        private const int CharStackBufferSize = 64;

        /// <summary>
        /// Calculates the number of bytes produced by encoding the characters in the specified
        /// character span.
        /// </summary>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="chars">The span of characters to encode.</param>
        /// <returns>The number of bytes produced by encoding the specified character span.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
        /// <exception cref="EncoderFallbackException">
        /// A fallback occurred (for more information, see
        /// <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding">Character Encoding in .NET</a>)
        /// <para/>
        /// -and-
        /// <para/>
        /// <see cref="Encoding.EncoderFallback"/> is set to <see cref="System.Text.EncoderExceptionFallback"/>.
        /// </exception>
        public static int GetByteCount(this Encoding encoding, ReadOnlySpan<char> chars)
        {
            if (encoding is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.encoding);

            return GetByteCountInternal(encoding, chars);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int GetByteCountInternal(Encoding encoding, ReadOnlySpan<char> chars)
        {
            Debug.Assert(encoding is not null);

            fixed (char* charsPtr = &MemoryMarshalHelper.GetNonNullPinnableReference(chars))
            {
                return encoding!.GetByteCount(charsPtr, chars.Length);
            }
        }

        /// <summary>
        /// Encodes the specified character into a sequence of bytes.
        /// </summary>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="ch">The character to encode.</param>
        /// <param name="buffer">The memory location to store the chars. Typically,
        /// it should be <c>stackalloc byte[4]</c> since it will never be longer than 4 bytes.
        /// Note that this <paramref name="buffer"/> is not intended for use by callers,
        /// it is just a memory location to use when returning the result. The caller is
        /// responsible for ensuring the memory location has a sufficient scope that is
        /// at least as long as the scope of the return value.</param>
        /// <returns>A <see cref="ReadOnlySpan{Byte}"/> containing the results of encoding
        /// the specified character.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="buffer"/> has a
        /// <see cref="Span{Byte}.Length"/> less than 4.</exception>
        /// <exception cref="EncoderFallbackException">
        /// A fallback occurred (for more information, see
        /// <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding">Character Encoding in .NET</a>)
        /// <para/>
        /// -and-
        /// <para/>
        /// <see cref="Encoding.EncoderFallback"/> is set to <see cref="System.Text.EncoderExceptionFallback"/>.
        /// </exception>
        public static unsafe ReadOnlySpan<byte> GetBytes(this Encoding encoding, char ch, Span<byte> buffer)
        {
            if (encoding is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.encoding);
            if (buffer.Length < 4)
                ThrowHelper.ThrowArgumentException_DestinationTooShort();

            fixed (byte* bytesPtr = &MemoryMarshalHelper.GetNonNullPinnableReference(buffer))
            {
                char* charPtr = &ch;
                int bytesWritten = encoding.GetBytes(charPtr, 1, bytesPtr, buffer.Length);
                return buffer.Slice(0, bytesWritten);
            }
        }

        /// <summary>
        /// Encodes the specified code point into a sequence of bytes.
        /// </summary>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="codePoint">The code point to encode.</param>
        /// <param name="buffer">The memory location to store the chars. Typically,
        /// it should be <c>stackalloc byte[8]</c> since it will never be longer than 8 bytes.
        /// Note that this <paramref name="buffer"/> is not intended for use by callers,
        /// it is just a memory location to use when returning the result. The caller is
        /// responsible for ensuring the memory location has a sufficient scope that is
        /// at least as long as the scope of the return value.</param>
        /// <returns>A <see cref="ReadOnlySpan{Byte}"/> containing the results of encoding
        /// the specified character.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="buffer"/> has a <see cref="Span{Byte}.Length"/> less than 8.
        /// <para/>
        /// -or-
        /// <para/>
        /// If <paramref name="codePoint"/> is not a valid Unicode code point.
        /// </exception>
        /// <exception cref="EncoderFallbackException">
        /// A fallback occurred (for more information, see
        /// <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding">Character Encoding in .NET</a>)
        /// <para/>
        /// -and-
        /// <para/>
        /// <see cref="Encoding.EncoderFallback"/> is set to <see cref="System.Text.EncoderExceptionFallback"/>.
        /// </exception>
        public static unsafe ReadOnlySpan<byte> GetBytes(this Encoding encoding, int codePoint, Span<byte> buffer)
        {
            if (encoding is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.encoding);
            if (buffer.Length < 8)
                ThrowHelper.ThrowArgumentException_DestinationTooShort();

            ReadOnlySpan<char> chars = Character.ToChars(codePoint, stackalloc char[2]);

            fixed (byte* bytesPtr = &MemoryMarshalHelper.GetNonNullPinnableReference(buffer))
            fixed (char* charsPtr = &MemoryMarshalHelper.GetNonNullPinnableReference(chars))
            {
                int bytesWritten = encoding.GetBytes(charsPtr, chars.Length, bytesPtr, buffer.Length);
                return buffer.Slice(0, bytesWritten);
            }
        }

        /// <summary>
        /// Encodes into a span of bytes a set of characters from the specified read-only span.
        /// </summary>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="chars">The span containing the set of characters to encode.</param>
        /// <param name="bytes">The byte span to hold the encoded bytes.</param>
        /// <returns>The number of encoded bytes.</returns>
        /// <remarks>This is a polyfill for .NET target frameworks that do not have this overload.
        /// It will be superseded where the overload exists on <see cref="Encoding"/>.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
        /// <exception cref="EncoderFallbackException">
        /// A fallback occurred (for more information, see
        /// <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding">Character Encoding in .NET</a>)
        /// <para/>
        /// -and-
        /// <para/>
        /// <see cref="Encoding.EncoderFallback"/> is set to <see cref="System.Text.EncoderExceptionFallback"/>.
        /// </exception>
        public static int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            if (encoding is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.encoding);

            return GetBytesInternal(encoding, chars, bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int GetBytesInternal(Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            Debug.Assert(encoding is not null);

            fixed (char* charsPtr = &MemoryMarshalHelper.GetNonNullPinnableReference(chars))
            fixed (byte* bytesPtr = &MemoryMarshalHelper.GetNonNullPinnableReference(bytes))
            {
                return encoding!.GetBytes(charsPtr, chars.Length, bytesPtr, bytes.Length);
            }
        }

        /// <summary>
        /// Encodes into a span of bytes a set of characters from the specified read-only span if the destination is large enough.
        /// </summary>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="chars">The span containing the set of characters to encode.</param>
        /// <param name="bytes">The byte span to hold the encoded bytes.</param>
        /// <param name="bytesWritten">Upon successful completion of the operation, the number of bytes encoded into <paramref name="bytes"/>.</param>
        /// <returns><see langword="true"/> if all of the characters were encoded into the destination; <see langword="false"/> if the destination was too small to contain all the encoded bytes.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
        /// <exception cref="EncoderFallbackException">
        /// A fallback occurred (for more information, see
        /// <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding">Character Encoding in .NET</a>)
        /// <para/>
        /// -and-
        /// <para/>
        /// <see cref="Encoding.EncoderFallback"/> is set to <see cref="System.Text.EncoderExceptionFallback"/>.
        /// </exception>
        public static bool TryGetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes, out int bytesWritten)
        {
            if (encoding is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.encoding);

            int required = GetByteCountInternal(encoding, chars);
            if (required <= bytes.Length)
            {
                bytesWritten = GetBytesInternal(encoding, chars, bytes);
                return true;
            }

            bytesWritten = 0;
            return false;
        }

        /// <summary>
        /// Calculates the number of characters produced by decoding the provided read-only byte span.
        /// </summary>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bytes">A read-only byte span to decode.</param>
        /// <returns>The number of characters produced by decoding the byte span.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
        /// <exception cref="DecoderFallbackException">
        /// A fallback occurred (for more information, see
        /// <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding">Character Encoding in .NET</a>)
        /// <para/>
        /// -and-
        /// <para/>
        /// <see cref="Encoding.DecoderFallback"/> is set to <see cref="System.Text.DecoderExceptionFallback"/>.
        /// </exception>
        public static int GetCharCount(this Encoding encoding, ReadOnlySpan<byte> bytes)
        {
            if (encoding is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.encoding);

            return GetCharCountInternal(encoding, bytes);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int GetCharCountInternal(this Encoding encoding, ReadOnlySpan<byte> bytes)
        {
            Debug.Assert(encoding is not null);

            fixed (byte* bytesPtr = &MemoryMarshalHelper.GetNonNullPinnableReference(bytes))
            {
                return encoding!.GetCharCount(bytesPtr, bytes.Length);
            }
        }

        /// <summary>
        /// Decodes all the bytes in the specified read-only byte span into a character span.
        /// </summary>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bytes">A read-only span containing the sequence of bytes to decode.</param>
        /// <param name="chars">The character span receiving the decoded bytes.</param>
        /// <returns>The actual number of characters written at the span indicated by the <paramref name="chars"/> parameter</returns>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
        /// <exception cref="DecoderFallbackException">
        /// A fallback occurred (for more information, see
        /// <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding">Character Encoding in .NET</a>)
        /// <para/>
        /// -and-
        /// <para/>
        /// <see cref="Encoding.DecoderFallback"/> is set to <see cref="System.Text.DecoderExceptionFallback"/>.
        /// </exception>
        public static int GetChars(this Encoding encoding, ReadOnlySpan<byte> bytes, Span<char> chars)
        {
            if (encoding is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.encoding);

            return GetCharsInternal(encoding, bytes, chars);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe int GetCharsInternal(this Encoding encoding, ReadOnlySpan<byte> bytes, Span<char> chars)
        {
            Debug.Assert(encoding is not null);

            fixed (byte* bytesPtr = &MemoryMarshalHelper.GetNonNullPinnableReference(bytes))
            fixed (char* charsPtr = &MemoryMarshalHelper.GetNonNullPinnableReference(chars))
            {
                return encoding!.GetChars(bytesPtr, bytes.Length, charsPtr, chars.Length);
            }
        }

        /// <summary>
        /// Decodes into a span of chars a set of bytes from the specified read-only span if the destination is large enough.
        /// </summary>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bytes">A read-only span containing the sequence of bytes to decode.</param>
        /// <param name="chars">The character span receiving the decoded bytes.</param>
        /// <param name="charsWritten">Upon successful completion of the operation, the number of chars decoded into <paramref name="chars"/>.</param>
        /// <returns><see langword="true"/> if all of the characters were decoded into the destination; <see langword="false"/> if the destination was too small to contain all the decoded chars.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="encoding"/> is <c>null</c>.</exception>
        /// <exception cref="DecoderFallbackException">
        /// A fallback occurred (for more information, see
        /// <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding">Character Encoding in .NET</a>)
        /// <para/>
        /// -and-
        /// <para/>
        /// <see cref="Encoding.DecoderFallback"/> is set to <see cref="System.Text.DecoderExceptionFallback"/>.
        /// </exception>
        public static bool TryGetChars(this Encoding encoding, ReadOnlySpan<byte> bytes, Span<char> chars, out int charsWritten)
        {
            if (encoding is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.encoding);

            int required = GetCharCountInternal(encoding, bytes);
            if (required <= chars.Length)
            {
                charsWritten = GetCharsInternal(encoding, bytes, chars);
                return true;
            }

            charsWritten = 0;
            return false;
        }

        /// <summary>
        /// Decodes all the bytes in the specified byte span into a string.
        /// </summary>
        /// <param name="encoding">The character encoding to use.</param>
        /// <param name="bytes">A read-only byte span to decode to a Unicode string.</param>
        /// <returns>A string that contains the decoded bytes from the provided read-only span.</returns>
        /// <exception cref="DecoderFallbackException">
        /// A fallback occurred (for more information, see
        /// <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/character-encoding">Character Encoding in .NET</a>)
        /// <para/>
        /// -and-
        /// <para/>
        /// <see cref="Encoding.DecoderFallback"/> is set to <see cref="System.Text.DecoderExceptionFallback"/>.
        /// </exception>
        public static unsafe string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes)
        {
            if (encoding is null)
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.encoding);

            fixed (byte* bytesPtr = &MemoryMarshalHelper.GetNonNullPinnableReference(bytes))
            {
#if FEATURE_ENCODING_GETSTRING_BYTEPTR
                return encoding.GetString(bytesPtr, bytes.Length);
#else
                int byteLength = bytes.Length;

                // Get our string length
                int stringLength = encoding.GetCharCount(bytesPtr, byteLength);
                Debug.Assert(stringLength >= 0);

                char[]? buffer = null;
                Span<char> chars = stringLength > CharStackBufferSize
                    ? (buffer = ArrayPool<char>.Shared.Rent(stringLength)).AsSpan(0, stringLength)
                    : stackalloc char[stringLength];
                try
                {
                    fixed (char* pTempChars = &MemoryMarshal.GetReference(chars))
                    {
                        int doubleCheck = encoding.GetChars(bytesPtr, byteLength, pTempChars, stringLength);
                        Debug.Assert(stringLength == doubleCheck,
                            "Expected encoding.GetChars to return same length as encoding.GetCharCount");
                    }

                    return chars.ToString();
                }
                finally
                {
                    ArrayPool<char>.Shared.ReturnIfNotNull(buffer);
                }
#endif
            }
        }
    }
}
