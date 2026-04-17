#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace J2N.Collections.Generic
{
    internal readonly ref struct StringComparerDescriptor
    {
        public enum Classification
        {
            Ordinal,
            OrdinalIgnoreCase,
            InvariantCulture,
            InvariantCultureIgnoreCase,
            CurrentCulture,
            CurrentCultureIgnoreCase,
        }

        [Flags]
        public enum Fields : byte
        {
            None = 0,
            Type = 1 << 0,
            CultureName = 1 << 1,
            Options = 1 << 2,

            All = Type | CultureName | Options
        }


        public Classification Type { get; }
        public string? CultureName { get; }
        public CompareOptions Options { get; }

        public StringComparerDescriptor(
            Classification type,
            string? cultureName,
            CompareOptions options)
        {
            Type = type;
            CultureName = cultureName;

            // J2N: Only IgnoreCase is supported by J2N, so we ignore any other options that are inadvertently specfied.
            // No options are ever valid for Ordinal or OrdinalIgnoreCase.

            Options = type switch
            {
                Classification.Ordinal or
                Classification.OrdinalIgnoreCase => CompareOptions.None,

                _ => options & CompareOptions.IgnoreCase
            };
        }

        public static bool TryDescribe(object? comparer, out StringComparerDescriptor descriptor)
        {
            if (ReferenceEquals(comparer, StringComparer.Ordinal))
            {
                descriptor = new(Classification.Ordinal, cultureName: null, CompareOptions.None);
                return true;
            }

            if (ReferenceEquals(comparer, StringComparer.OrdinalIgnoreCase))
            {
                descriptor = new(Classification.OrdinalIgnoreCase, cultureName: null, CompareOptions.None);
                return true;
            }

            // NOTE: It is possible for users to create custom StringComparer instances that are invariant, so we need the extra check.
            if (ReferenceEquals(comparer, StringComparer.InvariantCulture) || StringComparer.InvariantCulture.Equals(comparer))
            {
                descriptor = new(Classification.InvariantCulture, cultureName: CultureInfo.InvariantCulture.Name, CompareOptions.None);
                return true;
            }

            // NOTE: It is possible for users to create custom StringComparer instances that are invariant, so we need the extra check.
            if (ReferenceEquals(comparer, StringComparer.InvariantCultureIgnoreCase) || StringComparer.InvariantCultureIgnoreCase.Equals(comparer))
            {
                descriptor = new(Classification.InvariantCultureIgnoreCase, cultureName: CultureInfo.InvariantCulture.Name, CompareOptions.IgnoreCase);
                return true;
            }

            if (comparer is not StringComparer sc)
            {
                descriptor = default;
                return false;
            }

#if FEATURE_STRINGCOMPARER_ISWELLKNOWNCULTUREAWARECOMPARER
            if (StringComparer.IsWellKnownCultureAwareComparer(sc, out CompareInfo? compareInfo, out CompareOptions options))
            {
                bool ignoreCase = (options & CompareOptions.IgnoreCase) != 0;

                descriptor = new(
                    ignoreCase
                        ? Classification.CurrentCultureIgnoreCase
                        : Classification.CurrentCulture,
                    cultureName: compareInfo.Name,
                    options);
                return true;
            }
#else
            // Best-effort fallback
            if (StringComparer.CurrentCulture.Equals(sc))
            {
                descriptor = new(
                    Classification.CurrentCulture,
                    CultureInfo.CurrentCulture.Name,
                    CompareOptions.None);
                return true;
            }

            if (StringComparer.CurrentCultureIgnoreCase.Equals(sc))
            {
                descriptor = new(
                    Classification.CurrentCultureIgnoreCase,
                    CultureInfo.CurrentCulture.Name,
                    CompareOptions.IgnoreCase);
                return true;
            }
#endif

            descriptor = default;
            return false;
        }

        public readonly bool TryCreateStringComparer(object rawComparer, out StringComparer? comparer) =>
            TryCreateStringComparer(rawComparer, in this, out comparer);

        public static bool TryCreateStringComparer(object rawComparer, in StringComparerDescriptor descriptor, [MaybeNullWhen(false)] out StringComparer comparer)
        {
            comparer = null;

            switch (descriptor.Type)
            {
                case Classification.Ordinal:
                    comparer = StringComparer.Ordinal;
                    return true;

                case Classification.OrdinalIgnoreCase:
                    comparer = StringComparer.OrdinalIgnoreCase;
                    return true;

                case Classification.InvariantCulture:
                    comparer = StringComparer.InvariantCulture;
                    return true;

                case Classification.InvariantCultureIgnoreCase:
                    comparer = StringComparer.InvariantCultureIgnoreCase;
                    return true;

                case Classification.CurrentCulture:
                case Classification.CurrentCultureIgnoreCase:
                    {
                        bool ignoreCase = (descriptor.Options & CompareOptions.IgnoreCase) != 0;
                        StringComparer currentCulture = ignoreCase ? StringComparer.CurrentCultureIgnoreCase : StringComparer.CurrentCulture;

                        // Use the well-known instance if it matches.
                        // rawComparer is the deserialized instance to compare against. If the culture matches and casing matches,
                        // we can use the well-known instance.
                        if (currentCulture.Equals(rawComparer))
                        {
                            comparer = currentCulture;
                            return true;
                        }

                        // Otherwise, try to create a StringComparer from the descriptor
                        try
                        {
                            CultureInfo culture = CultureInfo.GetCultureInfo(descriptor.CultureName!);
                            comparer = StringComparer.Create(culture, ignoreCase);
                            return true;
                        }
                        catch (CultureNotFoundException)
                        {
                            // Culture doesn't exist on the current system (may happen after deserialization). Default to current culture.
                            comparer = currentCulture;
                            return true;
                        }
                    }

                default:
                    return false;
            }
        }
    }
}
