// Source: https://github.com/dotnet/runtime/blob/v9.0.0/src/libraries/System.Runtime/tests/System.Runtime.Tests/System/Text/StringBuilderReplaceTests.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using J2N.Buffers;
using J2N.TestUtilities.Xunit;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Xunit;

namespace J2N.Text.Tests
{
    /// <summary>
    /// Contains tests that ensure the correctness (compliance with the BCL) of an <see cref="MutableTextBuffer"/> implementation
    /// including subclasses.
    /// </summary>
    /// <remarks>
    /// J2N: This class does not map exactly to the upstream code. It was refactored to be an abstract class that can be used for testing
    /// multiple implementations of <see cref="MutableTextBuffer"/>. Each implementation is presumed to have the same behavior, but
    /// may have different internal implementations. For example, one implementation may use a buffer that is allocated on the heap,
    /// while another may use a buffer that is allocated from an array pool. The tests in this class are designed to ensure that all
    /// implementations behave correctly and consistently with the BCL.
    /// </remarks>
    public abstract partial class StringBuilder_Tests
    {
        public static IEnumerable<object[]> Replace_TestData()
        {
            yield return new object[] { "", "a", "!", 0, 0, "" };
            yield return new object[] { "aaaabbbbccccdddd", "a", "!", 0, 16, "!!!!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "a", "!", 2, 3, "aa!!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "a", "!", 4, 1, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aab", "!", 2, 2, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aab", "!", 2, 3, "aa!bbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aa", "!", 0, 16, "!!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aa", "$!", 0, 16, "$!$!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aa", "$!$", 0, 16, "$!$$!$bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aaaa", "!", 0, 16, "!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aaaa", "$!", 0, 16, "$!bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "a", "", 0, 16, "bbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "b", null, 0, 16, "aaaaccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aaaabbbbccccdddd", "", 0, 16, "" };
            yield return new object[] { "aaaabbbbccccdddd", "aaaabbbbccccdddd", "", 16, 0, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", "aaaabbbbccccdddde", "", 0, 16, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", "a", "b", 0, 16, "bbbbbbbbbbbbbbbb" };

            // Regression tests for issues found during initial AI review
            yield return new object[] { "a-a", "a", "ZZ", 0, 3, "ZZ-ZZ" };
            yield return new object[] { "axa", "a", "XX", 0, 3, "XXxXX" };
            yield return new object[] { "abab", "b", "YYY", 0, 4, "aYYYaYYY" };
            yield return new object[] { "a,a,a", "a", "QQ", 0, 5, "QQ,QQ,QQ" };
            yield return new object[] { "foobar", "ba", "oo", 0, 6, "foooor" };

            // Expansion path
            yield return new object[] { "a-a", "a", "XXXXXXXXXXXXXXXXXXXX", 0, 3, "XXXXXXXXXXXXXXXXXXXX-XXXXXXXXXXXXXXXXXXXX" };
            yield return new object[] { "abab", "b", "XXXXXXXXXXXXXXXXXXXX", 0, 4, "aXXXXXXXXXXXXXXXXXXXXaXXXXXXXXXXXXXXXXXXXX" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", "a", "XXXXXXXXXXXXXXXXXXXX", 0, 16, "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" };
            yield return new object[] { "abababababababab", "b", "XXXXXXXXXXXXXXXXXXXX", 0, 16, "aXXXXXXXXXXXXXXXXXXXXaXXXXXXXXXXXXXXXXXXXXaXXXXXXXXXXXXXXXXXXXXaXXXXXXXXXXXXXXXXXXXXaXXXXXXXXXXXXXXXXXXXXaXXXXXXXXXXXXXXXXXXXXaXXXXXXXXXXXXXXXXXXXXaXXXXXXXXXXXXXXXXXXXX" };
            yield return new object[] { "a,a,a,a,a,a,a,a", "a", "XXXXXXXXXXXXXXXXXXXX", 0, 15, "XXXXXXXXXXXXXXXXXXXX,XXXXXXXXXXXXXXXXXXXX,XXXXXXXXXXXXXXXXXXXX,XXXXXXXXXXXXXXXXXXXX,XXXXXXXXXXXXXXXXXXXX,XXXXXXXXXXXXXXXXXXXX,XXXXXXXXXXXXXXXXXXXX,XXXXXXXXXXXXXXXXXXXX" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", "aa", "XXXXXXXXXXXXXXXXXXXX", 0, 16, "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" };
            yield return new object[] { "abababababababab", "ab", "XXXXXXXXXXXXXXXXXXXX", 0, 16, "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", "a", "", 0, 16, "" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", "a", "XXXXXXXXXXXXXXXXXXXX", 2, 12, "aaXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXaa" };
            yield return new object[] { "aaaabbbbccccddddaaaabbbbccccdddd", "bbbb", "XXXXXXXXXXXXXXXXXXXX", 0, 32, "aaaaXXXXXXXXXXXXXXXXXXXXccccddddaaaaXXXXXXXXXXXXXXXXXXXXccccdddd" };
            yield return new object[] { "012345678901234567890123456789", "0", "XXXXXXXXXXXXXXXXXXXX", 0, 30, "XXXXXXXXXXXXXXXXXXXX123456789XXXXXXXXXXXXXXXXXXXX123456789XXXXXXXXXXXXXXXXXXXX123456789" };
            yield return new object[] { "xyxyxyxyxyxyxyxy", "xy", "XXXXXXXXXXXXXXXXXXXX", 0, 16, "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" };
            yield return new object[] { "startstartstarttartcart55", "art", "_AAABBBCCCDDDEEEFFFGGG_", 0, 25, "st_AAABBBCCCDDDEEEFFFGGG_st_AAABBBCCCDDDEEEFFFGGG_st_AAABBBCCCDDDEEEFFFGGG_t_AAABBBCCCDDDEEEFFFGGG_c_AAABBBCCCDDDEEEFFFGGG_55" };

        }

        [Theory]
        [MemberData(nameof(Replace_TestData))]
        public void Replace_String(string value, string oldValue, string newValue, int startIndex, int count, string expected)
        {
            MutableTextBuffer builder;
            if (startIndex == 0 && count == value.Length)
            {
                // Use Replace(string, string)
                builder = MutableTextBufferFactory(value);
                builder.Replace(oldValue, newValue);
                Assert.Equal(expected, builder.ToString());
            }

            // Use Replace(string, string, int, int)
            builder = MutableTextBufferFactory(value);
            builder.Replace(oldValue, newValue, startIndex, count);
            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Replace_TestData))]
        public void Replace_CharSpan(string value, string oldValue, string newValue, int startIndex, int count, string expected)
        {
            MutableTextBuffer builder;
            if (startIndex == 0 && count == value.Length)
            {
                // Use Replace(ReadOnlySpan<char>, ReadOnlySpan<char>)
                builder = MutableTextBufferFactory(value);
                builder.Replace(oldValue.AsSpan(), newValue.AsSpan());
                Assert.Equal(expected, builder.ToString());
            }

            // Use Replace(ReadOnlySpan<char>, ReadOnlySpan<char>, int, int)
            builder = MutableTextBufferFactory(value);
            builder.Replace(oldValue.AsSpan(), newValue.AsSpan(), startIndex, count);
            Assert.Equal(expected, builder.ToString());
        }

        public static IEnumerable<object[]> Replace_Overlapping_TestData()
        {
            yield return new object[] { "foobar", TestValue.Literal("ba"), TestValue.Builder(1, 2), 0, 6, "foooor" };
            yield return new object[] { "foobar", TestValue.Builder(3, 2), TestValue.Literal("oo"), 0, 6, "foooor" };
            yield return new object[] { "foobar", TestValue.Builder(3, 2), TestValue.Builder(1, 2), 0, 6, "foooor" };

            yield return new object[] { "foobar", TestValue.Builder(0, 3), TestValue.Literal("awnughcoo"), 0, 6, "awnughcoobar" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(0, 10), TestValue.Literal("qkajelrfvrt"), 0, 16, "qkajelrfvrtccdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(0, 11), TestValue.Literal("ncpcaqghbvx"), 0, 16, "ncpcaqghbvxcdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(8, 6), TestValue.Literal("sytssuartco"), 0, 16, "aaaabbbbsytssuartcodd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(10, 5), TestValue.Literal("zuvqimjokw"), 0, 16, "aaaabbbbcczuvqimjokwd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(8, 7), TestValue.Literal("qxwu"), 0, 16, "aaaabbbbqxwud" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(11, 3), TestValue.Literal("fvk"), 0, 16, "aaaabbbbcccfvkdd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(0, 11), TestValue.Literal("ospommcq"), 0, 16, "ospommcqcdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(4, 7), TestValue.Literal("sxkcch"), 0, 16, "aaaasxkcchcdddd" };

            yield return new object[] { "a-a", TestValue.Builder(0, 2), TestValue.Literal("ru"), 0, 3, "rua" };

            yield return new object[] { "axa", TestValue.Builder(0, 2), TestValue.Literal("saa"), 0, 3, "saaa" };

            yield return new object[] { "abab", TestValue.Builder(0, 3), TestValue.Literal("fxfzsrafz"), 0, 4, "fxfzsrafzb" };

            yield return new object[] { "a,a,a", TestValue.Builder(0, 3), TestValue.Literal("kgedapdod"), 0, 5, "kgedapdod,a" };

            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(10, 2), TestValue.Literal("rjhyd"), 0, 16, "rjhydrjhydrjhydrjhydrjhydrjhydrjhydrjhyd" };

            yield return new object[] { "abababababababab", TestValue.Builder(8, 7), TestValue.Literal("rh"), 0, 16, "rhbrhb" };

            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(3, 6), TestValue.Literal("bpfipvgutquq"), 0, 16, "bpfipvgutquqbpfipvgutquqaaaa" };

