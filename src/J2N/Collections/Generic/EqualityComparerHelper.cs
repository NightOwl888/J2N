using SCG = System.Collections.Generic;

namespace J2N.Collections.Generic
{
    /// <summary>
    /// Helper methods for working with EqualityComparers.
    /// </summary>
    internal static class EqualityComparerHelper
    {
        /// <summary>
        /// Determines if the provided <paramref name="set1EqualityComparer"/> equals
        /// the equality comparer of the provided <paramref name="set2"/>.
        /// </summary>
        /// <param name="set1EqualityComparer">The equality comparer of the first set.</param>
        /// <param name="set2">The second set.</param>
        /// <typeparam name="T">The type of elements in the set</typeparam>
        /// <returns>Returns <c>true</c> if the equality comparers are equal.</returns>
        public static bool AreSetEqualityComparersEqual<T>(SCG.IEqualityComparer<T> set1EqualityComparer, SCG.IEnumerable<T> set2)
            => set2 switch
            {
                HashSet<T> hashSet => set1EqualityComparer.Equals(hashSet.EqualityComparer),
                OrderedHashSet<T> orderedHashSet => set1EqualityComparer.Equals(orderedHashSet.EqualityComparer),
                LinkedHashSet<T> linkedHashSet => set1EqualityComparer.Equals(linkedHashSet.EqualityComparer),
                SCG.HashSet<T> scgHashSet => set1EqualityComparer.Equals(scgHashSet.Comparer),
                Net5.HashSet<T> net5HashSet => set1EqualityComparer.Equals(net5HashSet.EqualityComparer),
                _ => false
            };
    }
}
