using NUnit.Framework;
using System;
using System.Reflection;

namespace J2N.IO
{
    /// <summary>
    /// Tests from JDK/nio/BasicInt.java
    /// </summary>
    public class TestInt16Buffer2 : BaseBufferTestCase
    {
        private static short[] VALUES = {
            short.MinValue,
            (short) -1,
            (short) 0,
            (short) 1,
            short.MaxValue,
        };

        private static void relGet(Int16Buffer b)
        {
            int n = b.Capacity;
            short v;
            for (int i = 0; i < n; i++)
                ck(b, (long)b.Get(), (long)((short)Ic(i)));
            b.Rewind();
        }

        private static void relGet(Int16Buffer b, int start)
        {
            int n = b.Remaining;
            short v;
            for (int i = start; i < n; i++)
                ck(b, (long)b.Get(), (long)((short)Ic(i)));
            b.Rewind();
        }

        private static void absGet(Int16Buffer b)
        {
            int n = b.Capacity;
            short v;
            for (int i = 0; i < n; i++)
                ck(b, (long)b.Get(), (long)((short)Ic(i)));
            b.Rewind();
        }

        private static void bulkGet(Int16Buffer b)
        {
            int n = b.Capacity;
            short[] a = new short[n + 7];
            b.Get(a, 7, n);
            for (int i = 0; i < n; i++)
                ck(b, (long)a[i + 7], (long)((short)Ic(i)));
        }

        private static void relPut(Int16Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            for (int i = 0; i < n; i++)
                b.Put((short)Ic(i));
            b.Flip();
        }

        private static void absPut(Int16Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            for (int i = 0; i < n; i++)
                b.Put(i, (short)Ic(i));
            b.Limit = (n);
            b.Position = (0);
        }

        private static void bulkPutArray(Int16Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            short[] a = new short[n + 7];
            for (int i = 0; i < n; i++)
                a[i + 7] = (short)Ic(i);
            b.Put(a, 7, n);
            b.Flip();
        }

        private static void bulkPutBuffer(Int16Buffer b)
        {
            int n = b.Capacity;
            b.Clear();
            Int16Buffer c = Int16Buffer.Allocate(n + 7);
            c.Position = (7);
            for (int i = 0; i < n; i++)
                c.Put((short)Ic(i));
            c.Flip();
            c.Position = (7);
            b.Put(c);
            b.Flip();
        }

        //6231529
        private static void callReset(Int16Buffer b)
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

            //Int16Buffer direct1 = ByteBuffer.AllocateDirect(cap).AsInt16Buffer();
            Int16Buffer nondirect1 = ByteBuffer.Allocate(cap).AsInt16Buffer();
            //direct1.Put(nondirect1);

            //Int16Buffer direct2 = ByteBuffer.AllocateDirect(cap).AsInt16Buffer();
            Int16Buffer nondirect2 = ByteBuffer.Allocate(cap).AsInt16Buffer();
            //nondirect2.Put(direct2);

            //Int16Buffer direct3 = ByteBuffer.AllocateDirect(cap).AsInt16Buffer();
            //Int16Buffer direct4 = ByteBuffer.AllocateDirect(cap).AsInt16Buffer();
            //direct3.Put(direct4);