            yield return new object[] { "abababababababab", TestValue.Builder(0, 9), TestValue.Literal("yzqiancsjfvb"), 0, 16, "yzqiancsjfvbbababab" };

            yield return new object[] { "a,a,a,a,a,a,a,a", TestValue.Builder(8, 4), TestValue.Literal("eibikrujygzo"), 0, 15, "eibikrujygzoeibikrujygzoeibikrujygzoa,a" };

            yield return new object[] { "aaaabbbbccccddddaaaabbbbccccdddd", TestValue.Builder(11, 7), TestValue.Literal("c"), 0, 32, "aaaabbbbccccaabbbbccccdddd" };

            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(24, 2), TestValue.Literal("r"), 0, 30, "0123r67890123r67890123r6789" };

            yield return new object[] { "xyxyxyxyxyxyxyxy", TestValue.Builder(7, 3), TestValue.Literal("ehlkxuts"), 0, 16, "xehlkxutsxehlkxutsxehlkxutsxehlkxuts" };

            yield return new object[] { "startstartstarttartcart55", TestValue.Builder(19, 3), TestValue.Literal("mspea"), 0, 25, "startstartstarttartmspeat55" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(7, 7), TestValue.Literal("mshn"), 2, 3, "aaaabbbbccccdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(1, 11), TestValue.Literal(""), 4, 4, "aaaabbbbccccdddd" };

            yield return new object[] { "abababababab", TestValue.Builder(5, 6), TestValue.Literal("dpykxl"), 2, 8, "abadpykxlbab" };

            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(11, 4), TestValue.Literal("xeyyakqsoe"), 2, 12, "aaxeyyakqsoexeyyakqsoexeyyakqsoeaa" };

            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", TestValue.Builder(3, 17), TestValue.Literal("hbvdwil"), 0, 26, "abchbvdwiluvwxyz" };

            yield return new object[] { "foobar", TestValue.Builder(1, 4), TestValue.Literal("ytpzxz"), 0, 6, "fytpzxzr" };
            yield return new object[] { "foobar", TestValue.Literal("ba"), TestValue.Builder(0, 2), 0, 6, "foofor" };
            yield return new object[] { "foobar", TestValue.Builder(1, 4), TestValue.Builder(0, 2), 0, 6, "ffor" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(6, 2), TestValue.Literal("gao"), 0, 16, "aaaagaogaoccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("a"), TestValue.Builder(5, 6), 0, 16, "bbbcccbbbcccbbbcccbbbcccbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(6, 2), TestValue.Builder(5, 6), 0, 16, "aaaabbbcccbbbcccccccdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(4, 10), TestValue.Literal("ceoeedxlv"), 0, 16, "aaaaceoeedxlvdd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("aa"), TestValue.Builder(5, 6), 0, 16, "bbbcccbbbcccbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(4, 10), TestValue.Builder(5, 6), 0, 16, "aaaabbbcccdd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(5, 6), TestValue.Literal("xnm"), 0, 16, "aaaabxnmcdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("aa"), TestValue.Builder(5, 5), 0, 16, "bbbccbbbccbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(5, 6), TestValue.Builder(5, 5), 0, 16, "aaaabbbbcccdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(1, 5), TestValue.Literal("ammrnkotzg"), 0, 16, "aammrnkotzgbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("aaaa"), TestValue.Builder(0, 9), 0, 16, "aaaabbbbcbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(1, 5), TestValue.Builder(0, 9), 0, 16, "aaaaabbbbcbbccccdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(2, 6), TestValue.Literal("cxzxlpibra"), 0, 16, "aacxzxlpibraccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("aaaa"), TestValue.Builder(11, 3), 0, 16, "cddbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(2, 6), TestValue.Builder(11, 3), 0, 16, "aacddccccdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(11, 3), TestValue.Literal(""), 0, 16, "aaaabbbbcccdd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("a"), TestValue.Builder(1, 9), 0, 16, "aaabbbbccaaabbbbccaaabbbbccaaabbbbccbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(11, 3), TestValue.Builder(1, 9), 0, 16, "aaaabbbbcccaaabbbbccdd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(8, 4), TestValue.Literal("muthgz"), 0, 16, "aaaabbbbmuthgzdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("bbbb"), TestValue.Builder(5, 3), 0, 16, "aaaabbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(8, 4), TestValue.Builder(5, 3), 0, 16, "aaaabbbbbbbdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(11, 3), TestValue.Literal("svrr"), 0, 16, "aaaabbbbcccsvrrdd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("aaaabbbbccccdddd"), TestValue.Builder(1, 7), 0, 16, "aaabbbb" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(11, 3), TestValue.Builder(1, 7), 0, 16, "aaaabbbbcccaaabbbbdd" };

