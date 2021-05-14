using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    /// <summary>
    /// J2N TODO: Docs
    /// </summary>
    public enum RoundingMode
    {
        /// <summary>
        /// J2N TODO: Docs
        /// </summary>
        Conservative,
        /// <summary>
        /// J2N TODO: Docs
        /// </summary>
        RoundEven
    }

    /// <summary>
    /// J2N TODO: Docs
    /// </summary>
    public static class RoundingModeExtensions
    {
        /// <summary>
        /// J2N TODO: Docs
        /// </summary>
        /// <param name="roundingMode"></param>
        /// <param name="even"></param>
        /// <returns></returns>
        public static bool AcceptUpperBound(this RoundingMode roundingMode, bool even)
        {
            switch (roundingMode)
            {
                case RoundingMode.Conservative:
                    return false;
                case RoundingMode.RoundEven:
                    return even;
                default:
                    return false;
            }
        }

        /// <summary>
        /// J2N TODO: Docs
        /// </summary>
        /// <param name="roundingMode"></param>
        /// <param name="even"></param>
        /// <returns></returns>
        public static bool AcceptLowerBound(this RoundingMode roundingMode, bool even)
        {
            switch (roundingMode)
            {
                case RoundingMode.Conservative:
                    return false;
                case RoundingMode.RoundEven:
                    return even;
                default:
                    return false;
            }
        }
    }
}
