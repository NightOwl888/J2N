using J2N.CodeGeneration;

namespace J2N.Text
{
    public partial class MutableTextBuffer
    {
        /// <summary>
        /// Deletes a sequence of characters specified by <paramref name="startIndex"/> and <paramref name="count"/>.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Delete{TBuilder}(TBuilder, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void DeleteInternal(int startIndex, int count) // Coverage for the JDK
        {
            if ((uint)startIndex > (uint)m_Position)
                ThrowHelper.ThrowArgumentOutOfRange_ArgumentOutOfRange_IndexString(startIndex, ExceptionArgument.startIndex);
            if (count < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(count, ExceptionArgument.count);

            int pos = m_Position;
            if ((uint)startIndex + (uint)count > pos)
                count = pos - startIndex;
            if (count > 0)
                RemoveCore(startIndex, count);
        }

        /// <summary>
        /// Removes the character at the specified index from this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.RemoveAt{TBuilder}(TBuilder, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void RemoveAtInternal(int index) // Coverage for the JDK (deleteCharAt)
        {
            if (index < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(index, ExceptionArgument.index);

            int currentLength = Length;
            if ((uint)index >= (uint)currentLength)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessException(index, ExceptionArgument.index);
            }

            RemoveCore(index, 1);
        }

        /// <summary>
        /// Removes the specified range of characters from this instance.
        /// </summary>
        /// <remarks>
        /// The public API and full documentation live in
        /// <see cref="MutableTextBufferExtensions.Remove{TBuilder}(TBuilder, int, int)"/>.
        /// Update that documentation if the behavior changes.
        /// </remarks>
        [CodeGenerationExtensionImplementation]
        internal void RemoveInternal(int startIndex, int length)
        {
            if (length < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(length, ExceptionArgument.length);
            if (startIndex < 0)
                ThrowHelper.ThrowArgumentOutOfRange_MustBeNonNegative(startIndex, ExceptionArgument.startIndex);

            if (length > m_Position - startIndex)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexMustBeLessOrEqualException(length, ExceptionArgument.length);
            }

            RemoveCore(startIndex, length);
        }
    }
}
