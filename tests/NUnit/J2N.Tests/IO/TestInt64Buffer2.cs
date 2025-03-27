﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace J2N.IO
{
    /// <summary>
    /// Tests from JDK/nio/BasicLong.java
    /// </summary>
    public class TestInt64Buffer2 : BaseBufferTestCase
    {
        private static readonly long[] VALUES = {
            long.MinValue,
            (long) -1,
            (long) 0,
            (long) 1,
            long.MaxValue,
        };


        private static void relGet(Int64Buffer b)
        {
            int n = b.Capacity;
            for (int i = 0; i < n; i++)
                ck(b, (long)b.Get(), (long)((long)Ic(i)));
            b.Rewind();
        }

        private static void relGet(Int64Buffer b, int start)
        {
            int n = b.Remaining;
            for (int i = start; i < n; i++)
                ck(b, (long)b.Get(), (long)((long)Ic(i)));
            b.Rewind();
        }

        private static void absGet(Int64Buffer b)
        {
            int n = b.Capacity;
            for (int i = 0; i < n; i++)
                ck(b, (long)b.Get(), (long)((long)Ic(i)));
            b.Rewind();
        }

        private static void bulkGet(Int64Buffer b)
        {
            int n = b.Capacity;
            long[] a = new long[n + 7];
            b.Get(a, 7, n);
            for (int i = 0; i < n; i++)
                ck(b, (long)a[i + 7], (long)((long)Ic(i)));
        }

        private static void bulkGetSpan(Int64Buffer b) // J2N specific
        {
            int n = b.Capacity;
            Span<long> a = new long[n + 7];
            b.Get(a.Slice(7, n));
            for (int i = 0; i < n; i++)
                ck(b, (long)a[i + 7], (long)((long)Ic(i)));
        }

        private static void relPut(Int64Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            for (int i = 0; i < n; i++)
                b.Put((long)Ic(i));
            b.Flip();
        }

        private static void absPut(Int64Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            for (int i = 0; i < n; i++)
                b.Put(i, (long)Ic(i));
            b.Limit = (n);
            b.Position = (0);
        }

        private static void bulkPutArray(Int64Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            long[] a = new long[n + 7];
            for (int i = 0; i < n; i++)
                a[i + 7] = (long)Ic(i);
            b.Put(a, 7, n);
            b.Flip();
        }

        private static void bulkPutSpan(Int64Buffer b) // J2N specific
        {
            int n = b.Capacity;
            b.Clear();
            Span<long> a = new long[n + 7];
            for (int i = 0; i < n; i++)
                a[i + 7] = (long)Ic(i);
            b.Put(a.Slice(7, n));
            b.Flip();
        }

        private static void bulkPutBuffer(Int64Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            Int64Buffer c = Int64Buffer.Allocate(n + 7);
            c.Position = (7);
            for (int i = 0; i < n; i++)
                c.Put((long)Ic(i));
            c.Flip();
            c.Position = (7);
            b.Put(c);
            b.Flip();
        }

        //6231529
        private static void callReset(Int64Buffer b)
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

            //LongBuffer direct1 = ByteBuffer.AllocateDirect(cap).AsLongBuffer();
            Int64Buffer nondirect1 = ByteBuffer.Allocate(cap).AsInt64Buffer();
            //direct1.Put(nondirect1);

            //LongBuffer direct2 = ByteBuffer.AllocateDirect(cap).AsLongBuffer();
            Int64Buffer nondirect2 = ByteBuffer.Allocate(cap).AsInt64Buffer();
            //nondirect2.Put(direct2);

            //LongBuffer direct3 = ByteBuffer.AllocateDirect(cap).AsLongBuffer();
            //LongBuffer direct4 = ByteBuffer.AllocateDirect(cap).AsLongBuffer();
            //direct3.Put(direct4);

            Int64Buffer nondirect3 = ByteBuffer.Allocate(cap).AsInt64Buffer();
            Int64Buffer nondirect4 = ByteBuffer.Allocate(cap).AsInt64Buffer();
            nondirect3.Put(nondirect4);
        }

        private static void checkSlice(Int64Buffer b, Int64Buffer slice)
        {
            ck(slice, 0, slice.Position);
            ck(slice, b.Remaining, slice.Limit);
            ck(slice, b.Remaining, slice.Capacity);
            //if (b.IsDirect != slice.IsDirect) // J2N: IsDirect not implemented
            //    fail("Lost direction", slice);
            if (b.IsReadOnly != slice.IsReadOnly)
                fail("Lost read-only", slice);
        }

        private static void fail(string problem,
                                 Int64Buffer xb, Int64Buffer yb,
                                 long x, long y)
        {
            fail(problem + string.Format(": x={0} y={1}", x, y), xb, yb);
        }

        private static void tryCatch(IO.Buffer b, Type ex, Action thunk)
        {
            bool caught = false;
            try
            {
                thunk();
            }
            catch (Exception x)
            {
                if (ex.IsAssignableFrom(x.GetType()))
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

        private static void tryCatch(long[] t, Type ex, Action thunk)
        {
            tryCatch(Int64Buffer.Wrap(t), ex, thunk);
        }

        public static void test(int level, Int64Buffer b, bool direct)
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

            relPut(b); // J2N specific
            relGet(b);
            absGet(b);
            bulkGetSpan(b);

            absPut(b); // J2N specific
            relGet(b);
            absGet(b);
            bulkGetSpan(b);

            bulkPutArray(b);
            relGet(b);

            bulkPutSpan(b); // J2N specific
            relGet(b);

            bulkPutBuffer(b);
            relGet(b);

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
                b.Put((long)42);
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
                b.Put(b.Limit, (long)42);
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
            b.Put((long)0);
            b.Put((long)-1);
            b.Put((long)1);
            b.Put(long.MaxValue);
            b.Put(long.MinValue);

            //long v; // J2N: Not used
            b.Flip();
            ck(b, b.Get(), 0);
            ck(b, b.Get(), (long)-1);
            ck(b, b.Get(), 1);
            ck(b, b.Get(), long.MaxValue);
            ck(b, b.Get(), long.MinValue);


            // Comparison
            b.Rewind();
            Int64Buffer b2 = Int64Buffer.Allocate(b.Capacity);
            b2.Put(b);
            b2.Flip();
            b.Position = (2);
            b2.Position = (2);
            if (!b.Equals(b2))
            {
                for (int i = 2; i < b.Limit; i++)
                {
                    long x = b.Get(i);
                    long y = b2.Get(i);
                    if (x != y)
                        output.WriteLine("[" + i + "] " + x + " != " + y);
                }
                fail("Identical buffers not equal", b, b2);
            }
            if (b.CompareTo(b2) != 0)
                fail("Comparison to identical buffer != 0", b, b2);

            b.Limit = (b.Limit + 1);
            b.Position = (b.Limit - 1);
            b.Put((long)99);
            b.Rewind();
            b2.Rewind();
            if (b.Equals(b2))
                fail("Non-identical buffers equal", b, b2);
            if (b.CompareTo(b2) <= 0)
                fail("Comparison to shorter buffer <= 0", b, b2);
            b.Limit = (b.Limit - 1);

            b.Put(2, (long)42);
            if (b.Equals(b2))
                fail("Non-identical buffers equal", b, b2);
            if (b.CompareTo(b2) <= 0)
                fail("Comparison to lesser buffer <= 0", b, b2);

            // Check equals and compareTo with interesting values
            foreach (long x in VALUES)
            {
                Int64Buffer xb = Int64Buffer.Wrap(new long[] { x });
                if (xb.CompareTo(xb) != 0)
                {
                    fail("compareTo not reflexive", xb, xb, x, x);
                }
                if (!xb.Equals(xb))
                {
                    fail("equals not reflexive", xb, xb, x, x);
                }
                foreach (long y in VALUES)
                {
                    Int64Buffer yb = Int64Buffer.Wrap(new long[] { y });
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
                    // from Long.compare(x, y)
                    if (xb.CompareTo(yb) != ((x < y) ? -1 : ((x == y) ? 0 : 1)))
                    {

                        fail("Incorrect results for LongBuffer.compareTo",
                             xb, yb, x, y);
                    }
                    if (xb.Equals(yb) != ((x == y) /*|| (x != x) && (y != y)*/))
                    {
                        fail("Incorrect results for LongBuffer.equals",
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
            Int64Buffer sb = b.Slice();
            checkSlice(b, sb);
            b.Position = (0);
            Int64Buffer sb2 = sb.Slice();
            checkSlice(sb, sb2);

            if (!sb.Equals(sb2))
                fail("Sliced slices do not match", sb, sb2);
            if ((sb.HasArray) && (sb.ArrayOffset != sb2.ArrayOffset))
                fail("Array offsets do not match: "
                     + sb.ArrayOffset + " != " + sb2.ArrayOffset, sb, sb2);


            // Read-only views

            b.Rewind();

            Int64Buffer rb = b.AsReadOnlyBuffer();
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
                bulkPutSpan(rb); // J2N specific
            });

            tryCatch(b, typeof(ReadOnlyBufferException), () =>
            {
                bulkPutBuffer(rb);
            });

            // put(LongBuffer) should not change source position
            Int64Buffer src = Int64Buffer.Allocate(1);
            tryCatch(b, typeof(ReadOnlyBufferException), () =>
            {
                rb.Put(src);
            });
            ck(src, src.Position, 0);

            tryCatch(b, typeof(ReadOnlyBufferException), () =>
            {
                rb.Compact();
            });


            if (rb.GetType().Name.StartsWith("Heap", StringComparison.Ordinal))
            {

                tryCatch(b, typeof(ReadOnlyBufferException), () =>
                {
                    var x = rb.Array;
                });

                tryCatch(b, typeof(ReadOnlyBufferException), () =>
                {
                    var x = rb.ArrayOffset;
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


        public static void test(long[] ba)
        {
            int offset = 47;
            int length = 900;
            Int64Buffer b = Int64Buffer.Wrap(ba, offset, length);
            Show(0, b);
            ck(b, b.Capacity, ba.Length);
            ck(b, b.Position, offset);
            ck(b, b.Limit, offset + length);

            // The offset must be non-negative and no larger than <array.length>.
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int64Buffer.Wrap(ba, -1, ba.Length);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int64Buffer.Wrap(ba, ba.Length + 1, ba.Length);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int64Buffer.Wrap(ba, 0, -1);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int64Buffer.Wrap(ba, 0, ba.Length + 1);
            });

            // A NullPointerException will be thrown if the array is null.
            tryCatch(ba, typeof(ArgumentNullException), () =>
            {
                Int64Buffer.Wrap((long[])null, 0, 5);
            });
            tryCatch(ba, typeof(ArgumentNullException), () =>
            {
                Int64Buffer.Wrap((long[])null);
            });
        }


        public static void TestAllocate()
        {
            // An IllegalArgumentException will be thrown for negative capacities.
            tryCatch((Buffer)null, typeof(ArgumentException), () =>
            {
                Int64Buffer.Allocate(-1);
            });
        }

        [Test]
        public static void Test()
        {
            TestAllocate();
            test(0, Int64Buffer.Allocate(7 * 1024), false);
            test(0, Int64Buffer.Wrap(new long[7 * 1024], 0, 7 * 1024), false);
            test(new long[1024]);

            callReset(Int64Buffer.Allocate(10));
            putBuffer();

        }
    }
}
