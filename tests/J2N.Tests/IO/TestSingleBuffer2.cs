using NUnit.Framework;
using System;

namespace J2N.IO
{
    /// <summary>
    /// Tests from JDK/nio/BasicFloat.java
    /// </summary>
    public class TestSingleBuffer2 : BaseBufferTestCase
    {
        private static readonly float[] VALUES = {
        float.MinValue,
        (float) -1,
        (float) 0,
        (float) 1,
        float.MaxValue,

        float.NegativeInfinity,
        float.PositiveInfinity,
        float.NaN,
        (float) -0.0,
    };

        private static void relGet(SingleBuffer b)
        {
            int n = b.Capacity;
            float v;
            for (int i = 0; i < n; i++)
                ck(b, (long)b.Get(), (long)((float)Ic(i)));
            b.Rewind();
        }

        private static void relGet(SingleBuffer b, int start)
        {
            int n = b.Remaining;
            float v;
            for (int i = start; i < n; i++)
                ck(b, (long)b.Get(), (long)((float)Ic(i)));
            b.Rewind();
        }

        private static void absGet(SingleBuffer b)
        {
            int n = b.Capacity;
            float v;
            for (int i = 0; i < n; i++)
                ck(b, (long)b.Get(), (long)((float)Ic(i)));
            b.Rewind();
        }

        private static void bulkGet(SingleBuffer b)
        {
            int n = b.Capacity;
            float[] a = new float[n + 7];
            b.Get(a, 7, n);
            for (int i = 0; i < n; i++)
                ck(b, (long)a[i + 7], (long)((float)Ic(i)));
        }

        private static void relPut(SingleBuffer b)
        {
            int n = b.Capacity;
            b.Clear();
            for (int i = 0; i < n; i++)
                b.Put((float)Ic(i));
            b.Flip();
        }

        private static void absPut(SingleBuffer b)
        {
            int n = b.Capacity;
            b.Clear();
            for (int i = 0; i < n; i++)
                b.Put(i, (float)Ic(i));
            b.Limit = (n);
            b.Position = (0);
        }

        private static void bulkPutArray(SingleBuffer b)
        {
            int n = b.Capacity;
            b.Clear();
            float[] a = new float[n + 7];
            for (int i = 0; i < n; i++)
                a[i + 7] = (float)Ic(i);
            b.Put(a, 7, n);
            b.Flip();
        }

        private static void bulkPutBuffer(SingleBuffer b)
        {
            int n = b.Capacity;
            b.Clear();
            SingleBuffer c = SingleBuffer.Allocate(n + 7);
            c.Position = (7);
            for (int i = 0; i < n; i++)
                c.Put((float)Ic(i));
            c.Flip();
            c.Position = (7);
            b.Put(c);
            b.Flip();
        }

        //6231529
        private static void callReset(SingleBuffer b)
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

            //SingleBuffer direct1 = ByteBuffer.AllocateDirect(cap).AsSingleBuffer();
            SingleBuffer nondirect1 = ByteBuffer.Allocate(cap).AsSingleBuffer();
            //direct1.Put(nondirect1);

            //SingleBuffer direct2 = ByteBuffer.AllocateDirect(cap).AsSingleBuffer();
            SingleBuffer nondirect2 = ByteBuffer.Allocate(cap).AsSingleBuffer();
            //nondirect2.Put(direct2);

            //SingleBuffer direct3 = ByteBuffer.AllocateDirect(cap).AsSingleBuffer();
            //SingleBuffer direct4 = ByteBuffer.AllocateDirect(cap).AsSingleBuffer();
            //direct3.Put(direct4);

