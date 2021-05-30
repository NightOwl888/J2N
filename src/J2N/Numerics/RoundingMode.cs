// Copyright 2018 Ulf Adams
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace J2N.Numerics
{
    /// <summary>
    /// J2N TODO: Docs
    /// </summary>
    internal enum RoundingMode
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
    internal static class RoundingModeExtensions
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