            yield return new object[] { "a-a", TestValue.Builder(0, 2), TestValue.Literal("ljnct"), 0, 3, "ljncta" };
            yield return new object[] { "a-a", TestValue.Literal("a"), TestValue.Builder(0, 2), 0, 3, "a--a-" };
            yield return new object[] { "a-a", TestValue.Builder(0, 2), TestValue.Builder(0, 2), 0, 3, "a-a" };

            yield return new object[] { "axa", TestValue.Builder(0, 2), TestValue.Literal("orhet"), 0, 3, "orheta" };
            yield return new object[] { "axa", TestValue.Literal("a"), TestValue.Builder(0, 2), 0, 3, "axxax" };
            yield return new object[] { "axa", TestValue.Builder(0, 2), TestValue.Builder(0, 2), 0, 3, "axa" };

            yield return new object[] { "abab", TestValue.Builder(0, 2), TestValue.Literal("sqygaef"), 0, 4, "sqygaefsqygaef" };
            yield return new object[] { "abab", TestValue.Literal("b"), TestValue.Builder(0, 3), 0, 4, "aabaaaba" };
            yield return new object[] { "abab", TestValue.Builder(0, 2), TestValue.Builder(0, 3), 0, 4, "abaaba" };

            yield return new object[] { "a,a,a", TestValue.Builder(0, 2), TestValue.Literal("lxtdplriigua"), 0, 5, "lxtdplriigualxtdplriiguaa" };
            yield return new object[] { "a,a,a", TestValue.Literal("a"), TestValue.Builder(0, 3), 0, 5, "a,a,a,a,a,a" };
            yield return new object[] { "a,a,a", TestValue.Builder(0, 2), TestValue.Builder(0, 3), 0, 5, "a,aa,aa" };

            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(2, 11), TestValue.Literal("bloyxvsbi"), 0, 16, "bloyxvsbiaaaaa" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Literal("a"), TestValue.Builder(6, 3), 0, 16, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(2, 11), TestValue.Builder(6, 3), 0, 16, "aaaaaaaa" };

            yield return new object[] { "abababababababab", TestValue.Builder(2, 12), TestValue.Literal("mtdpgylickm"), 0, 16, "mtdpgylickmabab" };
            yield return new object[] { "abababababababab", TestValue.Literal("b"), TestValue.Builder(1, 9), 0, 16, "abababababababababababababababababababababababababababababababababababababababab" };
            yield return new object[] { "abababababababab", TestValue.Builder(2, 12), TestValue.Builder(1, 9), 0, 16, "babababababab" };

            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(2, 11), TestValue.Literal("y"), 0, 16, "yaaaaa" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Literal("aa"), TestValue.Builder(11, 3), 0, 16, "aaaaaaaaaaaaaaaaaaaaaaaa" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(2, 11), TestValue.Builder(11, 3), 0, 16, "aaaaaaaa" };

            yield return new object[] { "abababababababab", TestValue.Builder(2, 7), TestValue.Literal("tb"), 0, 16, "tbbtbb" };
            yield return new object[] { "abababababababab", TestValue.Literal("ab"), TestValue.Builder(0, 5), 0, 16, "ababaababaababaababaababaababaababaababa" };
            yield return new object[] { "abababababababab", TestValue.Builder(2, 7), TestValue.Builder(0, 5), 0, 16, "abababababab" };

            yield return new object[] { "a,a,a,a,a,a,a,a", TestValue.Builder(0, 2), TestValue.Literal("dpgdjzdtqarh"), 0, 15, "dpgdjzdtqarhdpgdjzdtqarhdpgdjzdtqarhdpgdjzdtqarhdpgdjzdtqarhdpgdjzdtqarhdpgdjzdtqarha" };
            yield return new object[] { "a,a,a,a,a,a,a,a", TestValue.Literal("a"), TestValue.Builder(10, 4), 0, 15, "a,a,,a,a,,a,a,,a,a,,a,a,,a,a,,a,a,,a,a," };
            yield return new object[] { "a,a,a,a,a,a,a,a", TestValue.Builder(0, 2), TestValue.Builder(10, 4), 0, 15, "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a" };

            yield return new object[] { "aaaabbbbccccddddaaaabbbbccccdddd", TestValue.Builder(15, 2), TestValue.Literal("lguz"), 0, 32, "aaaabbbbccccdddlguzaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccddddaaaabbbbccccdddd", TestValue.Literal("bbbb"), TestValue.Builder(10, 14), 0, 32, "aaaaccddddaaaabbbbccccddddaaaaccddddaaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccddddaaaabbbbccccdddd", TestValue.Builder(15, 2), TestValue.Builder(10, 14), 0, 32, "aaaabbbbccccdddccddddaaaabbbbaaabbbbccccdddd" };

            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(17, 10), TestValue.Literal("mdusl"), 0, 30, "0123456mduslmdusl789" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Literal("0"), TestValue.Builder(18, 3), 0, 30, "890123456789890123456789890123456789" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(17, 10), TestValue.Builder(18, 3), 0, 30, "0123456890890789" };

            yield return new object[] { "xyxyxyxyxyxyxyxy", TestValue.Builder(2, 6), TestValue.Literal("uncdzob"), 0, 16, "uncdzobuncdzobxyxy" };
            yield return new object[] { "xyxyxyxyxyxyxyxy", TestValue.Literal("xy"), TestValue.Builder(5, 7), 0, 16, "yxyxyxyyxyxyxyyxyxyxyyxyxyxyyxyxyxyyxyxyxyyxyxyxyyxyxyxy" };
            yield return new object[] { "xyxyxyxyxyxyxyxy", TestValue.Builder(2, 6), TestValue.Builder(5, 7), 0, 16, "yxyxyxyyxyxyxyxyxy" };

            yield return new object[] { "startstartstarttartcart55", TestValue.Builder(20, 4), TestValue.Literal("kycwzhwmijfg"), 0, 25, "startstartstarttartckycwzhwmijfg5" };
            yield return new object[] { "startstartstarttartcart55", TestValue.Literal("art"), TestValue.Builder(20, 3), 0, 25, "startstartstarttartcart55" };
            yield return new object[] { "startstartstarttartcart55", TestValue.Builder(20, 4), TestValue.Builder(20, 3), 0, 25, "startstartstarttartcart5" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(7, 3), TestValue.Literal("ktxwmfyzrgfi"), 2, 3, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("a"), TestValue.Builder(8, 5), 2, 3, "aaccccdccccdbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(7, 3), TestValue.Builder(8, 5), 2, 3, "aaaabbbbccccdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(6, 3), TestValue.Literal("pkjathsuu"), 4, 4, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("bb"), TestValue.Builder(9, 4), 4, 4, "aaaacccdcccdccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(6, 3), TestValue.Builder(9, 4), 4, 4, "aaaabbbbccccdddd" };

            yield return new object[] { "abababababab", TestValue.Builder(4, 6), TestValue.Literal("u"), 2, 8, "abuabab" };
            yield return new object[] { "abababababab", TestValue.Literal("ab"), TestValue.Builder(3, 8), 2, 8, "abbabababababababababababababababaab" };
            yield return new object[] { "abababababab", TestValue.Builder(4, 6), TestValue.Builder(3, 8), 2, 8, "abbabababaabab" };

            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(8, 7), TestValue.Literal("lhemo"), 2, 12, "aalhemoaaaaaaa" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Literal("a"), TestValue.Builder(6, 2), 2, 12, "aaaaaaaaaaaaaaaaaaaaaaaaaaaa" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(8, 7), TestValue.Builder(6, 2), 2, 12, "aaaaaaaaaaa" };

            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", TestValue.Builder(16, 3), TestValue.Literal("yv"), 0, 26, "abcdefghijklmnopyvtuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", TestValue.Literal("abcdefghijklmnopqrstuvwxyz"), TestValue.Builder(21, 3), 0, 26, "vwx" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", TestValue.Builder(16, 3), TestValue.Builder(21, 3), 0, 26, "abcdefghijklmnopvwxtuvwxyz" };


            yield return new object[] { "foobar", TestValue.Builder(0, 2), TestValue.Literal("dkyukhupa"), 0, 6, "dkyukhupaobar" };
            yield return new object[] { "foobar", TestValue.Literal("ba"), TestValue.Builder(1, 2), 0, 6, "foooor" };
            yield return new object[] { "foobar", TestValue.Builder(0, 2), TestValue.Builder(1, 2), 0, 6, "ooobar" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(11, 4), TestValue.Literal("fspxuic"), 0, 16, "aaaabbbbcccfspxuicd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("a"), TestValue.Builder(4, 3), 0, 16, "bbbbbbbbbbbbbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(11, 4), TestValue.Builder(4, 3), 0, 16, "aaaabbbbcccbbbd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(7, 8), TestValue.Literal(""), 0, 16, "aaaabbbd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("aa"), TestValue.Builder(5, 6), 0, 16, "bbbcccbbbcccbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(7, 8), TestValue.Builder(5, 6), 0, 16, "aaaabbbbbbcccd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(10, 3), TestValue.Literal("hlq"), 0, 16, "aaaabbbbcchlqddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("aa"), TestValue.Builder(10, 5), 0, 16, "ccdddccdddbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(10, 3), TestValue.Builder(10, 5), 0, 16, "aaaabbbbccccdddddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(3, 8), TestValue.Literal("nvrzkvybkqr"), 0, 16, "aaanvrzkvybkqrcdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("aaaa"), TestValue.Builder(0, 8), 0, 16, "aaaabbbbbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(3, 8), TestValue.Builder(0, 8), 0, 16, "aaaaaaabbbbcdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(1, 7), TestValue.Literal("cf"), 0, 16, "acfccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("aaaa"), TestValue.Builder(10, 3), 0, 16, "ccdbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(1, 7), TestValue.Builder(10, 3), 0, 16, "accdccccdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(5, 7), TestValue.Literal("rhcgwvbjuhmp"), 0, 16, "aaaabrhcgwvbjuhmpdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("a"), TestValue.Builder(11, 2), 0, 16, "cdcdcdcdbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(5, 7), TestValue.Builder(11, 2), 0, 16, "aaaabcddddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(6, 4), TestValue.Literal(""), 0, 16, "aaaabbccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("bbbb"), TestValue.Builder(7, 6), 0, 16, "aaaabccccdccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(6, 4), TestValue.Builder(7, 6), 0, 16, "aaaabbbccccdccdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(9, 5), TestValue.Literal("rlfpirao"), 0, 16, "aaaabbbbcrlfpiraodd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("aaaabbbbccccdddd"), TestValue.Builder(10, 2), 0, 16, "cc" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(9, 5), TestValue.Builder(10, 2), 0, 16, "aaaabbbbcccdd" };

            yield return new object[] { "a-a", TestValue.Builder(0, 2), TestValue.Literal("mvgf"), 0, 3, "mvgfa" };
            yield return new object[] { "a-a", TestValue.Literal("a"), TestValue.Builder(0, 2), 0, 3, "a--a-" };
            yield return new object[] { "a-a", TestValue.Builder(0, 2), TestValue.Builder(0, 2), 0, 3, "a-a" };

            yield return new object[] { "axa", TestValue.Builder(0, 2), TestValue.Literal("qwjloae"), 0, 3, "qwjloaea" };
            yield return new object[] { "axa", TestValue.Literal("a"), TestValue.Builder(0, 2), 0, 3, "axxax" };
            yield return new object[] { "axa", TestValue.Builder(0, 2), TestValue.Builder(0, 2), 0, 3, "axa" };

            yield return new object[] { "abab", TestValue.Builder(0, 3), TestValue.Literal("lb"), 0, 4, "lbb" };
            yield return new object[] { "abab", TestValue.Literal("b"), TestValue.Builder(0, 2), 0, 4, "aabaab" };
            yield return new object[] { "abab", TestValue.Builder(0, 3), TestValue.Builder(0, 2), 0, 4, "abb" };

            yield return new object[] { "a,a,a", TestValue.Builder(0, 2), TestValue.Literal("efhf"), 0, 5, "efhfefhfa" };
            yield return new object[] { "a,a,a", TestValue.Literal("a"), TestValue.Builder(0, 3), 0, 5, "a,a,a,a,a,a" };
            yield return new object[] { "a,a,a", TestValue.Builder(0, 2), TestValue.Builder(0, 3), 0, 5, "a,aa,aa" };

            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(6, 4), TestValue.Literal(""), 0, 16, "" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Literal("a"), TestValue.Builder(6, 7), 0, 16, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(6, 4), TestValue.Builder(6, 7), 0, 16, "aaaaaaaaaaaaaaaaaaaaaaaaaaaa" };

            yield return new object[] { "abababababababab", TestValue.Builder(1, 9), TestValue.Literal("adq"), 0, 16, "aadqababab" };
            yield return new object[] { "abababababababab", TestValue.Literal("b"), TestValue.Builder(4, 11), 0, 16, "aabababababaaabababababaaabababababaaabababababaaabababababaaabababababaaabababababaaabababababa" };
            yield return new object[] { "abababababababab", TestValue.Builder(1, 9), TestValue.Builder(4, 11), 0, 16, "aabababababaababab" };

            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(3, 6), TestValue.Literal("twfnonzpltx"), 0, 16, "twfnonzpltxtwfnonzpltxaaaa" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Literal("aa"), TestValue.Builder(7, 7), 0, 16, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(3, 6), TestValue.Builder(7, 7), 0, 16, "aaaaaaaaaaaaaaaaaa" };

            yield return new object[] { "abababababababab", TestValue.Builder(11, 2), TestValue.Literal("uopqiqa"), 0, 16, "auopqiqauopqiqauopqiqauopqiqauopqiqauopqiqauopqiqab" };
            yield return new object[] { "abababababababab", TestValue.Literal("ab"), TestValue.Builder(3, 12), 0, 16, "babababababababababababababababababababababababababababababababababababababababababababababababa" };
            yield return new object[] { "abababababababab", TestValue.Builder(11, 2), TestValue.Builder(3, 12), 0, 16, "ababababababababababababababababababababababababababababababababababababababababababab" };

            yield return new object[] { "a,a,a,a,a,a,a,a", TestValue.Builder(0, 4), TestValue.Literal("qvkkfre"), 0, 15, "qvkkfreqvkkfreqvkkfrea,a" };
            yield return new object[] { "a,a,a,a,a,a,a,a", TestValue.Literal("a"), TestValue.Builder(10, 3), 0, 15, "a,a,a,a,a,a,a,a,a,a,a,a,a,a,a,a" };
            yield return new object[] { "a,a,a,a,a,a,a,a", TestValue.Builder(0, 4), TestValue.Builder(10, 3), 0, 15, "a,aa,aa,aa,a" };

            yield return new object[] { "aaaabbbbccccddddaaaabbbbccccdddd", TestValue.Builder(16, 2), TestValue.Literal("sqq"), 0, 32, "sqqsqqbbbbccccddddsqqsqqbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccddddaaaabbbbccccdddd", TestValue.Literal("bbbb"), TestValue.Builder(24, 7), 0, 32, "aaaaccccdddccccddddaaaaccccdddccccdddd" };
            yield return new object[] { "aaaabbbbccccddddaaaabbbbccccdddd", TestValue.Builder(16, 2), TestValue.Builder(24, 7), 0, 32, "ccccdddccccdddbbbbccccddddccccdddccccdddbbbbccccdddd" };

            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(1, 8), TestValue.Literal("grcmqlunubrt"), 0, 30, "0grcmqlunubrt90grcmqlunubrt90grcmqlunubrt9" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Literal("0"), TestValue.Builder(17, 8), 0, 30, "789012341234567897890123412345678978901234123456789" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(1, 8), TestValue.Builder(17, 8), 0, 30, "078901234907890123490789012349" };

            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(24, 2), TestValue.Literal("vprcnrmed"), 0, 30, "0123vprcnrmed67890123vprcnrmed67890123vprcnrmed6789" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Literal("0"), TestValue.Builder(23, 2), 0, 30, "341234567893412345678934123456789" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(24, 2), TestValue.Builder(23, 2), 0, 30, "012334678901233467890123346789" };

            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(11, 4), TestValue.Literal("nnodfp"), 0, 30, "0nnodfp567890nnodfp567890nnodfp56789" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Literal("0"), TestValue.Builder(17, 8), 0, 30, "789012341234567897890123412345678978901234123456789" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(11, 4), TestValue.Builder(17, 8), 0, 30, "078901234567890789012345678907890123456789" };

            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(3, 2), TestValue.Literal("hras"), 0, 30, "012hras56789012hras56789012hras56789" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Literal("0"), TestValue.Builder(1, 19), 0, 30, "123456789012345678912345678912345678901234567891234567891234567890123456789123456789" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(3, 2), TestValue.Builder(1, 19), 0, 30, "012123456789012345678956789012123456789012345678956789012123456789012345678956789" };

            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(14, 8), TestValue.Literal("e"), 0, 30, "0123e23e23456789" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Literal("0"), TestValue.Builder(10, 10), 0, 30, "012345678912345678901234567891234567890123456789123456789" };
            yield return new object[] { "012345678901234567890123456789", TestValue.Builder(14, 8), TestValue.Builder(10, 10), 0, 30, "0123012345678923012345678923456789" };

            yield return new object[] { "xyxyxyxyxyxyxyxy", TestValue.Builder(0, 6), TestValue.Literal("ybkzztsmyzzx"), 0, 16, "ybkzztsmyzzxybkzztsmyzzxxyxy" };
            yield return new object[] { "xyxyxyxyxyxyxyxy", TestValue.Literal("xy"), TestValue.Builder(7, 2), 0, 16, "yxyxyxyxyxyxyxyx" };
            yield return new object[] { "xyxyxyxyxyxyxyxy", TestValue.Builder(0, 6), TestValue.Builder(7, 2), 0, 16, "yxyxxyxy" };

            yield return new object[] { "startstartstarttartcart55", TestValue.Builder(5, 12), TestValue.Literal("qvagbksp"), 0, 25, "startqvagbksprtcart55" };
            yield return new object[] { "startstartstarttartcart55", TestValue.Literal("art"), TestValue.Builder(16, 4), 0, 25, "startcstartcstartctartccartc55" };
            yield return new object[] { "startstartstarttartcart55", TestValue.Builder(5, 12), TestValue.Builder(16, 4), 0, 25, "startartcrtcart55" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(3, 9), TestValue.Literal("sbr"), 2, 3, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("a"), TestValue.Builder(9, 6), 2, 3, "aacccdddcccdddbbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(3, 9), TestValue.Builder(9, 6), 2, 3, "aaaabbbbccccdddd" };

            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(5, 10), TestValue.Literal("lvxevy"), 4, 4, "aaaabbbbccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Literal("bb"), TestValue.Builder(6, 9), 4, 4, "aaaabbccccdddbbccccdddccccdddd" };
            yield return new object[] { "aaaabbbbccccdddd", TestValue.Builder(5, 10), TestValue.Builder(6, 9), 4, 4, "aaaabbbbccccdddd" };

            yield return new object[] { "abababababab", TestValue.Builder(3, 6), TestValue.Literal("nwm"), 2, 8, "abanwmbab" };
            yield return new object[] { "abababababab", TestValue.Literal("ab"), TestValue.Builder(1, 4), 2, 8, "abbabababababababaab" };
            yield return new object[] { "abababababab", TestValue.Builder(3, 6), TestValue.Builder(1, 4), 2, 8, "ababababab" };

            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(7, 4), TestValue.Literal("hygkmnrwzl"), 2, 12, "aahygkmnrwzlhygkmnrwzlhygkmnrwzlaa" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Literal("a"), TestValue.Builder(3, 11), 2, 12, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" };
            yield return new object[] { "aaaaaaaaaaaaaaaa", TestValue.Builder(7, 4), TestValue.Builder(3, 11), 2, 12, "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" };

            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", TestValue.Builder(13, 8), TestValue.Literal(""), 0, 26, "abcdefghijklmvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", TestValue.Literal("abcdefghijklmnopqrstuvwxyz"), TestValue.Builder(10, 7), 0, 26, "klmnopq" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", TestValue.Builder(13, 8), TestValue.Builder(10, 7), 0, 26, "abcdefghijklmklmnopqvwxyz" };

            // Exercise pooled snapshot path (> CharStackBufferSize) for aliased oldValue.
            yield return new object[] { "a012345678901234567890123456789012345678901234567890123456789012345", TestValue.Builder(0, 65), TestValue.Literal("X"), 0, 66, "X45" };

            // Exercise pooled snapshot path (> CharStackBufferSize) for aliased newValue.
            yield return new object[] { "x012345678901234567890123456789012345678901234567890123456789012345", TestValue.Literal("x"), TestValue.Builder(1, 65), 0, 66, "01234567890123456789012345678901234567890123456789012345678901234" + "012345678901234567890123456789012345678901234567890123456789012345" };

            // Exercise pooled snapshot path (> CharStackBufferSize) when both values alias.
            yield return new object[] { "a012345678901234567890123456789012345678901234567890123456789012345", TestValue.Builder(0, 65), TestValue.Builder(1, 65), 0, 66, "0123456789012345678901234567890123456789012345678901234567890123445" };
        }

