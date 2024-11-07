﻿using NUnit.Framework;
using System;
using System.Reflection;

namespace J2N.IO
{
    /// <summary>
    /// Tests from JDK/nio/BasicInt.java
    /// </summary>
    public class TestInt32Buffer2 : BaseBufferTestCase
    {

        private static readonly int[] VALUES = {
            int.MinValue,
            (int) -1,
            (int) 0,
            (int) 1,
            int.MaxValue,
        };

        private static void relGet(Int32Buffer b)
        {
            int n = b.Capacity;
            int v;
            for (int i = 0; i < n; i++)
                ck(b, (long)b.Get(), (long)((int)Ic(i)));
            b.Rewind();
        }

        private static void relGet(Int32Buffer b, int start)
        {
            int n = b.Remaining;
            int v;
            for (int i = start; i < n; i++)
                ck(b, (long)b.Get(), (long)((int)Ic(i)));
            b.Rewind();
        }

        private static void absGet(Int32Buffer b)
        {
            int n = b.Capacity;
            int v;
            for (int i = 0; i < n; i++)
                ck(b, (long)b.Get(), (long)((int)Ic(i)));
            b.Rewind();
        }

        private static void bulkGet(Int32Buffer b)
        {
            int n = b.Capacity;
            int[] a = new int[n + 7];
            b.Get(a, 7, n);
            for (int i = 0; i < n; i++)
                ck(b, (long)a[i + 7], (long)((int)Ic(i)));
        }

        private static void relPut(Int32Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            for (int i = 0; i < n; i++)
                b.Put((int)Ic(i));
            b.Flip();
        }

        private static void absPut(Int32Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            for (int i = 0; i < n; i++)
                b.Put(i, (int)Ic(i));
            b.Limit = (n);
            b.Position = (0);
        }

        private static void bulkPutArray(Int32Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            int[] a = new int[n + 7];
            for (int i = 0; i < n; i++)
                a[i + 7] = (int)Ic(i);
            b.Put(a, 7, n);
            b.Flip();
        }

        private static void bulkPutBuffer(Int32Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            Int32Buffer c = Int32Buffer.Allocate(n + 7);
            c.Position = (7);
            for (int i = 0; i < n; i++)
                c.Put((int)Ic(i));
            c.Flip();
            c.Position = (7);
            b.Put(c);
            b.Flip();
        }

        //6231529
        private static void callReset(Int32Buffer b)
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

            // J2N: AllocateDirect not supported

            //Int32Buffer direct1 = ByteBuffer.AllocateDirect(cap).AsInt32Buffer();
            Int32Buffer nondirect1 = ByteBuffer.Allocate(cap).AsInt32Buffer();
            //direct1.Put(nondirect1);

            //Int32Buffer direct2 = ByteBuffer.AllocateDirect(cap).AsInt32Buffer();
            Int32Buffer nondirect2 = ByteBuffer.Allocate(cap).AsInt32Buffer();
            //nondirect2.Put(direct2);

            //Int32Buffer direct3 = ByteBuffer.AllocateDirect(cap).AsInt32Buffer();
            //Int32Buffer direct4 = ByteBuffer.AllocateDirect(cap).AsInt32Buffer();
            //direct3.Put(direct4);

            Int32Buffer nondirect3 = ByteBuffer.Allocate(cap).AsInt32Buffer();
            Int32Buffer nondirect4 = ByteBuffer.Allocate(cap).AsInt32Buffer();
            nondirect3.Put(nondirect4);
        }

