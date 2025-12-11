// Source: https://github.com/dotnet/runtime/blob/v10.0.0/src/libraries/System.Runtime.InteropServices/tests/System.Runtime.InteropServices.UnitTests/System/Runtime/InteropServices/CollectionsMarshalTests.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Collections.Generic;
using J2N.Runtime.CompilerServices;
using J2N.TestUtilities.Xunit;
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace J2N.Runtime.InteropServices.Tests
{
    public class CollectionMarshalTests
    {
        [Fact]
        public unsafe void NullListAsSpanValueType()
        {
            List<int> list = null;
            Span<int> span = CollectionMarshal.AsSpan(list);

            Assert.Equal(0, span.Length);

            fixed (int* pSpan = span)
            {
                Assert.True(pSpan == null);
            }
        }

        [Fact]
        public unsafe void NullListAsSpanClass()
        {
            List<object> list = null;
            Span<object> span = CollectionMarshal.AsSpan(list);

            Assert.Equal(0, span.Length);
        }

        [Fact]
        public void ListAsSpanValueType()
        {
            var list = new List<int>();
            foreach (int length in Enumerable.Range(0, 36))
            {
                list.Clear();
                ValidateContentEquality(list, CollectionMarshal.AsSpan(list));

                for (int i = 0; i < length; i++)
                {
                    list.Add(i);
                }
                ValidateContentEquality(list, CollectionMarshal.AsSpan(list));

                list.TrimExcess();
                ValidateContentEquality(list, CollectionMarshal.AsSpan(list));

                list.Add(length + 1);
                ValidateContentEquality(list, CollectionMarshal.AsSpan(list));
            }

            static void ValidateContentEquality(List<int> list, Span<int> span)
            {
                Assert.Equal(list.Count, span.Length);

                for (int i = 0; i < span.Length; i++)
                {
                    Assert.Equal(list[i], span[i]);
                }
            }
        }

        [Fact]
        public void ListAsSpanClass()
        {
            var list = new List<IntAsObject>();
            foreach (int length in Enumerable.Range(0, 36))
            {
                list.Clear();
                ValidateContentEquality(list, CollectionMarshal.AsSpan(list));

                for (var i = 0; i < length; i++)
                {
                    list.Add(new IntAsObject { Value = i });
                }
                ValidateContentEquality(list, CollectionMarshal.AsSpan(list));

                list.TrimExcess();
                ValidateContentEquality(list, CollectionMarshal.AsSpan(list));

                list.Add(new IntAsObject { Value = length + 1 });
                ValidateContentEquality(list, CollectionMarshal.AsSpan(list));
            }

            static void ValidateContentEquality(List<IntAsObject> list, Span<IntAsObject> span)
            {
                Assert.Equal(list.Count, span.Length);

                for (int i = 0; i < span.Length; i++)
                {
                    Assert.Equal(list[i].Value, span[i].Value);
                }
            }
        }

        [Fact]
        public void ListAsSpanLinkBreaksOnResize()
        {
            var list = new List<int>(capacity: 10);

            for (int i = 0; i < 10; i++)
            {
                list.Add(i);
            }
            list.TrimExcess();
            Span<int> span = CollectionMarshal.AsSpan(list);

            int startCapacity = list.Capacity;
            int startCount = list.Count;
            Assert.Equal(startCount, startCapacity);
            Assert.Equal(startCount, span.Length);

            for (int i = 0; i < span.Length; i++)
            {
                span[i]++;
                Assert.Equal(list[i], span[i]);

                list[i]++;
                Assert.Equal(list[i], span[i]);
            }

            // Resize to break link between Span and List
            list.Add(11);

            Assert.NotEqual(startCapacity, list.Capacity);
            Assert.NotEqual(startCount, list.Count);
            Assert.Equal(startCount, span.Length);

            for (int i = 0; i < span.Length; i++)
            {
                span[i] += 2;
                Assert.NotEqual(list[i], span[i]);

                list[i] += 3;
                Assert.NotEqual(list[i], span[i]);
            }
        }

        #region Dictionary

        [Fact]
        public void Dictionary_GetValueRefOrNullRefValueType()
        {
            var dict = new Dictionary<int, Struct>
            {
                {  1, default },
                {  2, default }
            };

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            Struct itemVal = dict[1];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does not change values in dictionary
            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            CollectionMarshal.GetValueRefOrNullRef(dict, 1).Value = 3;
            CollectionMarshal.GetValueRefOrNullRef(dict, 1).Property = 4;

            Assert.Equal(3, dict[1].Value);
            Assert.Equal(4, dict[1].Property);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrNullRef(dict, 2);

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            itemRef = new Struct() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            // Check for null refs

            Assert.True(Unsafe.IsNullRef(ref CollectionMarshal.GetValueRefOrNullRef(dict, 3)));
            Assert.Throws<NullReferenceException>(() => CollectionMarshal.GetValueRefOrNullRef(dict, 3).Value = 9);

            Assert.Equal(2, dict.Count);
        }

        [Fact]
        public void Dictionary_GetValueRefOrNullRefClass()
        {
            var dict = new Dictionary<int, IntAsObject>
            {
                {  1, new IntAsObject() },
                {  2, new IntAsObject() }
            };

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            IntAsObject itemVal = dict[1];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does change values in dictionary
            Assert.Equal(1, dict[1].Value);
            Assert.Equal(2, dict[1].Property);

            CollectionMarshal.GetValueRefOrNullRef(dict, 1).Value = 3;
            CollectionMarshal.GetValueRefOrNullRef(dict, 1).Property = 4;

            Assert.Equal(3, dict[1].Value);
            Assert.Equal(4, dict[1].Property);

            ref IntAsObject itemRef = ref CollectionMarshal.GetValueRefOrNullRef(dict, 2);

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            itemRef = new IntAsObject() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            // Check for null refs

            Assert.True(Unsafe.IsNullRef(ref CollectionMarshal.GetValueRefOrNullRef(dict, 3)));
            Assert.Throws<NullReferenceException>(() => CollectionMarshal.GetValueRefOrNullRef(dict, 3).Value = 9);

            Assert.Equal(2, dict.Count);
        }

        [Fact]
        public void Dictionary_GetValueRefOrNullRefLinkBreaksOnResize()
        {
            var dict = new Dictionary<int, Struct>
            {
                {  1, new Struct() }
            };

            Assert.Equal(1, dict.Count);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrNullRef(dict, 1);

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 1;
            itemRef.Property = 2;

            Assert.Equal(1, itemRef.Value);
            Assert.Equal(2, itemRef.Property);
            Assert.Equal(dict[1].Value, itemRef.Value);
            Assert.Equal(dict[1].Property, itemRef.Property);

            // Resize
            dict.EnsureCapacity(100);
            for (int i = 2; i <= 50; i++)
            {
                dict.Add(i, new Struct());
            }

            itemRef.Value = 3;
            itemRef.Property = 4;

            Assert.Equal(3, itemRef.Value);
            Assert.Equal(4, itemRef.Property);

            // Check connection broken
            Assert.NotEqual(dict[1].Value, itemRef.Value);
            Assert.NotEqual(dict[1].Property, itemRef.Property);

            Assert.Equal(50, dict.Count);
        }

        [Fact]
        public void Dictionary_GetValueRefOrAddDefaultValueType()
        {
            // This test is the same as the one for GetValueRefOrNullRef, but it uses
            // GetValueRefOrAddDefault instead, and also checks for incorrect additions.
            // The two APIs should behave the same when values already exist.
            var dict = new Dictionary<int, Struct>
            {
                {  1, default },
                {  2, default }
            };

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            Struct itemVal = dict[1];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does not change values in dictionary
            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            CollectionMarshal.GetValueRefOrAddDefault(dict, 1, out bool exists).Value = 3;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);

            CollectionMarshal.GetValueRefOrAddDefault(dict, 1, out exists).Property = 4;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(3, dict[1].Value);
            Assert.Equal(4, dict[1].Property);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(dict, 2, out exists);

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            itemRef = new Struct() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            // Check for correct additions

            ref Struct entry3Ref = ref CollectionMarshal.GetValueRefOrAddDefault(dict, 3, out exists);

            Assert.False(exists);
            Assert.Equal(3, dict.Count);
            Assert.False(Unsafe.IsNullRef(ref entry3Ref));
            Assert.True(EqualityComparer<Struct>.Default.Equals(entry3Ref, default));

            entry3Ref.Property = 42;
            entry3Ref.Value = 12345;

            Struct value3 = dict[3];

            Assert.Equal(42, value3.Property);
            Assert.Equal(12345, value3.Value);
        }

        [Fact]
        public void Dictionary_GetValueRefOrAddDefaultClass()
        {
            var dict = new Dictionary<int, IntAsObject>
            {
                {  1, new IntAsObject() },
                {  2, new IntAsObject() }
            };

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            IntAsObject itemVal = dict[1];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does change values in dictionary
            Assert.Equal(1, dict[1].Value);
            Assert.Equal(2, dict[1].Property);

            CollectionMarshal.GetValueRefOrAddDefault(dict, 1, out bool exists).Value = 3;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);

            CollectionMarshal.GetValueRefOrAddDefault(dict, 1, out exists).Property = 4;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(3, dict[1].Value);
            Assert.Equal(4, dict[1].Property);

            ref IntAsObject itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(dict, 2, out exists);

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            itemRef = new IntAsObject() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            // Check for correct additions

            ref IntAsObject entry3Ref = ref CollectionMarshal.GetValueRefOrAddDefault(dict, 3, out exists);

            Assert.False(exists);
            Assert.Equal(3, dict.Count);
            Assert.False(Unsafe.IsNullRef(ref entry3Ref));
            Assert.Null(entry3Ref);

            entry3Ref = new IntAsObject() { Value = 12345, Property = 42 };

            IntAsObject value3 = dict[3];

            Assert.Equal(42, value3.Property);
            Assert.Equal(12345, value3.Value);
        }

        [Fact]
        public void Dictionary_GetValueRefOrAddDefaultLinkBreaksOnResize()
        {
            var dict = new Dictionary<int, Struct>
            {
                {  1, new Struct() }
            };

            Assert.Equal(1, dict.Count);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(dict, 1, out bool exists);

            Assert.True(exists);
            Assert.Equal(1, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 1;
            itemRef.Property = 2;

            Assert.Equal(1, itemRef.Value);
            Assert.Equal(2, itemRef.Property);
            Assert.Equal(dict[1].Value, itemRef.Value);
            Assert.Equal(dict[1].Property, itemRef.Property);

            // Resize
            dict.EnsureCapacity(100);
            for (int i = 2; i <= 50; i++)
            {
                dict.Add(i, new Struct());
            }

            itemRef.Value = 3;
            itemRef.Property = 4;

            Assert.Equal(3, itemRef.Value);
            Assert.Equal(4, itemRef.Property);

            // Check connection broken
            Assert.NotEqual(dict[1].Value, itemRef.Value);
            Assert.NotEqual(dict[1].Property, itemRef.Property);

            Assert.Equal(50, dict.Count);
        }

        #endregion Dictionary

        #region Dictionary AlternateLookup

#if FEATURE_IALTERNATEEQUALITYCOMPARER

        [Fact] // J2N specific
        public void Dictionary_AlternateLookup_GetValueRefOrNullRefValueType()
        {
            var dict = new Dictionary<string, Struct>
            {
                {  "1", default },
                {  "2", default }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            Struct itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does not change values in dictionary
            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Value = 3;
            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Property = 4;

            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrNullRef(lookup, "2".AsSpan());

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new Struct() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for null refs

            Assert.True(Unsafe.IsNullRef(ref CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan())));
            Assert.Throws<NullReferenceException>(() => CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan()).Value = 9);

            Assert.Equal(2, dict.Count);
        }

        [Fact] // J2N specific
        public void Dictionary_AlternateLookup_GetValueRefOrNullRefClass()
        {
            var dict = new Dictionary<string, IntAsObject>
            {
                {  "1", new IntAsObject() },
                {  "2", new IntAsObject() }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            IntAsObject itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does change values in dictionary
            Assert.Equal(1, lookup["1".AsSpan()].Value);
            Assert.Equal(2, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Value = 3;
            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Property = 4;

            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref IntAsObject itemRef = ref CollectionMarshal.GetValueRefOrNullRef(lookup, "2".AsSpan());

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new IntAsObject() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for null refs

            Assert.True(Unsafe.IsNullRef(ref CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan())));
            Assert.Throws<NullReferenceException>(() => CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan()).Value = 9);

            Assert.Equal(2, dict.Count);
        }

        [Fact] // J2N specific
        public void Dictionary_AlternateLookup_GetValueRefOrNullRefLinkBreaksOnResize()
        {
            var dict = new Dictionary<string, Struct>
            {
                {  "1", new Struct() }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(1, dict.Count);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan());

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 1;
            itemRef.Property = 2;

            Assert.Equal(1, itemRef.Value);
            Assert.Equal(2, itemRef.Property);
            Assert.Equal(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["1".AsSpan()].Property, itemRef.Property);

            // Resize
            dict.EnsureCapacity(100);
            for (int i = 2; i <= 50; i++)
            {
                Assert.True(lookup.TryAdd(i.ToString(CultureInfo.InvariantCulture).AsSpan(), new Struct()));
            }

            itemRef.Value = 3;
            itemRef.Property = 4;

            Assert.Equal(3, itemRef.Value);
            Assert.Equal(4, itemRef.Property);

            // Check connection broken
            Assert.NotEqual(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.NotEqual(lookup["1".AsSpan()].Property, itemRef.Property);

            Assert.Equal(50, dict.Count);
        }

        [Fact] // J2N specific
        public void Dictionary_AlternateLookup_GetValueRefOrAddDefaultValueType()
        {
            // This test is the same as the one for GetValueRefOrNullRef, but it uses
            // GetValueRefOrAddDefault instead, and also checks for incorrect additions.
            // The two APIs should behave the same when values already exist.
            var dict = new Dictionary<string, Struct>
            {
                {  "1", default },
                {  "2", default }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            Struct itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does not change values in dictionary
            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out bool exists).Value = 3;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out exists).Property = 4;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "2".AsSpan(), out exists);

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new Struct() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for correct additions

            ref Struct entry3Ref = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "3".AsSpan(), out exists);

            Assert.False(exists);
            Assert.Equal(3, dict.Count);
            Assert.False(Unsafe.IsNullRef(ref entry3Ref));
            Assert.True(EqualityComparer<Struct>.Default.Equals(entry3Ref, default));

            entry3Ref.Property = 42;
            entry3Ref.Value = 12345;

            Struct value3 = lookup["3".AsSpan()];

            Assert.Equal(42, value3.Property);
            Assert.Equal(12345, value3.Value);
        }

        [Fact] // J2N specific
        public void Dictionary_AlternateLookup_GetValueRefOrAddDefaultClass()
        {
            var dict = new Dictionary<string, IntAsObject>
            {
                {  "1", new IntAsObject() },
                {  "2", new IntAsObject() }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            IntAsObject itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does change values in dictionary
            Assert.Equal(1, lookup["1".AsSpan()].Value);
            Assert.Equal(2, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out bool exists).Value = 3;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out exists).Property = 4;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref IntAsObject itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "2".AsSpan(), out exists);

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new IntAsObject() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for correct additions

            ref IntAsObject entry3Ref = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "3".AsSpan(), out exists);

            Assert.False(exists);
            Assert.Equal(3, dict.Count);
            Assert.False(Unsafe.IsNullRef(ref entry3Ref));
            Assert.Null(entry3Ref);

            entry3Ref = new IntAsObject() { Value = 12345, Property = 42 };

            IntAsObject value3 = lookup["3".AsSpan()];

            Assert.Equal(42, value3.Property);
            Assert.Equal(12345, value3.Value);
        }

        [Fact] // J2N specific
        public void Dictionary_AlternateLookup_GetValueRefOrAddDefaultLinkBreaksOnResize()
        {
            var dict = new Dictionary<string, Struct>
            {
                {  "1", new Struct() }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(1, dict.Count);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out bool exists);

            Assert.True(exists);
            Assert.Equal(1, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 1;
            itemRef.Property = 2;

            Assert.Equal(1, itemRef.Value);
            Assert.Equal(2, itemRef.Property);
            Assert.Equal(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["1".AsSpan()].Property, itemRef.Property);

            // Resize
            dict.EnsureCapacity(100);
            for (int i = 2; i <= 50; i++)
            {
                lookup.TryAdd(i.ToString(CultureInfo.InvariantCulture).AsSpan(), new Struct());
            }

            itemRef.Value = 3;
            itemRef.Property = 4;

            Assert.Equal(3, itemRef.Value);
            Assert.Equal(4, itemRef.Property);

            // Check connection broken
            Assert.NotEqual(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.NotEqual(lookup["1".AsSpan()].Property, itemRef.Property);

            Assert.Equal(50, dict.Count);
        }

#endif

        #endregion Dictionary AlternateLookup

        #region Dictionary SpanAlternateLookup

        [Fact] // J2N specific
        public void Dictionary_SpanAlternateLookup_GetValueRefOrNullRefValueType()
        {
            var dict = new Dictionary<string, Struct>
            {
                {  "1", default },
                {  "2", default }
            };

            var lookup = dict.GetSpanAlternateLookup<char>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            Struct itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does not change values in dictionary
            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Value = 3;
            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Property = 4;

            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrNullRef(lookup, "2".AsSpan());

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new Struct() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for null refs

            Assert.True(Unsafe.IsNullRef(ref CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan())));
            Assert.Throws<NullReferenceException>(() => CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan()).Value = 9);

            Assert.Equal(2, dict.Count);
        }

        [Fact] // J2N specific
        public void Dictionary_SpanAlternateLookup_GetValueRefOrNullRefClass()
        {
            var dict = new Dictionary<string, IntAsObject>
            {
                {  "1", new IntAsObject() },
                {  "2", new IntAsObject() }
            };

            var lookup = dict.GetSpanAlternateLookup<char>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            IntAsObject itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does change values in dictionary
            Assert.Equal(1, lookup["1".AsSpan()].Value);
            Assert.Equal(2, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Value = 3;
            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Property = 4;

            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref IntAsObject itemRef = ref CollectionMarshal.GetValueRefOrNullRef(lookup, "2".AsSpan());

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new IntAsObject() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for null refs

            Assert.True(Unsafe.IsNullRef(ref CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan())));
            Assert.Throws<NullReferenceException>(() => CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan()).Value = 9);

            Assert.Equal(2, dict.Count);
        }

        [Fact] // J2N specific
        public void Dictionary_SpanAlternateLookup_GetValueRefOrNullRefLinkBreaksOnResize()
        {
            var dict = new Dictionary<string, Struct>
            {
                {  "1", new Struct() }
            };

            var lookup = dict.GetSpanAlternateLookup<char>();

            Assert.Equal(1, dict.Count);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan());

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 1;
            itemRef.Property = 2;

            Assert.Equal(1, itemRef.Value);
            Assert.Equal(2, itemRef.Property);
            Assert.Equal(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["1".AsSpan()].Property, itemRef.Property);

            // Resize
            dict.EnsureCapacity(100);
            for (int i = 2; i <= 50; i++)
            {
                Assert.True(lookup.TryAdd(i.ToString(CultureInfo.InvariantCulture).AsSpan(), new Struct()));
            }

            itemRef.Value = 3;
            itemRef.Property = 4;

            Assert.Equal(3, itemRef.Value);
            Assert.Equal(4, itemRef.Property);

            // Check connection broken
            Assert.NotEqual(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.NotEqual(lookup["1".AsSpan()].Property, itemRef.Property);

            Assert.Equal(50, dict.Count);
        }

        [Fact] // J2N specific
        public void Dictionary_SpanAlternateLookup_GetValueRefOrAddDefaultValueType()
        {
            // This test is the same as the one for GetValueRefOrNullRef, but it uses
            // GetValueRefOrAddDefault instead, and also checks for incorrect additions.
            // The two APIs should behave the same when values already exist.
            var dict = new Dictionary<string, Struct>
            {
                {  "1", default },
                {  "2", default }
            };

            var lookup = dict.GetSpanAlternateLookup<char>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            Struct itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does not change values in dictionary
            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out bool exists).Value = 3;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out exists).Property = 4;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "2".AsSpan(), out exists);

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new Struct() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for correct additions

            ref Struct entry3Ref = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "3".AsSpan(), out exists);

            Assert.False(exists);
            Assert.Equal(3, dict.Count);
            Assert.False(Unsafe.IsNullRef(ref entry3Ref));
            Assert.True(EqualityComparer<Struct>.Default.Equals(entry3Ref, default));

            entry3Ref.Property = 42;
            entry3Ref.Value = 12345;

            Struct value3 = lookup["3".AsSpan()];

            Assert.Equal(42, value3.Property);
            Assert.Equal(12345, value3.Value);
        }

        [Fact] // J2N specific
        public void Dictionary_SpanAlternateLookup_GetValueRefOrAddDefaultClass()
        {
            var dict = new Dictionary<string, IntAsObject>
            {
                {  "1", new IntAsObject() },
                {  "2", new IntAsObject() }
            };

            var lookup = dict.GetSpanAlternateLookup<char>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            IntAsObject itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does change values in dictionary
            Assert.Equal(1, lookup["1".AsSpan()].Value);
            Assert.Equal(2, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out bool exists).Value = 3;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out exists).Property = 4;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref IntAsObject itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "2".AsSpan(), out exists);

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new IntAsObject() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for correct additions

            ref IntAsObject entry3Ref = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "3".AsSpan(), out exists);

            Assert.False(exists);
            Assert.Equal(3, dict.Count);
            Assert.False(Unsafe.IsNullRef(ref entry3Ref));
            Assert.Null(entry3Ref);

            entry3Ref = new IntAsObject() { Value = 12345, Property = 42 };

            IntAsObject value3 = lookup["3".AsSpan()];

            Assert.Equal(42, value3.Property);
            Assert.Equal(12345, value3.Value);
        }

        [Fact] // J2N specific
        public void Dictionary_SpanAlternateLookup_GetValueRefOrAddDefaultLinkBreaksOnResize()
        {
            var dict = new Dictionary<string, Struct>
            {
                {  "1", new Struct() }
            };

            var lookup = dict.GetSpanAlternateLookup<char>();

            Assert.Equal(1, dict.Count);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out bool exists);

            Assert.True(exists);
            Assert.Equal(1, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 1;
            itemRef.Property = 2;

            Assert.Equal(1, itemRef.Value);
            Assert.Equal(2, itemRef.Property);
            Assert.Equal(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["1".AsSpan()].Property, itemRef.Property);

            // Resize
            dict.EnsureCapacity(100);
            for (int i = 2; i <= 50; i++)
            {
                lookup.TryAdd(i.ToString(CultureInfo.InvariantCulture).AsSpan(), new Struct());
            }

            itemRef.Value = 3;
            itemRef.Property = 4;

            Assert.Equal(3, itemRef.Value);
            Assert.Equal(4, itemRef.Property);

            // Check connection broken
            Assert.NotEqual(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.NotEqual(lookup["1".AsSpan()].Property, itemRef.Property);

            Assert.Equal(50, dict.Count);
        }

        #endregion Dictionary SpanAlternateLookup

        #region OrderedDictionary

        [Fact]
        public void OrderedDictionary_GetValueRefOrNullRefValueType()
        {
            var dict = new OrderedDictionary<int, Struct>
            {
                {  1, default },
                {  2, default }
            };

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            Struct itemVal = dict[1];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does not change values in dictionary
            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            CollectionMarshal.GetValueRefOrNullRef(dict, 1).Value = 3;
            CollectionMarshal.GetValueRefOrNullRef(dict, 1).Property = 4;

            Assert.Equal(3, dict[1].Value);
            Assert.Equal(4, dict[1].Property);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrNullRef(dict, 2);

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            itemRef = new Struct() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            // Check for null refs

            Assert.True(Unsafe.IsNullRef(ref CollectionMarshal.GetValueRefOrNullRef(dict, 3)));
            Assert.Throws<NullReferenceException>(() => CollectionMarshal.GetValueRefOrNullRef(dict, 3).Value = 9);

            Assert.Equal(2, dict.Count);
        }

        [Fact]
        public void OrderedDictionary_GetValueRefOrNullRefClass()
        {
            var dict = new OrderedDictionary<int, IntAsObject>
            {
                {  1, new IntAsObject() },
                {  2, new IntAsObject() }
            };

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            IntAsObject itemVal = dict[1];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does change values in dictionary
            Assert.Equal(1, dict[1].Value);
            Assert.Equal(2, dict[1].Property);

            CollectionMarshal.GetValueRefOrNullRef(dict, 1).Value = 3;
            CollectionMarshal.GetValueRefOrNullRef(dict, 1).Property = 4;

            Assert.Equal(3, dict[1].Value);
            Assert.Equal(4, dict[1].Property);

            ref IntAsObject itemRef = ref CollectionMarshal.GetValueRefOrNullRef(dict, 2);

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            itemRef = new IntAsObject() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            // Check for null refs

            Assert.True(Unsafe.IsNullRef(ref CollectionMarshal.GetValueRefOrNullRef(dict, 3)));
            Assert.Throws<NullReferenceException>(() => CollectionMarshal.GetValueRefOrNullRef(dict, 3).Value = 9);

            Assert.Equal(2, dict.Count);
        }

        [Fact]
        public void OrderedDictionary_GetValueRefOrNullRefLinkBreaksOnResize()
        {
            var dict = new OrderedDictionary<int, Struct>
            {
                {  1, new Struct() }
            };

            Assert.Equal(1, dict.Count);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrNullRef(dict, 1);

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 1;
            itemRef.Property = 2;

            Assert.Equal(1, itemRef.Value);
            Assert.Equal(2, itemRef.Property);
            Assert.Equal(dict[1].Value, itemRef.Value);
            Assert.Equal(dict[1].Property, itemRef.Property);

            // Resize
            dict.EnsureCapacity(100);
            for (int i = 2; i <= 50; i++)
            {
                dict.Add(i, new Struct());
            }

            itemRef.Value = 3;
            itemRef.Property = 4;

            Assert.Equal(3, itemRef.Value);
            Assert.Equal(4, itemRef.Property);

            // Check connection broken
            Assert.NotEqual(dict[1].Value, itemRef.Value);
            Assert.NotEqual(dict[1].Property, itemRef.Property);

            Assert.Equal(50, dict.Count);
        }

        [Fact]
        public void OrderedDictionary_GetValueRefOrAddDefaultValueType()
        {
            // This test is the same as the one for GetValueRefOrNullRef, but it uses
            // GetValueRefOrAddDefault instead, and also checks for incorrect additions.
            // The two APIs should behave the same when values already exist.
            var dict = new OrderedDictionary<int, Struct>
            {
                {  1, default },
                {  2, default }
            };

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            Struct itemVal = dict[1];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does not change values in dictionary
            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            CollectionMarshal.GetValueRefOrAddDefault(dict, 1, out bool exists).Value = 3;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);

            CollectionMarshal.GetValueRefOrAddDefault(dict, 1, out exists).Property = 4;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(3, dict[1].Value);
            Assert.Equal(4, dict[1].Property);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(dict, 2, out exists);

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            itemRef = new Struct() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            // Check for correct additions

            ref Struct entry3Ref = ref CollectionMarshal.GetValueRefOrAddDefault(dict, 3, out exists);

            Assert.False(exists);
            Assert.Equal(3, dict.Count);
            Assert.False(Unsafe.IsNullRef(ref entry3Ref));
            Assert.True(EqualityComparer<Struct>.Default.Equals(entry3Ref, default));

            entry3Ref.Property = 42;
            entry3Ref.Value = 12345;

            Struct value3 = dict[3];

            Assert.Equal(42, value3.Property);
            Assert.Equal(12345, value3.Value);
        }

        [Fact]
        public void OrderedDictionary_GetValueRefOrAddDefaultClass()
        {
            var dict = new OrderedDictionary<int, IntAsObject>
            {
                {  1, new IntAsObject() },
                {  2, new IntAsObject() }
            };

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, dict[1].Value);
            Assert.Equal(0, dict[1].Property);

            IntAsObject itemVal = dict[1];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does change values in dictionary
            Assert.Equal(1, dict[1].Value);
            Assert.Equal(2, dict[1].Property);

            CollectionMarshal.GetValueRefOrAddDefault(dict, 1, out bool exists).Value = 3;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);

            CollectionMarshal.GetValueRefOrAddDefault(dict, 1, out exists).Property = 4;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(3, dict[1].Value);
            Assert.Equal(4, dict[1].Property);

            ref IntAsObject itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(dict, 2, out exists);

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            itemRef = new IntAsObject() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(dict[2].Value, itemRef.Value);
            Assert.Equal(dict[2].Property, itemRef.Property);

            // Check for correct additions

            ref IntAsObject entry3Ref = ref CollectionMarshal.GetValueRefOrAddDefault(dict, 3, out exists);

            Assert.False(exists);
            Assert.Equal(3, dict.Count);
            Assert.False(Unsafe.IsNullRef(ref entry3Ref));
            Assert.Null(entry3Ref);

            entry3Ref = new IntAsObject() { Value = 12345, Property = 42 };

            IntAsObject value3 = dict[3];

            Assert.Equal(42, value3.Property);
            Assert.Equal(12345, value3.Value);
        }

        [Fact]
        public void OrderedDictionary_GetValueRefOrAddDefaultLinkBreaksOnResize()
        {
            var dict = new OrderedDictionary<int, Struct>
            {
                {  1, new Struct() }
            };

            Assert.Equal(1, dict.Count);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(dict, 1, out bool exists);

            Assert.True(exists);
            Assert.Equal(1, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 1;
            itemRef.Property = 2;

            Assert.Equal(1, itemRef.Value);
            Assert.Equal(2, itemRef.Property);
            Assert.Equal(dict[1].Value, itemRef.Value);
            Assert.Equal(dict[1].Property, itemRef.Property);

            // Resize
            dict.EnsureCapacity(100);
            for (int i = 2; i <= 50; i++)
            {
                dict.Add(i, new Struct());
            }

            itemRef.Value = 3;
            itemRef.Property = 4;

            Assert.Equal(3, itemRef.Value);
            Assert.Equal(4, itemRef.Property);

            // Check connection broken
            Assert.NotEqual(dict[1].Value, itemRef.Value);
            Assert.NotEqual(dict[1].Property, itemRef.Property);

            Assert.Equal(50, dict.Count);
        }

        #endregion OrderedDictionary

        #region OrderedDictionary AlternateLookup

#if FEATURE_IALTERNATEEQUALITYCOMPARER

        [Fact] // J2N specific
        public void OrderedDictionary_AlternateLookup_GetValueRefOrNullRefValueType()
        {
            var dict = new OrderedDictionary<string, Struct>
            {
                {  "1", default },
                {  "2", default }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            Struct itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does not change values in dictionary
            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Value = 3;
            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Property = 4;

            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrNullRef(lookup, "2".AsSpan());

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new Struct() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for null refs

            Assert.True(Unsafe.IsNullRef(ref CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan())));
            Assert.Throws<NullReferenceException>(() => CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan()).Value = 9);

            Assert.Equal(2, dict.Count);
        }

        [Fact] // J2N specific
        public void OrderedDictionary_AlternateLookup_GetValueRefOrNullRefClass()
        {
            var dict = new OrderedDictionary<string, IntAsObject>
            {
                {  "1", new IntAsObject() },
                {  "2", new IntAsObject() }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            IntAsObject itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does change values in dictionary
            Assert.Equal(1, lookup["1".AsSpan()].Value);
            Assert.Equal(2, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Value = 3;
            CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan()).Property = 4;

            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref IntAsObject itemRef = ref CollectionMarshal.GetValueRefOrNullRef(lookup, "2".AsSpan());

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new IntAsObject() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for null refs

            Assert.True(Unsafe.IsNullRef(ref CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan())));
            Assert.Throws<NullReferenceException>(() => CollectionMarshal.GetValueRefOrNullRef(lookup, "3".AsSpan()).Value = 9);

            Assert.Equal(2, dict.Count);
        }

        [Fact] // J2N specific
        public void OrderedDictionary_AlternateLookup_GetValueRefOrNullRefLinkBreaksOnResize()
        {
            var dict = new OrderedDictionary<string, Struct>
            {
                {  "1", new Struct() }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(1, dict.Count);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrNullRef(lookup, "1".AsSpan());

            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 1;
            itemRef.Property = 2;

            Assert.Equal(1, itemRef.Value);
            Assert.Equal(2, itemRef.Property);
            Assert.Equal(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["1".AsSpan()].Property, itemRef.Property);

            // Resize
            dict.EnsureCapacity(100);
            for (int i = 2; i <= 50; i++)
            {
                Assert.True(lookup.TryAdd(i.ToString(CultureInfo.InvariantCulture).AsSpan(), new Struct()));
            }

            itemRef.Value = 3;
            itemRef.Property = 4;

            Assert.Equal(3, itemRef.Value);
            Assert.Equal(4, itemRef.Property);

            // Check connection broken
            Assert.NotEqual(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.NotEqual(lookup["1".AsSpan()].Property, itemRef.Property);

            Assert.Equal(50, dict.Count);
        }

        [Fact] // J2N specific
        public void OrderedDictionary_AlternateLookup_GetValueRefOrAddDefaultValueType()
        {
            // This test is the same as the one for GetValueRefOrNullRef, but it uses
            // GetValueRefOrAddDefault instead, and also checks for incorrect additions.
            // The two APIs should behave the same when values already exist.
            var dict = new OrderedDictionary<string, Struct>
            {
                {  "1", default },
                {  "2", default }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            Struct itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does not change values in dictionary
            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out bool exists).Value = 3;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out exists).Property = 4;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "2".AsSpan(), out exists);

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new Struct() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for correct additions

            ref Struct entry3Ref = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "3".AsSpan(), out exists);

            Assert.False(exists);
            Assert.Equal(3, dict.Count);
            Assert.False(Unsafe.IsNullRef(ref entry3Ref));
            Assert.True(EqualityComparer<Struct>.Default.Equals(entry3Ref, default));

            entry3Ref.Property = 42;
            entry3Ref.Value = 12345;

            Struct value3 = lookup["3".AsSpan()];

            Assert.Equal(42, value3.Property);
            Assert.Equal(12345, value3.Value);
        }

        [Fact] // J2N specific
        public void OrderedDictionary_AlternateLookup_GetValueRefOrAddDefaultClass()
        {
            var dict = new OrderedDictionary<string, IntAsObject>
            {
                {  "1", new IntAsObject() },
                {  "2", new IntAsObject() }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(2, dict.Count);

            Assert.Equal(0, lookup["1".AsSpan()].Value);
            Assert.Equal(0, lookup["1".AsSpan()].Property);

            IntAsObject itemVal = lookup["1".AsSpan()];
            itemVal.Value = 1;
            itemVal.Property = 2;

            // Does change values in dictionary
            Assert.Equal(1, lookup["1".AsSpan()].Value);
            Assert.Equal(2, lookup["1".AsSpan()].Property);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out bool exists).Value = 3;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);

            CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out exists).Property = 4;

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(3, lookup["1".AsSpan()].Value);
            Assert.Equal(4, lookup["1".AsSpan()].Property);

            ref IntAsObject itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "2".AsSpan(), out exists);

            Assert.True(exists);
            Assert.Equal(2, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 5;
            itemRef.Property = 6;

            Assert.Equal(5, itemRef.Value);
            Assert.Equal(6, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            itemRef = new IntAsObject() { Value = 7, Property = 8 };

            Assert.Equal(7, itemRef.Value);
            Assert.Equal(8, itemRef.Property);
            Assert.Equal(lookup["2".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["2".AsSpan()].Property, itemRef.Property);

            // Check for correct additions

            ref IntAsObject entry3Ref = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "3".AsSpan(), out exists);

            Assert.False(exists);
            Assert.Equal(3, dict.Count);
            Assert.False(Unsafe.IsNullRef(ref entry3Ref));
            Assert.Null(entry3Ref);

            entry3Ref = new IntAsObject() { Value = 12345, Property = 42 };

            IntAsObject value3 = lookup["3".AsSpan()];

            Assert.Equal(42, value3.Property);
            Assert.Equal(12345, value3.Value);
        }

        [Fact] // J2N specific
        public void OrderedDictionary_AlternateLookup_GetValueRefOrAddDefaultLinkBreaksOnResize()
        {
            var dict = new OrderedDictionary<string, Struct>
            {
                {  "1", new Struct() }
            };

            var lookup = dict.GetAlternateLookup<ReadOnlySpan<char>>();

            Assert.Equal(1, dict.Count);

            ref Struct itemRef = ref CollectionMarshal.GetValueRefOrAddDefault(lookup, "1".AsSpan(), out bool exists);

            Assert.True(exists);
            Assert.Equal(1, dict.Count);
            Assert.Equal(0, itemRef.Value);
            Assert.Equal(0, itemRef.Property);

            itemRef.Value = 1;
            itemRef.Property = 2;

            Assert.Equal(1, itemRef.Value);
            Assert.Equal(2, itemRef.Property);
            Assert.Equal(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.Equal(lookup["1".AsSpan()].Property, itemRef.Property);

            // Resize
            dict.EnsureCapacity(100);
            for (int i = 2; i <= 50; i++)
            {
                lookup.TryAdd(i.ToString(CultureInfo.InvariantCulture).AsSpan(), new Struct());
            }

            itemRef.Value = 3;
            itemRef.Property = 4;

            Assert.Equal(3, itemRef.Value);
            Assert.Equal(4, itemRef.Property);

            // Check connection broken
            Assert.NotEqual(lookup["1".AsSpan()].Value, itemRef.Value);
            Assert.NotEqual(lookup["1".AsSpan()].Property, itemRef.Property);

            Assert.Equal(50, dict.Count);
        }

#endif

        #endregion OrderedDictionary AlternateLookup

        private struct Struct
        {
            public int Value;
            public int Property { get; set; }
        }

        private class IntAsObject
        {
            public int Value;
            public int Property { get; set; }
        }

        [Fact]
        public void ListSetCount()
        {
            List<int> list = null;
            // J2N: Changed API to throw ArgumentNullException instead of NullReferenceException.
            // To match other APIs, the null check is done first.
            Assert.Throws<ArgumentNullException>(() => CollectionMarshal.SetCount(list, 3));

            //Assert.Throws<ArgumentOutOfRangeException>(() => CollectionMarshal.SetCount(list, -1));

            list = new List<int>();
            Assert.Throws<ArgumentOutOfRangeException>(() => CollectionMarshal.SetCount(list, -1));

            CollectionMarshal.SetCount(list, 5);
            Assert.Equal(5, list.Count);

            list = new List<int>() { 1, 2, 3, 4, 5 };
            ref int intRef = ref MemoryMarshal.GetReference(CollectionMarshal.AsSpan(list));

            // make sure that size decrease preserves content
            CollectionMarshal.SetCount(list, 3);
            Assert.Equal(3, list.Count);
            Assert.Throws<ArgumentOutOfRangeException>(() => list[3]);
            AssertExtensions.SequenceEqual(CollectionMarshal.AsSpan(list), new int[] { 1, 2, 3 });
            Assert.True(Unsafe.AreSame(ref intRef, ref MemoryMarshal.GetReference(CollectionMarshal.AsSpan(list))));

            // make sure that size increase preserves content and doesn't clear
            CollectionMarshal.SetCount(list, 5);
            AssertExtensions.SequenceEqual(CollectionMarshal.AsSpan(list), new int[] { 1, 2, 3, 4, 5 });
            Assert.True(Unsafe.AreSame(ref intRef, ref MemoryMarshal.GetReference(CollectionMarshal.AsSpan(list))));

            // make sure that reallocations preserve content
            int newCount = list.Capacity * 2;
            CollectionMarshal.SetCount(list, newCount);
            Assert.Equal(newCount, list.Count);
            AssertExtensions.SequenceEqual(CollectionMarshal.AsSpan(list).Slice(0, 3) /*[..3]*/, new int[] { 1, 2, 3 });
            Assert.True(!Unsafe.AreSame(ref intRef, ref MemoryMarshal.GetReference(CollectionMarshal.AsSpan(list))));

            List<string> listReference = new List<string>() { "a", "b", "c", "d", "e" };
            ref string stringRef = ref MemoryMarshal.GetReference(CollectionMarshal.AsSpan(listReference));
            CollectionMarshal.SetCount(listReference, 3);

            // verify that reference types aren't cleared
            AssertExtensions.SequenceEqual(CollectionMarshal.AsSpan(listReference), new string[] { "a", "b", "c" });
            Assert.True(Unsafe.AreSame(ref stringRef, ref MemoryMarshal.GetReference(CollectionMarshal.AsSpan(listReference))));
            CollectionMarshal.SetCount(listReference, 5);

            // verify that removed reference types are cleared
            AssertExtensions.SequenceEqual(CollectionMarshal.AsSpan(listReference), new string[] { "a", "b", "c", null, null });
            Assert.True(Unsafe.AreSame(ref stringRef, ref MemoryMarshal.GetReference(CollectionMarshal.AsSpan(listReference))));
        }
    }
}
