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

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace J2N.Collections.Generic
{
    internal static class StringComparerMetadataSerializer
    {
        private const string ComparerDescriptorTypeName = "ComparerDescriptor.Type";
        private const string ComparerDescriptorCultureName = "ComparerDescriptor.Culture";
        private const string ComparerDescriptorOptionsName = "ComparerDescriptor.Options";

        // Tries to read a StringComparer from a deserialization stream and returns false if it cannot be deserialized
        internal static bool TryGetKnownStringComparer(SerializationInfo info, [MaybeNullWhen(false)] out IEqualityComparer<string?> comparer)
        {
            Debug.Assert(info != null);

            if (!TryReadDescriptor(info!, out StringComparerDescriptor descriptor)) // [!] asserted above
            {
                comparer = null;
                return false;
            }

            return NonRandomizedStringEqualityComparer.TryGetStringComparer(in descriptor, out comparer);
        }

        // Tries to read a StringComparer from a deserialization stream and returns false if it cannot be deserialized
        internal static bool TryGetKnownStringComparer(SerializationInfo info, [MaybeNullWhen(false)] out IComparer<string?> comparer)
        {
            Debug.Assert(info != null);

            if (!TryReadDescriptor(info!, out StringComparerDescriptor descriptor)) // [!] asserted above
            {
                comparer = null;
                return false;
            }

            return WrappedStringComparer.TryGetStringComparer(in descriptor, out comparer);
        }

        private static bool TryReadDescriptor(SerializationInfo info, out StringComparerDescriptor descriptor)
        {
            Debug.Assert(info != null);

            // Descriptor fields may not exist (old blobs)
            SerializationInfoEnumerator e = info!.GetEnumerator(); // [!] asserted above

            StringComparerDescriptor.Classification type = default;
            StringComparerDescriptor.Fields found = StringComparerDescriptor.Fields.None;
            CompareOptions options = default;
            string? cultureName = null;

            while (e.MoveNext())
            {
                switch (e.Name)
                {
                    case ComparerDescriptorTypeName:
                        type = (StringComparerDescriptor.Classification)(int)e.Value!;
                        found |= StringComparerDescriptor.Fields.Type;
                        break;

                    case ComparerDescriptorCultureName:
                        cultureName = (string?)e.Value;
                        found |= StringComparerDescriptor.Fields.CultureName;
                        break;

                    case ComparerDescriptorOptionsName:
                        options = (CompareOptions)(int)e.Value!;
                        found |= StringComparerDescriptor.Fields.Options;
                        break;
                }

                // Exit early once we have everything we need
                if (found == StringComparerDescriptor.Fields.All)
                    break;
            }

            if ((found & StringComparerDescriptor.Fields.Type) == 0)
            {
                // Old blob – descriptor not present
                descriptor = default;
                return false;
            }

            descriptor = new StringComparerDescriptor(type, cultureName, options);
            return true;
        }

        // Writes a StringComparerDescriptor to a serialization stream
        internal static void AddValue(this SerializationInfo info, StringComparerDescriptor descriptor)
        {
            Debug.Assert(info != null);

            info!.AddValue(ComparerDescriptorTypeName, (int)descriptor.Type);
            info.AddValue(ComparerDescriptorCultureName, descriptor.CultureName, typeof(string));
            info.AddValue(ComparerDescriptorOptionsName, (int)descriptor.Options);
        }
    }
}