        private static void checkSlice(Int32Buffer b, Int32Buffer slice)
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
                                 Int32Buffer xb, Int32Buffer yb,
                                 int x, int y)
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

        private static void tryCatch(int[] t, Type ex, Action thunk)
        {
            tryCatch(Int32Buffer.Wrap(t), ex, thunk);
        }

        public static void test(int level, Int32Buffer b, bool direct)
        {

            Show(level, b);

            //if (direct != b.IsDirect) // J2N: IsDirect not supported
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
                b.Put((int)42);
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
                b.Put(b.Limit, (int)42);
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
            b.Put((int)0);
            b.Put((int)-1);
            b.Put((int)1);
            b.Put(int.MaxValue);
            b.Put(int.MinValue);


            int v;
            b.Flip();
            ck(b, b.Get(), 0);
            ck(b, b.Get(), (int)-1);
            ck(b, b.Get(), 1);
            ck(b, b.Get(), int.MaxValue);
            ck(b, b.Get(), int.MinValue);


            // Comparison
            b.Rewind();
            Int32Buffer b2 = Int32Buffer.Allocate(b.Capacity);
            b2.Put(b);
            b2.Flip();
            b.Position = (2);
            b2.Position = (2);
            if (!b.Equals(b2))
            {
                for (int i = 2; i < b.Limit; i++)
                {
                    int x = b.Get(i);
                    int y = b2.Get(i);
                    if (x != y)
                        output.WriteLine("[" + i + "] " + x + " != " + y);
                }
                fail("Identical buffers not equal", b, b2);
            }
            if (b.CompareTo(b2) != 0)
                fail("Comparison to identical buffer != 0", b, b2);

            b.Limit = (b.Limit + 1);
            b.Position = (b.Limit - 1);
            b.Put((int)99);
            b.Rewind();
            b2.Rewind();
            if (b.Equals(b2))
                fail("Non-identical buffers equal", b, b2);
            if (b.CompareTo(b2) <= 0)
                fail("Comparison to shorter buffer <= 0", b, b2);
            b.Limit = (b.Limit - 1);

            b.Put(2, (int)42);
            if (b.Equals(b2))
                fail("Non-identical buffers equal", b, b2);
            if (b.CompareTo(b2) <= 0)
                fail("Comparison to lesser buffer <= 0", b, b2);

            // Check equals and compareTo with interesting values
            foreach (int x in VALUES)
            {
                Int32Buffer xb = Int32Buffer.Wrap(new int[] { x });
                if (xb.CompareTo(xb) != 0)
                {
                    fail("compareTo not reflexive", xb, xb, x, x);
                }
                if (!xb.Equals(xb))
                {
                    fail("equals not reflexive", xb, xb, x, x);
                }
                foreach (int y in VALUES)
                {
                    Int32Buffer yb = Int32Buffer.Wrap(new int[] { y });
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
                        fail("Incorrect results for Int32Buffer.compareTo",
                             xb, yb, x, y);
                    }
                    if (xb.Equals(yb) != ((x == y) /*|| ((x != x) && (y != y))*/))
                    {
                        fail("Incorrect results for Int32Buffer.equals",
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
            Int32Buffer sb = b.Slice();
            checkSlice(b, sb);
            b.Position = (0);
            Int32Buffer sb2 = sb.Slice();
            checkSlice(sb, sb2);

            if (!sb.Equals(sb2))
                fail("Sliced slices do not match", sb, sb2);
            if ((sb.HasArray) && (sb.ArrayOffset != sb2.ArrayOffset))
                fail("Array offsets do not match: "
                     + sb.ArrayOffset + " != " + sb2.ArrayOffset, sb, sb2);

            // Read-only views

            b.Rewind();
            Int32Buffer rb = b.AsReadOnlyBuffer();
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

            if (rb.GetType().Name.StartsWith("Heap"))
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
        public static void test(int[] ba)
        {
            int offset = 47;
            int length = 900;
            Int32Buffer b = Int32Buffer.Wrap(ba, offset, length);
            Show(0, b);
            ck(b, b.Capacity, ba.Length);
            ck(b, b.Position, offset);
            ck(b, b.Limit, offset + length);

            // The offset must be non-negative and no larger than <array.length>.
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int32Buffer.Wrap(ba, -1, ba.Length);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int32Buffer.Wrap(ba, ba.Length + 1, ba.Length);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int32Buffer.Wrap(ba, 0, -1);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int32Buffer.Wrap(ba, 0, ba.Length + 1);
            });

            // A NullPointerException will be thrown if the array is null.
            tryCatch(ba, typeof(ArgumentNullException), () =>
            {
                Int32Buffer.Wrap((int[])null, 0, 5);
            });
            tryCatch(ba, typeof(ArgumentNullException), () =>
            {
                Int32Buffer.Wrap((int[])null);
            });
        }

        private static void TestAllocate()
        {
            // An IllegalArgumentException will be thrown for negative capacities.
            tryCatch((Buffer)null, typeof(ArgumentException), () =>
            {
                Int32Buffer.Allocate(-1);
            });
        }

        [Test]
        public static void Test()
        {
            TestAllocate();
            test(0, Int32Buffer.Allocate(7 * 1024), false);
            test(0, Int32Buffer.Wrap(new int[7 * 1024], 0, 7 * 1024), false);
            test(new int[1024]);
            callReset(Int32Buffer.Allocate(10));
            putBuffer();
        }

    }
}
