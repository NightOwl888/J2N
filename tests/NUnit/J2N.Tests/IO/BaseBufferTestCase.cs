﻿using NUnit.Framework;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace J2N.IO
{
    /// <summary>
    /// Base class for tests from JDK/nio/Basic.java
    /// </summary>
    public class BaseBufferTestCase : TestCase
    {
        internal static TextWriter output =
#if DEBUG
            Console.Out;
#else
            new NullWriter();
#endif

        internal static long Ic(int i)
        {
            int j = i % 54;
            return j + 'a' + ((j > 26) ? 128 : 0);
        }

        internal static string ToString(IO.Buffer b)
        {
            return (b.GetType().Name
                    + "[pos=" + b.Position
                    + " lim=" + b.Limit
                    + " cap=" + b.Capacity
                    + "]");
        }

        internal static void Show(int level, IO.Buffer b)
        {
            for (int i = 0; i < level; i++)
                output.Write("  ");
            output.WriteLine(ToString(b) + " " + b.GetHashCode().ToString("x4", CultureInfo.InvariantCulture));
        }

        //static void fail(string s)
        //{
        //    Assert.Fail(s);
        //}

        internal static void fail(string s, IO.Buffer b)
        {
            fail(s + ": " + ToString(b));
        }

        internal static void fail(String s, IO.Buffer b, IO.Buffer b2)
        {
            fail(s + ": "
                                       + ToString(b) + ", " + ToString(b2));
        }

        internal static void fail(IO.Buffer b,
                         string expected, char expectedChar,
                         string got, char gotChar)
        {
            if (b is ByteBuffer)
            {
                ByteBuffer bb = (ByteBuffer)b;
                int n = Math.Min(16, bb.Limit);
                for (int i = 0; i < n; i++)
                    output.Write(" " + (bb.Get(i) & 0xff).ToString("x4", CultureInfo.InvariantCulture));
                output.WriteLine();
            }
            if (b is CharBuffer)
            {
                CharBuffer bb = (CharBuffer)b;
                int n = Math.Min(16, bb.Limit);
                for (int i = 0; i < n; i++)
                    output.Write(" " + (bb.Get(i) & 0xffff).ToString("x4", CultureInfo.InvariantCulture));
                output.WriteLine();
            }
            Assert.Fail(ToString(b)
                                       + ": Expected '" + expectedChar + "'=0x"
                                       + expected
                                       + ", got '" + gotChar + "'=0x"
                                       + got);
        }

        internal static void fail(IO.Buffer b, long expected, long got)
        {
            fail(b,
                 (expected).ToString("x4", CultureInfo.InvariantCulture), (char)expected,
                 (got).ToString("x4", CultureInfo.InvariantCulture), (char)got);
        }

        internal static void ck(IO.Buffer b, bool cond)
        {
            if (!cond)
                fail("Condition failed", b);
        }

        internal static void ck(IO.Buffer b, long got, long expected)
        {
            if (expected != got)
                fail(b, expected, got);
        }

        internal static void ck(IO.Buffer b, float got, float expected)
        {
            if (expected != got)
                fail(b,
                     expected.ToString("0.0##########", CultureInfo.InvariantCulture), (char)expected,
                     got.ToString("0.0##########", CultureInfo.InvariantCulture), (char)got);
        }

        internal static void ck(IO.Buffer b, double got, double expected)
        {
            if (expected != got)
                fail(b,
                     expected.ToString("0.0##########", CultureInfo.InvariantCulture), (char)expected,
                     got.ToString("0.0##########", CultureInfo.InvariantCulture), (char)got);
        }

        /// <summary>
        /// A simple writer that implements the null object pattern
        /// that we can swap in when verbosity is turned off.
        /// </summary>
        private class NullWriter : TextWriter
        {
            public override Encoding Encoding
            {
                get
                {
                    return Encoding.UTF8;
                }
            }

            public override void Write(char value)
            {

            }
        }
    }
}
