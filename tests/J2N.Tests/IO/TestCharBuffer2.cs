using NUnit.Framework;
using System;
using System.Reflection;
using StringBuffer = System.Text.StringBuilder;

namespace J2N.IO
{
    /// <summary>
    /// Tests from JDK/nio/BasicChar.java
    /// </summary>
    public class TestCharBuffer2 : BaseBufferTestCase
    {
        private static readonly char[] VALUES = {
            char.MinValue,
            unchecked((char) -1),
            (char) 0,
            (char) 1,
            char.MaxValue,
        };

        private static void relGet(CharBuffer b)
        {
            int n = b.Capacity;
            char v;
            for (int i = 0; i < n; i++)
                ck(b, (long)b.Get(), (long)((char)Ic(i)));
            b.Rewind();
        }

        private static void relGet(CharBuffer b, int start)
        {
            int n = b.Remaining;
            char v;
            for (int i = start; i < n; i++)
                ck(b, (long)b.Get(), (long)((char)Ic(i)));
            b.Rewind();
        }

        private static void absGet(CharBuffer b)
        {
            int n = b.Capacity;
            char v;
            for (int i = 0; i < n; i++)
                ck(b, (long)b.Get(), (long)((char)Ic(i)));
            b.Rewind();
        }

        private static void bulkGet(CharBuffer b)
        {
            int n = b.Capacity;
            char[] a = new char[n + 7];
            b.Get(a, 7, n);
            for (int i = 0; i < n; i++)
                ck(b, (long)a[i + 7], (long)((char)Ic(i)));
        }

        private static void relPut(CharBuffer b)
        {
            int n = b.Capacity;
            b.Clear();
            for (int i = 0; i < n; i++)
                b.Put((char)Ic(i));
            b.Flip();
        }

        private static void absPut(CharBuffer b)
        {
            int n = b.Capacity;
            b.Clear();
            for (int i = 0; i < n; i++)
                b.Put(i, (char)Ic(i));
            b.Limit = (n);
            b.Position = (0);
        }

        private static void bulkPutArray(CharBuffer b)
        {
            int n = b.Capacity;
            b.Clear();
            char[] a = new char[n + 7];
            for (int i = 0; i < n; i++)
                a[i + 7] = (char)Ic(i);
            b.Put(a, 7, n);
            b.Flip();
        }

        private static void bulkPutBuffer(CharBuffer b)
        {
            int n = b.Capacity;
            b.Clear();
            CharBuffer c = CharBuffer.Allocate(n + 7);
            c.Position = (7);
            for (int i = 0; i < n; i++)
                c.Put((char)Ic(i));
            c.Flip();
            c.Position = (7);
            b.Put(c);
            b.Flip();
        }

        //6231529
        private static void callReset(CharBuffer b)
        {
            b.Position = (0);
            b.Mark();

            b.Duplicate().Reset();
            b.AsReadOnlyBuffer().Reset();
        }



        // 6221101-6234263

        private static void putBuffer()
        {
            int cap = 10;

            // J2N: AllocateDirect not implemented

            //CharBuffer direct1 = ByteBuffer.AllocateDirect(cap).AsCharBuffer();
            CharBuffer nondirect1 = ByteBuffer.Allocate(cap).AsCharBuffer();
            //direct1.Put(nondirect1);

            //CharBuffer direct2 = ByteBuffer.AllocateDirect(cap).AsCharBuffer();
            CharBuffer nondirect2 = ByteBuffer.Allocate(cap).AsCharBuffer();
            //nondirect2.Put(direct2);

            //CharBuffer direct3 = ByteBuffer.AllocateDirect(cap).AsCharBuffer();
            //CharBuffer direct4 = ByteBuffer.AllocateDirect(cap).AsCharBuffer();
            //direct3.Put(direct4);

            CharBuffer nondirect3 = ByteBuffer.Allocate(cap).AsCharBuffer();
            CharBuffer nondirect4 = ByteBuffer.Allocate(cap).AsCharBuffer();
            nondirect3.Put(nondirect4);
        }
        private static void bulkPutString(CharBuffer b)
        {
            int n = b.Capacity;
            b.Clear();
            StringBuffer sb = new StringBuffer(n + 7);
            sb.Append("1234567");
            for (int i = 0; i < n; i++)
                sb.Append((char)Ic(i));
            b.Put(sb.ToString(), 7, (7 + n) - 7); // J2N: end - start
            b.Flip();
        }
        private static void checkSlice(CharBuffer b, CharBuffer slice)
        {
            ck(slice, 0, slice.Position);
            ck(slice, b.Remaining, slice.Limit);
            ck(slice, b.Remaining, slice.Capacity);
            //if (b.IsDirect != slice.IsDirect) // J2N: IsDirect not supported
            //    fail("Lost direction", slice);
            if (b.IsReadOnly != slice.IsReadOnly)
                fail("Lost read-only", slice);
        }

