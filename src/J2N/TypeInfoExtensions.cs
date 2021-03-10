#if FEATURE_TYPEINFO

using System;
using System.Linq;
using System.Reflection;
#nullable enable

namespace J2N
{
    /// <summary>
    /// Extensions to the <see cref="System.Reflection.TypeInfo"/> class.
    /// </summary>
    public static class TypeInfoExtensions
    {
        /// <summary>
        /// Returns true if this type implements the specified generic interface.
        /// <para/>
        /// Examples:
        /// <code>
        /// bool result1 = this.GetType().GetTypeInfo().ImplementsGenericInterface(typeof(IDictionary&lt;,&gt;);
        /// bool result2 = typeof(System.Collections.Generic.List&lt;int&gt;).GetTypeInfo().ImplementsGenericInterface(typeof(IList&lt;&gt;);
        /// </code>
        /// </summary>
        /// <param name="target">This <see cref="TypeInfo"/>.</param>
        /// <param name="interfaceType">The type of generic inteface to check.</param>
        /// <returns><c>true</c> if the type implements the generic interface; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="target"/> or <paramref name="interfaceType"/> is <c>null</c>.</exception>
        public static bool ImplementsGenericInterface(this TypeInfo target, Type interfaceType)
        {
            if (target is null)
                throw new ArgumentNullException(nameof(target));
            if (interfaceType is null)
                throw new ArgumentNullException(nameof(interfaceType));

            return target.IsGenericType && target.GetGenericTypeDefinition().GetInterfaces().Any(
                x => x.GetTypeInfo().IsGenericType && interfaceType.IsAssignableFrom(x.GetGenericTypeDefinition())
            );
        }
    }
}
#endif
