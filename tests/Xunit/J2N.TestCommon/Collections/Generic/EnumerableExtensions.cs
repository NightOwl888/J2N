using J2N.Collections.Generic;
using System;
using System.Text;
using SCG = System.Collections.Generic;

namespace J2N.Collections.Tests
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// For testing purposes, we have a "closer" ToList() implementation that returns J2N's list instead of the default.
        /// <para/>
        /// TODO: Make a production extension method (apprpriately named) that can be used instead of ToList()
        /// </summary>
        public static List<T> ToList<T>(this SCG.IEnumerable<T> source)
        {
            return new List<T>(source);
        }
    }
}