        private static void fail(String problem,
                                 CharBuffer xb, CharBuffer yb,
                                 char x, char y)
        {
            fail(problem + string.Format(": x={0} y={1}", x, y), xb, yb);
        }

        private static void tryCatch(Buffer b, Type ex, Action thunk)
        {
            bool caught = false;
            try
            {
                thunk();
            }
            catch (Exception x)
            {
                if (ex.GetTypeInfo().IsAssignableFrom(x.GetType()))
                {
                    caught = true;
                }
                else
                {
                    fail(x.Message + " not expected");
                }
            }
            if (!caught)
                fail(ex.Name + " not thrown", b);
        }

        private static void tryCatch(char[] t, Type ex, Action thunk)
        {
            tryCatch(CharBuffer.Wrap(t), ex, thunk);
        }

        public static void test(int level, CharBuffer b, bool direct)
        {

            Show(level, b);

            //if (direct != b.IsDirect) // J2N: IsDirect not implemented
            //    fail("Wrong direction", b);

            // Gets and puts

            relPut(b);
            relGet(b);
            absGet(b);
            bulkGet(b);

            absPut(b);
            relGet(b);
            absGet(b);
            bulkGet(b);

            bulkPutArray(b);
            relGet(b);

            bulkPutBuffer(b);
            relGet(b);



            bulkPutString(b);
            relGet(b);
            b.Position = (1);
            b.Limit = (7);
            ck(b, b.ToString().Equals("bcdefg"));

            // CharSequence ops

            b.Position = (2);
            ck(b, b[1], 'd');
            CharBuffer c = b.Subsequence(1, 4 - 1); // J2N: end - start
            ck(c, c.Capacity, b.Capacity);
            ck(c, c.Position, b.Position + 1);
            ck(c, c.Limit, b.Position + 4);
            ck(c, b.Subsequence(1, 4 - 1).ToString().Equals("def")); // J2N: end - start

            // 4938424
            b.Position = (4);
            ck(b, b[1], 'f');
            ck(b, b.Subsequence(1, 3 - 1).ToString().Equals("fg")); // J2N: end - start



            // Compact

            relPut(b);
            b.Position = (13);
            b.Compact();
            b.Flip();
            relGet(b, 13);

            // Exceptions

            relPut(b);
            b.Limit = (b.Capacity / 2);
            b.Position = (b.Limit);

            tryCatch(b, typeof(BufferUnderflowException), () =>
            {
                b.Get();
            });

            tryCatch(b, typeof(BufferOverflowException), () =>
            {
                b.Put((char)42);
            });

            // The index must be non-negative and lesss than the buffer's limit.
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                b.Get(b.Limit);
            });
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                b.Get(-1);
            });

            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                b.Put(b.Limit, (char)42);
            });

            tryCatch(b, typeof(InvalidMarkException), () =>
            {
                b.Position = (0);
                b.Mark();
                b.Compact();
                b.Reset();
            });

            // Values

            b.Clear();
            b.Put((char)0);
            b.Put(unchecked((char)-1));
            b.Put((char)1);
            b.Put(char.MaxValue);
            b.Put(char.MinValue);

            char v;
            b.Flip();
            ck(b, b.Get(), 0);
            ck(b, b.Get(), unchecked((char)-1));
            ck(b, b.Get(), 1);
            ck(b, b.Get(), char.MaxValue);
            ck(b, b.Get(), char.MinValue);


            // Comparison
            b.Rewind();
            CharBuffer b2 = CharBuffer.Allocate(b.Capacity);
            b2.Put(b);
            b2.Flip();
            b.Position = (2);
            b2.Position = (2);
            if (!b.Equals(b2))
            {
                for (int i = 2; i < b.Limit; i++)
                {
                    char x = b.Get(i);
                    char y = b2.Get(i);
                    if (x != y

                        )
                        output.WriteLine("[" + i + "] " + x + " != " + y);
                }
                fail("Identical buffers not equal", b, b2);
            }
            if (b.CompareTo(b2) != 0)
                fail("Comparison to identical buffer != 0", b, b2);

            b.Limit = (b.Limit + 1);
            b.Position = (b.Limit - 1);
            b.Put((char)99);
            b.Rewind();
            b2.Rewind();
            if (b.Equals(b2))
                fail("Non-identical buffers equal", b, b2);
            if (b.CompareTo(b2) <= 0)
                fail("Comparison to shorter buffer <= 0", b, b2);
            b.Limit = (b.Limit - 1);

            b.Put(2, (char)42);
            if (b.Equals(b2))
                fail("Non-identical buffers equal", b, b2);
            if (b.CompareTo(b2) <= 0)
                fail("Comparison to lesser buffer <= 0", b, b2);

            // Check equals and compareTo with interesting values
            foreach (char x in VALUES)
            {
                CharBuffer xb = CharBuffer.Wrap(new char[] { x });
                if (xb.CompareTo(xb) != 0)
                {
                    fail("compareTo not reflexive", xb, xb, x, x);
                }
                if (!xb.Equals(xb))
                {
                    fail("equals not reflexive", xb, xb, x, x);
                }
                foreach (char y in VALUES)
                {
                    CharBuffer yb = CharBuffer.Wrap(new char[] { y });
                    if (xb.CompareTo(yb) != -yb.CompareTo(xb))
                    {
                        fail("compareTo not anti-symmetric",
                             xb, yb, x, y);
                    }
                    if ((xb.CompareTo(yb) == 0) != xb.Equals(yb))
                    {
                        fail("compareTo inconsistent with equals",
                             xb, yb, x, y);
                    }
                    if (xb.CompareTo(yb) != x.CompareTo(y))
                    {
                        fail("Incorrect results for CharBuffer.compareTo",
                             xb, yb, x, y);
                    }
                    if (xb.Equals(yb) != ((x == y) /*|| ((x != x) && (y != y))*/))
                    {
                        fail("Incorrect results for CharBuffer.equals",
                             xb, yb, x, y);
                    }
                }
            }

            // Sub, dup

            relPut(b);
            relGet(b.Duplicate());
            b.Position = (13);
            relGet(b.Duplicate(), 13);
            relGet(b.Duplicate().Slice(), 13);
            relGet(b.Slice(), 13);
            relGet(b.Slice().Duplicate(), 13);

            // Slice

            b.Position = (5);
            CharBuffer sb = b.Slice();
            checkSlice(b, sb);
            b.Position = (0);
            CharBuffer sb2 = sb.Slice();
            checkSlice(sb, sb2);

            if (!sb.Equals(sb2))
                fail("Sliced slices do not match", sb, sb2);
            if ((sb.HasArray) && (sb.ArrayOffset != sb2.ArrayOffset))
                fail("Array offsets do not match: "
                     + sb.ArrayOffset + " != " + sb2.ArrayOffset, sb, sb2);


            // Read-only views

            b.Rewind();
            CharBuffer rb = b.AsReadOnlyBuffer();
            if (!b.Equals(rb))
                fail("Buffer not equal to read-only view", b, rb);
            Show(level + 1, rb);

            tryCatch(b, typeof(ReadOnlyBufferException), () =>
            {
                relPut(rb);
            });

            tryCatch(b, typeof(ReadOnlyBufferException), () =>
            {
                absPut(rb);
            });

            tryCatch(b, typeof(ReadOnlyBufferException), () =>
            {
                bulkPutArray(rb);
            });

            tryCatch(b, typeof(ReadOnlyBufferException), () =>
            {
                bulkPutBuffer(rb);
            });

            tryCatch(b, typeof(ReadOnlyBufferException), () =>
            {
                rb.Compact();
            });


            if (rb.GetType().Name.StartsWith("Heap", StringComparison.Ordinal))
            {

                tryCatch(b, typeof(ReadOnlyBufferException), () =>
                {
                    var _ = rb.Array;
                });

                tryCatch(b, typeof(ReadOnlyBufferException), () =>
                {
                    var _ = rb.ArrayOffset;
                });

                if (rb.HasArray)
                    fail("Read-only heap buffer's backing array is accessible",
                         rb);

            }

            // Bulk puts from read-only buffers

            b.Clear();
            rb.Rewind();
            b.Put(rb);

            relPut(b);                       // Required by testViews

        }

        private static void testStr()
        {
            String s = "abcdefghijklm";
            int start = 3;
            int end = 9;
            CharBuffer b = CharBuffer.Wrap(s, start, end - start); // J2N: end - start
            Show(0, b);
            ck(b, b.ToString().Equals(s.Substring(start, end - start))); // J2N: end - start
            ck(b, b.ToString().Equals("defghi"));
            ck(b, b.IsReadOnly);
            tryCatch(b, typeof(ReadOnlyBufferException), () =>
            {
                b.Put('x');
            });
            ck(b, start, b.Position);
            ck(b, end, b.Limit);
            ck(b, s.Length, b.Capacity);
            b.Position = (6);
            ck(b, b.Subsequence(0, 3 - 0).ToString().Equals("ghi")); // J2N: end - start

            // The index, relative to the position, must be non-negative and
            // smaller than remaining().
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                var _ = b[-1];
            });
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                var _ = b[b.Remaining];
            });

            // The index must be non-negative and less than the buffer's limit.
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                b.Get(b.Limit);
            });
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                b.Get(-1);
            });

            // The start must be non-negative and no larger than remaining().
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                b.Subsequence(-1, b.Remaining - -1); // J2N: end - start
            });
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                b.Subsequence(b.Remaining + 1, b.Remaining - (b.Remaining + 1)); // J2N: end - start
            });

            // The end must be no smaller than start and no larger than
            // remaining().
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                b.Subsequence(2, 1 - 2); // J2N: end - start
            });
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                b.Subsequence(0, (b.Remaining + 1) - 0); // J2N: end - start
            });

            // The offset must be non-negative and no larger than <array.length>.
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                CharBuffer.Wrap(s, -1, s.Length - -1); // J2N: end - start
            });
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                CharBuffer.Wrap(s, s.Length + 1, s.Length - (s.Length + 1)); // J2N: end - start
            });
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                CharBuffer.Wrap(s, 1, 0 - 1); // J2N: end - start
            });
            tryCatch(b, typeof(ArgumentOutOfRangeException), () =>
            {
                CharBuffer.Wrap(s, 0, (s.Length + 1) - 0); // J2N: end - start
            });
        }



        public static void test(char[] ba)
        {
            int offset = 47;
            int length = 900;
            CharBuffer b = CharBuffer.Wrap(ba, offset, length);
            Show(0, b);
            ck(b, b.Capacity, ba.Length);
            ck(b, b.Position, offset);
            ck(b, b.Limit, offset + length);

            // The offset must be non-negative and no larger than <array.length>.
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                CharBuffer.Wrap(ba, -1, ba.Length - -1); // J2N: end - start
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                CharBuffer.Wrap(ba, ba.Length + 1, ba.Length - (ba.Length + 1)); // J2N: end - start
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                CharBuffer.Wrap(ba, 0, -1 - 0); // J2N: end - start
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                CharBuffer.Wrap(ba, 0, (ba.Length + 1) - 0); // J2N: end - start
            });

            // A NullPointerException will be thrown if the array is null.
            tryCatch(ba, typeof(ArgumentNullException), () =>
            {
                CharBuffer.Wrap((char[])null, 0, 5 - 0); // J2N: end - start
            });
            tryCatch(ba, typeof(ArgumentNullException), () =>
            {
                CharBuffer.Wrap((char[])null);
            });
        }

        private static void TestAllocate()
        {
            // An IllegalArgumentException will be thrown for negative capacities.
            tryCatch((Buffer)null, typeof(ArgumentException), () =>
            {
                CharBuffer.Allocate(-1);
            });






        }

        [Test]
        public static void Test()
        {
            TestAllocate();
            test(0, CharBuffer.Allocate(7 * 1024), false);
            test(0, CharBuffer.Wrap(new char[7 * 1024], 0, 7 * 1024), false);
            test(new char[1024]);
            testStr();
            callReset(CharBuffer.Allocate(10));
            putBuffer();

        }

    }
}
