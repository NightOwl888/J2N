namespace J2N
{
    /// <summary>
    /// Extensions to the <see cref="System.Double"/> class.
    /// </summary>
    public static class DoubleExtensions
    {
        private const double NegativeZero = -0.0d;

        /// <summary>
        /// Gets a value indicating whether the current <see cref="double"/> has the value negative zero (<c>-0.0d</c>).
        /// While negative zero is supported by the <see cref="double"/> datatype in .NET, comparisons and string formatting ignore
        /// this feature. This method allows a simple way to check whether the current <see cref="double"/> has the value negative zero.
        /// </summary>
        /// <param name="d">This <see cref="double"/>.</param>
        /// <returns><c>true</c> if the current value represents negative zero; otherwise, <c>false</c>.</returns>
        public static bool IsNegativeZero(this double d)
        {
            return (d == 0 && BitConversion.DoubleToRawInt64Bits(d) == BitConversion.DoubleToRawInt64Bits(NegativeZero));
        }
    }
}
