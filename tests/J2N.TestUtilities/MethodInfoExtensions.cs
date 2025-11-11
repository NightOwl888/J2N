using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace J2N.TestUtilities
{
    public static class MethodInfoExtensions
    {
        /// <summary>Creates a delegate of the given type 'T' with the specified target from this method.</summary>
        public static T CreateDelegate<T>(this MethodInfo method, object? target) where T : Delegate => (T)method.CreateDelegate(typeof(T), target);
    }
}
