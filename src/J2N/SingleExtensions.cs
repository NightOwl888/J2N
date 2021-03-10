using System.Runtime.CompilerServices;
#nullable enable

namespace J2N
{
    /// <summary>
    /// Extensions to the <see cref="System.Single"/> class.
    /// </summary>
    public static class SingleExtensions
    {
        private const float NegativeZero = -0.0f;

        /// <summary>
        /// Gets a value indicating whether the current <see cref="float"/> has the value negative zero (<c>-0.0f</c>).
        /// While negative zero is supported by the <see cref="float"/> datatype in .NET, comparisons and string formatting ignore
        /// this feature. This method allows a simple way to check whether the current <see cref="float"/> has the value negative zero.
        /// </summary>
        /// <param name="f">This <see cref="float"/>.</param>
        /// <returns><c>true</c> if the current value represents negative zero; otherwise, <c>false</c>.</returns>
#if FEATURE_METHODIMPLOPTIONS_AGRESSIVEINLINING
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif 
        public static bool IsNegativeZero(this float f)
        {
            return (f == 0 && BitConversion.SingleToRawInt32Bits(f) == BitConversion.SingleToRawInt32Bits(NegativeZero));
        }
    }
}