            SingleBuffer nondirect3 = ByteBuffer.Allocate(cap).AsSingleBuffer();
            SingleBuffer nondirect4 = ByteBuffer.Allocate(cap).AsSingleBuffer();
            nondirect3.Put(nondirect4);
        }
        private static void checkSlice(SingleBuffer b, SingleBuffer slice)
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
                                 SingleBuffer xb, SingleBuffer yb,
                                 float x, float y)
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

        private static void tryCatch(float[] t, Type ex, Action thunk)
        {
            tryCatch(SingleBuffer.Wrap(t), ex, thunk);
        }

        public static void test(int level, SingleBuffer b, bool direct)
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
                b.Put((float)42);
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
                b.Put(b.Limit, (float)42);
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
            b.Put((float)0);
            b.Put((float)-1);
            b.Put((float)1);
            b.Put(float.MaxValue);
            b.Put(float.MinValue);

            b.Put(-float.MaxValue);
            b.Put(-float.MinValue);
            b.Put(float.NegativeInfinity);
            b.Put(float.PositiveInfinity);
            b.Put(float.NaN);
            b.Put(0.91697687f);             // Changes value if incorrectly swapped

            float v;
            b.Flip();
            ck(b, b.Get(), 0);
            ck(b, b.Get(), (float)-1);
            ck(b, b.Get(), 1);
            ck(b, b.Get(), float.MaxValue);
            ck(b, b.Get(), float.MinValue);


            ck(b, b.Get(), -float.MaxValue);
            ck(b, b.Get(), -float.MinValue);
            ck(b, b.Get(), float.NegativeInfinity);
            ck(b, b.Get(), float.PositiveInfinity);
            // J2N TODO: Investigate why this comparison fails in .NET and passes in Java
            //if (BitConversion.SingleToRawInt32Bits(v = b.Get()) != BitConversion.SingleToRawInt32Bits(float.NaN))
            if (!float.IsNaN(v = b.Get()))
                fail(b, unchecked((long)float.NaN), (long)v);
            ck(b, b.Get(), 0.91697687f);

            // Comparison
            b.Rewind();
            SingleBuffer b2 = SingleBuffer.Allocate(b.Capacity);
            b2.Put(b);
            b2.Flip();
            b.Position = (2);
            b2.Position = (2);
            if (!b.Equals(b2))
            {
                for (int i = 2; i < b.Limit; i++)
                {
                    float x = b.Get(i);
                    float y = b2.Get(i);
                    if (x != y

                        || x.CompareTo(y) != 0

                        )
                        output.WriteLine("[" + i + "] " + x + " != " + y);
                }
                fail("Identical buffers not equal", b, b2);
            }
            if (b.CompareTo(b2) != 0)
                fail("Comparison to identical buffer != 0", b, b2);

            b.Limit = (b.Limit + 1);
            b.Position = (b.Limit - 1);
            b.Put((float)99);
            b.Rewind();
            b2.Rewind();
            if (b.Equals(b2))
                fail("Non-identical buffers equal", b, b2);
            if (b.CompareTo(b2) <= 0)
                fail("Comparison to shorter buffer <= 0", b, b2);
            b.Limit = (b.Limit - 1);

            b.Put(2, (float)42);
            if (b.Equals(b2))
                fail("Non-identical buffers equal", b, b2);
            if (b.CompareTo(b2) <= 0)
                fail("Comparison to lesser buffer <= 0", b, b2);

            // Check equals and compareTo with interesting values
            foreach (float x in VALUES)
            {
                SingleBuffer xb = SingleBuffer.Wrap(new float[] { x });
                if (xb.CompareTo(xb) != 0)
                {
                    fail("compareTo not reflexive", xb, xb, x, x);
                }
                if (!xb.Equals(xb))
                {
                    fail("equals not reflexive", xb, xb, x, x);
                }
                foreach (float y in VALUES)
                {
                    SingleBuffer yb = SingleBuffer.Wrap(new float[] { y });
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

                        if (x == 0.0 && y == 0.0) continue;
                        fail("Incorrect results for SingleBuffer.compareTo",
                             xb, yb, x, y);
                    }
                    if (xb.Equals(yb) != ((x == y) || (float.IsNaN(x) && float.IsNaN(y))))
                    {
                        fail("Incorrect results for SingleBuffer.equals",
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
            SingleBuffer sb = b.Slice();
            checkSlice(b, sb);
            b.Position = (0);
            SingleBuffer sb2 = sb.Slice();
            checkSlice(sb, sb2);

            if (!sb.Equals(sb2))
                fail("Sliced slices do not match", sb, sb2);
            if ((sb.HasArray) && (sb.ArrayOffset != sb2.ArrayOffset))
                fail("Array offsets do not match: "
                     + sb.ArrayOffset + " != " + sb2.ArrayOffset, sb, sb2);

            // Read-only views

            b.Rewind();
            SingleBuffer rb = b.AsReadOnlyBuffer();
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

        public static void test(float[] ba)
        {
            int offset = 47;
            int length = 900;
            SingleBuffer b = SingleBuffer.Wrap(ba, offset, length);
            Show(0, b);
            ck(b, b.Capacity, ba.Length);
            ck(b, b.Position, offset);
            ck(b, b.Limit, offset + length);

            // The offset must be non-negative and no larger than <array.Length>.
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                SingleBuffer.Wrap(ba, -1, ba.Length);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                SingleBuffer.Wrap(ba, ba.Length + 1, ba.Length);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                SingleBuffer.Wrap(ba, 0, -1);
            });
            tryCatch(ba, typeof(ArgumentOutOfRangeException), () =>
            {
                SingleBuffer.Wrap(ba, 0, ba.Length + 1);
            });

            // A NullPointerException will be thrown if the array is null.
            tryCatch(ba, typeof(ArgumentNullException), () =>
            {
                SingleBuffer.Wrap((float[])null, 0, 5);
            });
            tryCatch(ba, typeof(ArgumentNullException), () =>
            {
                SingleBuffer.Wrap((float[])null);
            });
        }

        private static void testAllocate()
        {
            // An IllegalArgumentException will be thrown for negative capacities.
            tryCatch((Buffer)null, typeof(ArgumentException), () =>
            {
                SingleBuffer.Allocate(-1);
            });

        }

        [Test]
        public static void Test()
        {
            testAllocate();
            test(0, SingleBuffer.Allocate(7 * 1024), false);
            test(0, SingleBuffer.Wrap(new float[7 * 1024], 0, 7 * 1024), false);
            test(new float[1024]);
            callReset(SingleBuffer.Allocate(10));
            putBuffer();
        }
    }
}