#nullable enable
        internal readonly record struct TestValue
        {
            public string? Value { get; init; }
            public int StartIndex { get; init; }
            public int Length { get; init; }
            public bool FromBuilder { get; init; }

            public ReadOnlySpan<char> GetSpan(MutableTextBuffer builder)
                => FromBuilder
                    ? builder.AsSpan(StartIndex, Length)
                    : Value.AsSpan();

            // Handy for comparing against StringBuilder without changing the test data
            //public ReadOnlySpan<char> GetSpan(StringBuilder builder)
            //    => FromBuilder
            //        ? builder.ToString(StartIndex, Length).AsSpan()
            //        : Value.AsSpan();

            internal static TestValue Literal(string value)
                => new() { Value = value };

            internal static TestValue Builder(int start, int length)
                => new() { FromBuilder = true, StartIndex = start, Length = length };
        }

#nullable restore

        [Theory]
        [MemberData(nameof(Replace_Overlapping_TestData))]
        internal void Replace_CharSpan_Overlapping(string value, TestValue oldValue, TestValue newValue, int startIndex, int count, string expected)
        {
            MutableTextBuffer builder;
            if (startIndex == 0 && count == value.Length)
            {
                builder = MutableTextBufferFactory(value);
                builder.Replace(oldValue.GetSpan(builder), newValue.GetSpan(builder));
                Assert.Equal(expected, builder.ToString());
            }
            builder = MutableTextBufferFactory(value);
            builder.Replace(oldValue.GetSpan(builder), newValue.GetSpan(builder), startIndex, count);
            Assert.Equal(expected, builder.ToString());
        }

        // J2N: Multiple chunks not supported
        //[Fact]
        //public void Replace_StringBuilderWithMultipleChunks()
        //{
        //    MutableTextBuffer builder = OpenStringBuilderTests.StringBuilderWithMultipleChunks();
        //    Replace(builder, "a", "b", builder.Length - 10, 10);
        //    Assert.Equal(new string('a', builder.Length - 10) + new string('b', 10), builder.ToString());
        //}

        [Fact]
        public void Replace_String_Large()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            builder.Replace("a", "b", builder.Length - 10, 10);
            Assert.Equal(new string('a', builder.Length - 10) + new string('b', 10), builder.ToString());
        }

        [Fact]
        public void Replace_CharSpan_Large()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            builder.Replace("a".AsSpan(), "b".AsSpan(), builder.Length - 10, 10);
            Assert.Equal(new string('a', builder.Length - 10) + new string('b', 10), builder.ToString());
        }

        // J2N: Multiple chunks not supported
        //[Fact]
        //public void Replace_StringBuilderWithMultipleChunks_WholeString()
        //{
        //    MutableTextBuffer builder = OpenStringBuilderTests.StringBuilderWithMultipleChunks();
        //    Replace(builder, builder.ToString(), "");
        //    Assert.Same(string.Empty, builder.ToString());
        //}

        [Fact]
        public void Replace_String_WholeString()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            builder.Replace(builder.ToString(), "");
            Assert.Same(string.Empty, builder.ToString());
        }

        [Fact]
        public void Replace_CharSpan_WholeString()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            builder.Replace(builder.ToString().AsSpan(), "".AsSpan());
            Assert.Same(string.Empty, builder.ToString());
        }

        // J2N: Multiple chunks not supported
        //[Fact]
        //public void Replace_StringBuilderWithMultipleChunks_LongString()
        //{
        //    MutableTextBuffer builder = OpenStringBuilderTests.StringBuilderWithMultipleChunks();
        //    Replace(builder, builder.ToString() + "b", "");
        //    Assert.Equal(OpenStringBuilderTests.s_chunkSplitSource, builder.ToString());
        //}

        [Fact]
        public void Replace_String_LongString()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            builder.Replace(builder.ToString() + "b", "");
            Assert.Equal(s_chunkSplitSource, builder.ToString());
        }

        [Fact]
        public void Replace_CharSpan_LongString()
        {
            MutableTextBuffer builder = MutableTextBufferFactory(s_chunkSplitSource);
            string findString = builder.ToString() + "b";
            builder.Replace(findString.AsSpan(), "".AsSpan());
            Assert.Equal(s_chunkSplitSource, builder.ToString());
        }

        [Fact]
        public void Replace_String_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentNullException>("oldValue", () => builder.Replace(null, "")); // Old value is null
            AssertExtensions.Throws<ArgumentNullException>("oldValue", () => builder.Replace(null, "a", 0, 0)); // Old value is null

            AssertExtensions.Throws<ArgumentException>("oldValue", () => builder.Replace("", "a")); // Old value is empty
            AssertExtensions.Throws<ArgumentException>("oldValue", () => builder.Replace("", "a", 0, 0)); // Old value is empty

            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Replace("o", "oo")); // New length > builder.MaxCapacity
            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Replace("o", "oo", 0, 5)); // New length > builder.MaxCapacity

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Replace("a", "b", -1, 0)); // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a", "b", 0, -1)); // Count < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Replace("a", "b", 6, 0)); // Count + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a", "b", 5, 1)); // Count + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a", "b", 4, 2)); // Count + start index > builder.Length
        }

        [Fact]
        public void Replace_CharSpan_Invalid()
        {
            var builder = MutableTextBufferFactory(0, 5);
            builder.Append("Hello");

            AssertExtensions.Throws<ArgumentException>("oldValue", () => builder.Replace("".AsSpan(), "a".AsSpan())); // Old value is empty
            AssertExtensions.Throws<ArgumentException>("oldValue", () => builder.Replace("".AsSpan(), "a".AsSpan(), 0, 0)); // Old value is empty

            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Replace("o".AsSpan(), "oo".AsSpan())); // New length > builder.MaxCapacity
            AssertExtensions.Throws<ArgumentOutOfRangeException>("requiredLength", () => builder.Replace("o".AsSpan(), "oo".AsSpan(), 0, 5)); // New length > builder.MaxCapacity

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Replace("a".AsSpan(), "b".AsSpan(), -1, 0)); // Start index < 0
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a".AsSpan(), "b".AsSpan(), 0, -1)); // Count < 0

            AssertExtensions.Throws<ArgumentOutOfRangeException>("startIndex", () => builder.Replace("a".AsSpan(), "b".AsSpan(), 6, 0)); // Count + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a".AsSpan(), "b".AsSpan(), 5, 1)); // Count + start index > builder.Length
            AssertExtensions.Throws<ArgumentOutOfRangeException>("count", () => builder.Replace("a".AsSpan(), "b".AsSpan(), 4, 2)); // Count + start index > builder.Length
        }

        public static IEnumerable<object[]> Replace_Int32_Int32_TestData()
        {
            yield return new object[] { "", 0, 0, "abc", "abc" };
            yield return new object[] { "abcdef", 0, 0, "X", "Xabcdef" };
            yield return new object[] { "abcdef", 6, 0, "X", "abcdefX" };
            yield return new object[] { "abcdef", 2, 0, "XYZ", "abXYZcdef" };

            yield return new object[] { "abcdef", 0, 2, "XY", "XYcdef" };
            yield return new object[] { "abcdef", 2, 2, "XY", "abXYef" };
            yield return new object[] { "abcdef", 4, 2, "XY", "abcdXY" };

            yield return new object[] { "abcdef", 2, 2, "WXYZ", "abWXYZef" };
            yield return new object[] { "abcdef", 2, 3, "Q", "abQf" };
            yield return new object[] { "abcdef", 2, 3, "", "abf" };

            // Count extends past end (Harmony/JDK semantics)
            yield return new object[] { "abcdef", 4, 100, "XYZ", "abcdXYZ" };
            yield return new object[] { "abcdef", 6, 100, "XYZ", "abcdefXYZ" };

            yield return new object[] { "0123456789", 3, 4, "abcdef", "012abcdef789" };
            yield return new object[] { "The quick brown fox", 4, 5, "slow", "The slow brown fox" };
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_TestData))]
        public void Replace_Int32_Int32_String(string value, int startIndex, int count, string newValue, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            builder.Replace(startIndex, count, newValue);

            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_TestData))]
        public void Replace_Int32_Int32_CharSpan(string value, int startIndex, int count, string newValue, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            builder.Replace(startIndex, count, newValue.AsSpan());

            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_TestData))]
        public void Replace_Int32_Int32_StringBuilder(string value, int startIndex, int count, string newValue, string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            builder.Replace(startIndex, count, new StringBuilder(newValue));

            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_TestData))]
        public void Replace_Int32_Int32_ICharSequence(string value, int startIndex, int count, string newValue, string expected)
        {
            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ICharSequence sequence = new SpannableCharSequence(newValue.AsMemory());
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ICharSequence sequence = new CopyableCharSequence(newValue.AsMemory());
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ICharSequence sequence = new SpanCopyableCharSequence(newValue.AsMemory());
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ICharSequence sequence = new SimpleCharSequence(newValue.AsMemory());
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }
        }

        public static IEnumerable<object[]> Replace_Int32_Int32_Invalid_TestData()
        {
            // value, capacity, maxCapacity, startIndex, count, newValue,
            // expectedExceptionType, expectedParamName

            // null replacement
            yield return new object[] { "Hello", 5, 100, 0, 0, null, typeof(ArgumentNullException), "newValue" };

            // invalid start index
            yield return new object[] { "Hello", 5, 100, -1, 0, "X", typeof(ArgumentOutOfRangeException), "startIndex" };
            yield return new object[] { "Hello", 5, 100, 6, 0, "X", typeof(ArgumentOutOfRangeException), "startIndex" };

            // invalid count
            yield return new object[] { "Hello", 5, 100, 0, -1, "X", typeof(ArgumentOutOfRangeException), "count" };

            // replacement would exceed MaxCapacity
            yield return new object[] { "Hello", 5, 5, 4, 1, "ABCDE", typeof(ArgumentOutOfRangeException), "requiredLength" };
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Invalid_TestData))]
        public void Replace_Int32_Int32_String_Invalid(
            string value,
            int capacity,
            int maxCapacity,
            int startIndex,
            int count,
            string newValue,
            Type exceptionType,
            string paramName)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(capacity, maxCapacity);
            builder.Append(value);

            AssertExtensions.Throws(exceptionType, paramName,
                () => builder.Replace(startIndex, count, newValue));
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Invalid_TestData))]
        public void Replace_Int32_Int32_CharSpan_Invalid(
            string value,
            int capacity,
            int maxCapacity,
            int startIndex,
            int count,
            string newValue,
            Type exceptionType,
            string paramName)
        {
            // Span overload cannot receive null.
            if (newValue is null)
                return;

            MutableTextBuffer builder = MutableTextBufferFactory(capacity, maxCapacity);
            builder.Append(value);

            AssertExtensions.Throws(exceptionType, paramName,
                () => builder.Replace(startIndex, count, newValue.AsSpan()));
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Invalid_TestData))]
        public void Replace_Int32_Int32_StringBuilder_Invalid(
            string value,
            int capacity,
            int maxCapacity,
            int startIndex,
            int count,
            string newValue,
            Type exceptionType,
            string paramName)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(capacity, maxCapacity);
            builder.Append(value);

            StringBuilder replacement = newValue is null ? null : new StringBuilder(newValue);

            AssertExtensions.Throws(exceptionType, paramName,
                () => builder.Replace(startIndex, count, replacement));
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Invalid_TestData))]
        public void Replace_Int32_Int32_ICharSequence_Invalid(
            string value,
            int capacity,
            int maxCapacity,
            int startIndex,
            int count,
            string newValue,
            Type exceptionType,
            string paramName)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(capacity, maxCapacity);
            builder.Append(value);

            ICharSequence replacement = newValue is null ? null : new SimpleCharSequence(newValue.AsMemory());

            AssertExtensions.Throws(exceptionType, paramName,
                () => builder.Replace(startIndex, count, replacement));
        }

        public static IEnumerable<object[]> Replace_Int32_Int32_Overlapping_TestData()
        {
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 3, 0, 5, "abcdefghijabcdenopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 3, 10, 5, "klmnodefghijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 3, 10, 3, "klmdefghijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 3, 0, 8, "abcdefghijabcdefghnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 3, 15, 8, "pqrstuvwdefghijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 8, 0, 2, "abcdefghijabstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 8, 20, 2, "uvijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 10, 5, 5, "fghijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 3, 2, 0, 10, "abcabcdefghijfghijklmnopqrstuvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 10, 10, 5, 10, "abcdefghijfghijklmnouvwxyz" };
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 5, 10, 10, 10, "abcdeklmnopqrstpqrstuvwxyz" };

            // Whole-buffer replacement (fast no-op)
            yield return new object[] { "abcdefghijklmnopqrstuvwxyz", 0, 26, 0, 26, "abcdefghijklmnopqrstuvwxyz" };

            // Empty spans (Overlaps() == false)
            yield return new object[] { "abcdef", 2, 3, 1, 0, "abf" };
            yield return new object[] { "abcdef", 0, 6, 2, 0, "" };
            yield return new object[] { "abcdef", 4, 2, 0, 0, "abcd" };
            yield return new object[] { "abcdef", 3, 0, 1, 0, "abcdef" };

            // Exercise array pool fallback path.
            yield return new object[] { "x012345678901234567890123456789012345678901234567890123456789012345", 66, 0, 1, 66, "x01234567890123456789012345678901234567890123456789012345678901234" + "012345678901234567890123456789012345678901234567890123456789012345" + "5" };
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Overlapping_TestData))]
        public void Replace_Int32_Int32_CharSpan_Overlapping(
            string value,
            int startIndex,
            int count,
            int sourceIndex,
            int sourceLength,
            string expected)
        {
            MutableTextBuffer builder = MutableTextBufferFactory(value);

            ReadOnlySpan<char> source = builder.AsSpan(sourceIndex, sourceLength);

            builder.Replace(startIndex, count, source);

            Assert.Equal(expected, builder.ToString());
        }

        [Theory]
        [MemberData(nameof(Replace_Int32_Int32_Overlapping_TestData))]
        public void Replace_Int32_Int32_ICharSequence_Overlapping(
            string value,
            int startIndex,
            int count,
            int sourceIndex,
            int sourceLength,
            string expected)
        {
            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SpannableCharSequence(memory);
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new CopyableCharSequence(memory);
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SpanCopyableCharSequence(memory);
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }

            {
                MutableTextBuffer builder = MutableTextBufferFactory(value);
                ReadOnlyMemory<char> memory = builder.AsMemory(sourceIndex, sourceLength);
                ICharSequence sequence = new SimpleCharSequence(memory);
                builder.Replace(startIndex, count, sequence);
                Assert.Equal(expected, builder.ToString());
            }
        }

        [Fact]
        public void Replace_Int32_Int32_ReadOnlySpan_Overlapping_SelfReplacement_NoOp()
        {
            MutableTextBuffer builder = MutableTextBufferFactory("abcdefghijklmnopqrstuvwxyz");

            builder.Replace(5, 10, builder.AsSpan(5, 10));

            Assert.Equal("abcdefghijklmnopqrstuvwxyz", builder.ToString());
        }

        [Fact]
        public void Replace_Int32_Int32_ICharSequence_Overlapping_SelfReplacement_NoOp()
        {
            MutableTextBuffer builder = MutableTextBufferFactory("abcdefghijklmnopqrstuvwxyz");

            builder.Replace(
                5,
                10,
                new SpannableCharSequence(builder.AsMemory(5, 10)));

            Assert.Equal("abcdefghijklmnopqrstuvwxyz", builder.ToString());
        }

        private sealed class ClearedTestAllocator : IArrayAllocator<char>
        {
            public bool GuaranteesClearedArrays => true;

            public char[] Allocate(int minimumLength)
            {
                return new char[minimumLength];
            }

            public void Return(char[] array)
            {
                Array.Clear(array, 0, array.Length);
            }
        }

        [Fact]
        public void Replace_Int32_Int32_CharSpan_Overlapping_StaleSpanCorruption_Regression()
        {
            // Arrange: Create a buffer with zero capacity slack to force MakeRoom reallocation
            var options = new MutableTextBufferTestOptions
            {
                Allocator = new ClearedTestAllocator()
            };

            // "SS-------------\0\0" scenario mapped to exact initial size. Note that we must call the
            // capacity overload and append the initial value later to get a specific buffer size.
            string initialValue = "SS-------------";
            MutableTextBuffer builder = MutableTextBufferFactory(capacity: initialValue.Length, options: options);
            builder.Append(initialValue);

            // Source region lies before startIndex, delta > 0 forces reallocation via MakeRoom
            // sourceOffset = 0, sourceLength = 2 ("SS"), startIndex = 15, count = 0
            ReadOnlySpan<char> source = builder.AsSpan(0, 2);

            // Act
            builder.Replace(15, 0, source);

            // Assert: Without the fix, the cleared allocator zeroes out the stale array,
            // resulting in corruption like "SS-------------\0\0".
            // With the fix (re-deriving the span after MakeRoom), it should be "SS-------------SS"
            Assert.Equal("SS-------------SS", builder.ToString());
        }
    }
}
