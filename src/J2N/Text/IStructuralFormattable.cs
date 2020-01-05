using System;
using System.Collections.Generic;
using System.Text;

namespace J2N.Text
{
    /// <summary>
    /// Provides functionality to format the value of an collection into a string representation.
    /// </summary>
    internal interface IStructuralFormattable : IFormattable
    {
        /// <summary>
        /// Formats the value of the current instance using the specified format.
        /// </summary>
        /// <param name="format">
        /// The format to use. -or- A null reference (Nothing in Visual Basic) to use the
        /// default format defined for the type of the <see cref="System.IFormattable"/> implementation.</param>
        /// <param name="formatProvider">
        /// The provider to use to format the value. -or- A null reference (Nothing in Visual
        /// Basic) to obtain the numeric format information from the current locale setting
        /// of the operating system.</param>
        /// <returns>The value of the current instance in the specified format.</returns>
        new string ToString(string format, IFormatProvider formatProvider);
    }
}
