using System;
using System.Collections.Generic;

namespace J2N.Text
{
    /// <summary>
    /// An interface for the bidirectional iteration over a group of characters. The
    /// iteration starts at the begin index in the group of characters and continues
    /// for the length of the character sequence.
    /// </summary>
    internal interface ICharacterEnumerator : IEnumerator<char> // J2N TODO: API Make this interface public when the issues with converting iterator/enumerator are fixed
#if FEATURE_CLONEABLE
        ICloneable
#endif
    {
#if FEATURE_CLONEABLE
        /// <summary>
        /// Returns a new <see cref="ICharacterEnumerator"/> with the same properties.
        /// </summary>
        /// <returns>A shallow copy of this character enumerator.</returns>
        /// <seealso cref="ICloneable"/>
        new object Clone();
#else
        /// <summary>
        /// Returns a new <see cref="ICharacterEnumerator"/> with the same properties.
        /// </summary>
        /// <returns>A shallow copy of this character enumerator.</returns>
        object Clone();
#endif

        /// <summary>
        /// Decrements the current index.
        /// </summary>
        /// <returns><c>true</c> if the move operation was successful; otherwise, <c>false</c>.</returns>
        bool MovePrevious();

        /// <summary>
        /// Sets the current position to the begin index.
        /// </summary>
        /// <returns><c>true</c> if the move operation was successful. <c>false</c> if no move
        /// operation took place because of a 0 length.</returns>
        bool MoveFirst();

        /// <summary>
        /// Sets the current position to the begin index + length.
        /// </summary>
        /// <returns><c>true</c> if the move operation was successful. <c>false</c> if no move
        /// operation took place because of a 0 length.</returns>
        bool MoveLast();

        /// <summary>
        /// Gets the begin index. Returns the index of the first character of the iteration.
        /// </summary>
        int StartIndex { get; }

        /// <summary>
        /// Gets the end index. Returns the last character of the iteration.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics. That is, the end index is inclusive,
        /// not exclusive as it would be in Java. To translate from Java, always use <c>EndIndex + 1</c>.
        /// </summary>
        int EndIndex { get; }

        /// <summary>
        /// Gets the number of characters in the iteration.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets or sets the current index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">The value set indicates a position less than
        /// <see cref="StartIndex"/> or greater than <see cref="EndIndex"/>.</exception>
        int Index { get; set; }

        /// <summary>
        /// Attempts to set the <see cref="Index"/> to <paramref name="value"/>.
        /// <para/>
        /// Returns <c>true</c> if the <paramref name="value"/> passed is between
        /// <see cref="StartIndex"/> and <see cref="EndIndex"/> inclusive; otherwise, returns <c>false</c>.
        /// <para/>
        /// If <paramref name="value"/> is less than <see cref="StartIndex"/>, the
        /// <see cref="Index"/> will be set to <see cref="StartIndex"/>.
        /// <para/>
        /// If <paramref name="value"/> is greater than <see cref="EndIndex"/>, the
        /// <see cref="Index"/> will be set to <see cref="EndIndex"/>.
        /// </summary>
        /// <param name="value">The new index.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> passed is between
        /// <see cref="StartIndex"/> and <see cref="EndIndex"/> inclusive; otherwise, <c>false</c>.</returns>
        bool TrySetIndex(int value);
    }
}
