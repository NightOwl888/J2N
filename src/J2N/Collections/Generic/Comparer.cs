#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using System;
using System.Collections.Generic;


namespace J2N.Collections.Generic
{
    /// <summary>
    /// Provides comparers that use natural equality rules similar to those in Java.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    public static class Comparer<T>
    {
        /// <summary>
        /// Provides natural comparison rules similar to those in Java.
        /// <list type="bullet">
        ///     <item><description>
        ///         <see cref="double"/> and <see cref="float"/> are compared for positive zero and negative zero. These are considered
        ///         two separate numbers, as opposed to the default .NET behavior that considers them equal.
        ///     </description></item>
        ///     <item><description>
        ///         <see cref="double"/> and <see cref="float"/> are compared for NaN (not a number). All NaN values are considered equal,
        ///         which differs from the default .NET behavior, where NaN is never equal to NaN.
        ///     </description></item>
        ///     <item><description>
        ///         <see cref="double"/>? and <see cref="float"/>? are first compared for <c>null</c> prior to applying the above rules.
        ///     </description></item>
        ///     <item><description>
        ///         <see cref="string"/> uses culture-insensitive comparison using <see cref="StringComparer.Ordinal"/>.
        ///     </description></item>
        /// </list>
        /// </summary>
        public static IComparer<T> Default { get; } = LoadDefault();

        private static IComparer<T> LoadDefault()
        {
            Type genericClosingType = typeof(T);

            if (genericClosingType.IsGenericType && genericClosingType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // Special cases to match Java equality behavior
                if (typeof(double?).Equals(genericClosingType))
                    return (IComparer<T>)NullableDoubleComparer.Default;
                else if (typeof(float?).Equals(genericClosingType))
                    return (IComparer<T>)NullableSingleComparer.Default;

                return System.Collections.Generic.Comparer<T>.Default;
            }

            // Special cases to match Java equality behavior
            if (typeof(double).Equals(genericClosingType))
                return (IComparer<T>)DoubleComparer.Default;
            else if (typeof(float).Equals(genericClosingType))
                return (IComparer<T>)SingleComparer.Default;
            else if (typeof(string).Equals(genericClosingType))
                return (IComparer<T>)StringComparer.Ordinal;

            return System.Collections.Generic.Comparer<T>.Default;
        }
    }
}