            Int16Buffer nondirect3 = ByteBuffer.Allocate(cap).AsInt16Buffer();
            Int16Buffer nondirect4 = ByteBuffer.Allocate(cap).AsInt16Buffer();
            nondirect3.Put(nondirect4);
        }
        private static void checkSlice(Int16Buffer b, Int16Buffer slice)
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
                                 Int16Buffer xb, Int16Buffer yb,
                                 short x, short y)
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

        private static void tryCatch(short[] t, Type ex, Action thunk)
        {
            tryCatch(Int16Buffer.Wrap(t), ex, thunk);
        }

        public static void test(int level, Int16Buffer b, bool direct)
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
                b.Put((short)42);
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
                b.Put(b.Limit, (short)42);
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
            b.Put((short)0);
            b.Put((short)-1);
            b.Put((short)1);
            b.Put(short.MaxValue);
            b.Put(short.MinValue);

            short v;
            b.Flip();
            ck(b, b.Get(), 0);
            ck(b, b.Get(), (short)-1);
            ck(b, b.Get(), 1);
            ck(b, b.Get(), short.MaxValue);
            ck(b, b.Get(), short.MinValue);


            // Comparison
            b.Rewind();
            Int16Buffer b2 = Int16Buffer.Allocate(b.Capacity);
            b2.Put(b);
            b2.Flip();
            b.Position = (2);
            b2.Position = (2);
            if (!b.Equals(b2))
            {
                for (int i = 2; i < b.Limit; i++)
                {
                    short x = b.Get(i);
                    short y = b2.Get(i);
                    if (x != y)
                        output.WriteLine("[" + i + "] " + x + " != " + y);
                }
                fail("Identical buffers not equal", b, b2);
            }
            if (b.CompareTo(b2) != 0)
                fail("Comparison to identical buffer != 0", b, b2);

            b.Limit = (b.Limit + 1);
            b.Position = (b.Limit - 1);
            b.Put((short)99);
            b.Rewind();
            b2.Rewind();
            if (b.Equals(b2))
                fail("Non-identical buffers equal", b, b2);
            if (b.CompareTo(b2) <= 0)
                fail("Comparison to shorter buffer <= 0", b, b2);
            b.Limit = (b.Limit - 1);

            b.Put(2, (short)42);
            if (b.Equals(b2))
                fail("Non-identical buffers equal", b, b2);
            if (b.CompareTo(b2) <= 0)
                fail("Comparison to lesser buffer <= 0", b, b2);

            // Check equals and compareTo with interesting values
            foreach (short x in VALUES)
            {
                Int16Buffer xb = Int16Buffer.Wrap(new short[] { x });
                if (xb.CompareTo(xb) != 0)
                {
                    fail("compareTo not reflexive", xb, xb, x, x);
                }
                if (!xb.Equals(xb))
                {
                    fail("equals not reflexive", xb, xb, x, x);
                }
                foreach (short y in VALUES)
                {
                    Int16Buffer yb = Int16Buffer.Wrap(new short[] { y });
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
                        fail("Incorrect results for Int16Buffer.compareTo",
                             xb, yb, x, y);
                    }
                    if (xb.Equals(yb) != ((x == y) /*|| ((x != x) && (y != y))*/))
                    {
                        fail("Incorrect results for Int16Buffer.equals",
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
            Int16Buffer sb = b.Slice();
            checkSlice(b, sb);
            b.Position = (0);
            Int16Buffer sb2 = sb.Slice();
            checkSlice(sb, sb2);

            if (!sb.Equals(sb2))
                fail("Sliced slices do not match", sb, sb2);
            if ((sb.HasArray) && (sb.ArrayOffset != sb2.ArrayOffset))
                fail("Array offsets do not match: "
                     + sb.ArrayOffset + " != " + sb2.ArrayOffset, sb, sb2);

            // Read-only views

            b.Rewind();
            Int16Buffer rb = b.AsReadOnlyBuffer();
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

        public static void test(short[] ba)
        {
            int offset = 47;
            int length = 900;
            Int16Buffer b = Int16Buffer.Wrap(ba, offset, length);
            Show(0, b);
            ck(b, b.Capacity, ba.Length);
            ck(b, b.Position, offset);
            ck(b, b.Limit, offset + length);

            // The offset must be non-negative and no larger than <array.Length>.
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int16Buffer.Wrap(ba, -1, ba.Length);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int16Buffer.Wrap(ba, ba.Length + 1, ba.Length);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int16Buffer.Wrap(ba, 0, -1);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                Int16Buffer.Wrap(ba, 0, ba.Length + 1);
            });

            // A NullPointerException will be thrown if the array is null.
            tryCatch(ba, typeof(ArgumentNullException), () =>
            {
                Int16Buffer.Wrap((short[])null, 0, 5);
            });
            tryCatch(ba, typeof(ArgumentNullException), () =>
            {
                Int16Buffer.Wrap((short[])null);
            });
        }

        private static void testAllocate()
        {
            // An IllegalArgumentException will be thrown for negative capacities.
            tryCatch((Buffer)null, typeof(ArgumentException), () =>
            {
                Int16Buffer.Allocate(-1);
            });

        }

        [Test]
        public static void Test()
        {
            testAllocate();
            test(0, Int16Buffer.Allocate(7 * 1024), false);
            test(0, Int16Buffer.Wrap(new short[7 * 1024], 0, 7 * 1024), false);
            test(new short[1024]);
            callReset(Int16Buffer.Allocate(10));
            putBuffer();
        }

    }
}
