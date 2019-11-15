using System;
using System.Text;

namespace J2N.Text
{
    // from Apache Harmony

    /// <summary>
    /// Declares methods to append characters or character sequences.
    /// </summary>
    public interface IAppendable
    {
        /// <summary>
        /// Appends the string representation of a specified <see cref="char"/> object to this instance.
        /// </summary>
        /// <param name="value">The UTF-16-encoded code unit to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.</exception>
        IAppendable Append(char value);

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="value">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.</exception>
        IAppendable Append(string value);

        /// <summary>
        /// Appends a copy of a specified substring to this instance.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the third parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="count"/>.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> that contains the substring to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and 
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        IAppendable Append(string value, int startIndex, int count);

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> that contains the string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.</exception>
        IAppendable Append(StringBuilder value);

        /// <summary>
        /// Appends a copy of a specified substring to this instance.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the third parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="count"/>.
        /// </summary>
        /// <param name="value">The <see cref="StringBuilder"/> that contains the substring to append.</param>
        /// <param name="startIndex">The starting position of the substring within <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        IAppendable Append(StringBuilder value, int startIndex, int count);

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified array to this instance.
        /// </summary>
        /// <param name="value">The array of characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.</exception>
        IAppendable Append(char[] value);

        /// <summary>
        /// Appends the string representation of a specified subarray of Unicode characters to this instance.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the third parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="count"/>.
        /// </summary>
        /// <param name="value">A character array.</param>
        /// <param name="startIndex">The starting position in <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and 
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        IAppendable Append(char[] value, int startIndex, int count);

        /// <summary>
        /// Appends the string representation of the Unicode characters in a specified <see cref="ICharSequence"/> to this instance.
        /// </summary>
        /// <param name="value">The <see cref="ICharSequence"/> containing the characters to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.</exception>
        IAppendable Append(ICharSequence value);

        /// <summary>
        /// Appends the string representation of a specified <see cref="ICharSequence"/> of Unicode characters to this instance.
        /// <para/>
        /// IMPORTANT: This method has .NET semantics, that is, the third parameter is a length,
        /// not an exclusive end index as it would be in Java. To translate from Java to .NET,
        /// callers must account for this by subtracting (end - start) for the <paramref name="count"/>.
        /// </summary>
        /// <param name="value">The <see cref="ICharSequence"/> containing the characters to append.</param>
        /// <param name="startIndex">The starting position in <paramref name="value"/>.</param>
        /// <param name="count">The number of characters in <paramref name="value"/> to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>, and 
        /// <paramref name="startIndex"/> and <paramref name="count"/> are not zero.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="count"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> less than zero.
        /// <para/>
        /// -or-
        /// <para/>
        /// <paramref name="startIndex"/> + <paramref name="count"/> is greater than the length of <paramref name="value"/>.
        /// <para/>
        /// -or-
        /// <para/>
        /// Enlarging the value of this instance would exceed <see cref="StringBuilder.MaxCapacity"/>.
        /// </exception>
        IAppendable Append(ICharSequence value, int startIndex, int count);
    }
}
