using System;

namespace J2N
{
    /// <summary>
    /// Extensions to <see cref="System.Random"/>.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Generates a random <see cref="bool"/>, with a random distribution of
        /// approximately 50/50.
        /// </summary>
        /// <param name="random">This <see cref="Random"/>.</param>
        /// <returns>A random <see cref="bool"/>.</returns>
        public static bool NextBoolean(this Random random)
        {
            return random.Next(1, 100) >= 50;
        }
    }
}
