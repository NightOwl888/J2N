using J2N.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    /// <summary>
    /// A really, really simple bigint package
    /// tailored to the needs of floating base conversion.
    /// </summary>
    public class OldFDBigIntForTest
    {
        int nWords; // number of words used
        int[] data; // value: data[0] is least significant


        public OldFDBigIntForTest(int v)
        {
            nWords = 1;
            data = new int[1];
            data[0] = v;
        }

        public OldFDBigIntForTest(long v)
        {
            data = new int[2];
            data[0] = (int)v;
            data[1] = (int)(v.TripleShift(32));
            nWords = (data[1] == 0) ? 1 : 2;
        }

        public OldFDBigIntForTest(OldFDBigIntForTest other)
        {
            data = new int[nWords = other.nWords];
            Array.Copy(other.data, 0, data, 0, nWords);
        }

        private OldFDBigIntForTest(int[] d, int n)
        {
            data = d;
            nWords = n;
        }

        public OldFDBigIntForTest(long seed, char[] digit, int nd0, int nd)
        {
            int n = (nd + 8) / 9;        // estimate size needed.
            if (n < 2) n = 2;
            data = new int[n];      // allocate enough space
            data[0] = (int)seed;    // starting value
            data[1] = (int)(seed.TripleShift(32));
            nWords = (data[1] == 0) ? 1 : 2;
            int i = nd0;
            int limit = nd - 5;       // slurp digits 5 at a time.
            int v;
            while (i < limit)
            {
                int ilim = i + 5;
                v = (int)digit[i++] - (int)'0';
                while (i < ilim)
                {
                    v = 10 * v + (int)digit[i++] - (int)'0';
                }
                multaddMe(100000, v); // ... where 100000 is 10^5.
            }
            int factor = 1;
            v = 0;
            while (i < nd)
            {
                v = 10 * v + (int)digit[i++] - (int)'0';
                factor *= 10;
            }
            if (factor != 1)
            {
                multaddMe(factor, v);
            }
        }

        /*
         * Left shift by c bits.
         * Shifts this in place.
         */
        public void lshiftMe(int c)
        {
            if (c <= 0)
            {
                if (c == 0)
                    return; // silly.
                else
                    throw new ArgumentException("negative shift count");
            }
            int wordcount = c >> 5;
            int bitcount = c & 0x1f;
            int anticount = 32 - bitcount;
            int[] t = data;
            int[] s = data;
            if (nWords + wordcount + 1 > t.Length)
            {
                // reallocate.
                t = new int[nWords + wordcount + 1];
            }
            int target = nWords + wordcount;
            int src = nWords - 1;
            if (bitcount == 0)
            {
                // special hack, since an anticount of 32 won't go!
                Array.Copy(s, 0, t, wordcount, nWords);
                target = wordcount - 1;
            }
            else
            {
                t[target--] = s[src].TripleShift(anticount);
                while (src >= 1)
                {
                    t[target--] = (s[src] << bitcount) | (s[--src].TripleShift(anticount));
                }
                t[target--] = s[src] << bitcount;
            }
            while (target >= 0)
            {
                t[target--] = 0;
            }
            data = t;
            nWords += wordcount + 1;
            // may have constructed high-order word of 0.
            // if so, trim it
            while (nWords > 1 && data[nWords - 1] == 0)
                nWords--;
        }

        /*
         * normalize this number by shifting until
         * the MSB of the number is at 0x08000000.
         * This is in preparation for quoRemIteration, below.
         * The idea is that, to make division easier, we want the
         * divisor to be "normalized" -- usually this means shifting
         * the MSB into the high words sign bit. But because we know that
         * the quotient will be 0 < q < 10, we would like to arrange that
         * the dividend not span up into another word of precision.
         * (This needs to be explained more clearly!)
         */
        public int normalizeMe()
        {
            int src;
            int wordcount = 0;
            int bitcount = 0;
            int v = 0;
            for (src = nWords - 1; src >= 0 && (v = data[src]) == 0; src--)
            {
                wordcount += 1;
            }
            if (src < 0)
            {
                // oops. Value is zero. Cannot normalize it!
                throw new ArgumentException("zero value");
            }
            /*
             * In most cases, we assume that wordcount is zero. This only
             * makes sense, as we try not to maintain any high-order
             * words full of zeros. In fact, if there are zeros, we will
             * simply SHORTEN our number at this point. Watch closely...
             */
            nWords -= wordcount;
            /*
             * Compute how far left we have to shift v s.t. its highest-
             * order bit is in the right place. Then call lshiftMe to
             * do the work.
             */
            if ((v & 0xf0000000) != 0)
            {
                // will have to shift up into the next word.
                // too bad.
                for (bitcount = 32; (v & 0xf0000000) != 0; bitcount--)
                    //v >>>= 1;
                    v = v.TripleShift(1);
            }
            else
            {
                while (v <= 0x000fffff)
                {
                    // hack: byte-at-a-time shifting
                    v <<= 8;
                    bitcount += 8;
                }
                while (v <= 0x07ffffff)
                {
                    v <<= 1;
                    bitcount += 1;
                }
            }
            if (bitcount != 0)
                lshiftMe(bitcount);
            return bitcount;
        }

        /*
         * Multiply a OldFDBigIntForTest by an int.
         * Result is a new OldFDBigIntForTest.
         */
        public OldFDBigIntForTest
        mult(int iv)
        {
            long v = iv;
            int[] r;
            long p;

            // guess adequate size of r.
            r = new int[(v * ((long)data[nWords - 1] & 0xffffffffL) > 0xfffffffL) ? nWords + 1 : nWords];
            p = 0L;
            for (int i = 0; i < nWords; i++)
            {
                p += v * ((long)data[i] & 0xffffffffL);
                r[i] = (int)p;
                //p >>>= 32;
                p = p.TripleShift(32);
            }
            if (p == 0L)
            {
                return new OldFDBigIntForTest(r, nWords);
            }
            else
            {
                r[nWords] = (int)p;
                return new OldFDBigIntForTest(r, nWords + 1);
            }
        }

        /*
         * Multiply a OldFDBigIntForTest by an int and add another int.
         * Result is computed in place.
         * Hope it fits!
         */
        public void multaddMe(int iv, int addend)
        {
            long v = iv;
            long p;

            // unroll 0th iteration, doing addition.
            p = v * ((long)data[0] & 0xffffffffL) + ((long)addend & 0xffffffffL);
            data[0] = (int)p;
            //p >>>= 32;
            p = p.TripleShift(32);
            for (int i = 1; i < nWords; i++)
            {
                p += v * ((long)data[i] & 0xffffffffL);
                data[i] = (int)p;
                //p >>>= 32;
                p = p.TripleShift(32);
            }
            if (p != 0L)
            {
                data[nWords] = (int)p; // will fail noisily if illegal!
                nWords++;
            }
        }

        /*
         * Multiply a OldFDBigIntForTest by another OldFDBigIntForTest.
         * Result is a new OldFDBigIntForTest.
         */
        public OldFDBigIntForTest mult(OldFDBigIntForTest other)
        {
            // crudely guess adequate size for r
            int[] r = new int[nWords + other.nWords];
            int i;
            // I think I am promised zeros...

            for (i = 0; i < this.nWords; i++)
            {
                long v = (long)this.data[i] & 0xffffffffL; // UNSIGNED CONVERSION
                long p = 0L;
                int j;
                for (j = 0; j < other.nWords; j++)
                {
                    p += ((long)r[i + j] & 0xffffffffL) + v * ((long)other.data[j] & 0xffffffffL); // UNSIGNED CONVERSIONS ALL 'ROUND.
                    r[i + j] = (int)p;
                    //p >>>= 32;
                    p = p.TripleShift(32);
                }
                r[i + j] = (int)p;
            }
            // compute how much of r we actually needed for all that.
            for (i = r.Length - 1; i > 0; i--)
                if (r[i] != 0)
                    break;
            return new OldFDBigIntForTest(r, i + 1);
        }

        /*
         * Add one OldFDBigIntForTest to another. Return a OldFDBigIntForTest
         */
        public OldFDBigIntForTest add(OldFDBigIntForTest other)
        {
            int i;
            int[] a, b;
            int n, m;
            long c = 0L;
            // arrange such that a.nWords >= b.nWords;
            // n = a.nWords, m = b.nWords
            if (this.nWords >= other.nWords)
            {
                a = this.data;
                n = this.nWords;
                b = other.data;
                m = other.nWords;
            }
            else
            {
                a = other.data;
                n = other.nWords;
                b = this.data;
                m = this.nWords;
            }
            int[] r = new int[n];
            for (i = 0; i < n; i++)
            {
                c += (long)a[i] & 0xffffffffL;
                if (i < m)
                {
                    c += (long)b[i] & 0xffffffffL;
                }
                r[i] = (int)c;
                c >>= 32; // signed shift.
            }
            if (c != 0L)
            {
                // oops -- carry out -- need longer result.
                int[] s = new int[r.Length + 1];
                Array.Copy(r, 0, s, 0, r.Length);
                s[i++] = (int)c;
                return new OldFDBigIntForTest(s, i);
            }
            return new OldFDBigIntForTest(r, i);
        }

        /*
         * Subtract one OldFDBigIntForTest from another. Return a OldFDBigIntForTest
         * Assert that the result is positive.
         */
        public OldFDBigIntForTest sub(OldFDBigIntForTest other)
        {
            int[] r = new int[this.nWords];
            int i;
            int n = this.nWords;
            int m = other.nWords;
            int nzeros = 0;
            long c = 0L;
            for (i = 0; i < n; i++)
            {
                c += (long)this.data[i] & 0xffffffffL;
                if (i < m)
                {
                    c -= (long)other.data[i] & 0xffffffffL;
                }
                if ((r[i] = (int)c) == 0)
                    nzeros++;
                else
                    nzeros = 0;
                c >>= 32; // signed shift
            }
            Debug.Assert(c == 0L, c.ToString()); // borrow out of subtract
            Debug.Assert(dataInRangeIsZero(i, m, other)); // negative result of subtract
            return new OldFDBigIntForTest(r, n - nzeros);
        }

        private static bool dataInRangeIsZero(int i, int m, OldFDBigIntForTest other)
        {
            while (i < m)
                if (other.data[i++] != 0)
                    return false;
            return true;
        }

        /*
         * Compare OldFDBigIntForTest with another OldFDBigIntForTest. Return an integer
         * >0: this > other
         *  0: this == other
         * <0: this < other
         */
        public int cmp(OldFDBigIntForTest other)
        {
            int i;
            if (this.nWords > other.nWords)
            {
                // if any of my high-order words is non-zero,
                // then the answer is evident
                int j = other.nWords - 1;
                for (i = this.nWords - 1; i > j; i--)
                    if (this.data[i] != 0) return 1;
            }
            else if (this.nWords < other.nWords)
            {
                // if any of other's high-order words is non-zero,
                // then the answer is evident
                int j = this.nWords - 1;
                for (i = other.nWords - 1; i > j; i--)
                    if (other.data[i] != 0) return -1;
            }
            else
            {
                i = this.nWords - 1;
            }
            for (; i > 0; i--)
                if (this.data[i] != other.data[i])
                    break;
            // careful! want unsigned compare!
            // use brute force here.
            int a = this.data[i];
            int b = other.data[i];
            if (a < 0)
            {
                // a is really big, unsigned
                if (b < 0)
                {
                    return a - b; // both big, negative
                }
                else
                {
                    return 1; // b not big, answer is obvious;
                }
            }
            else
            {
                // a is not really big
                if (b < 0)
                {
                    // but b is really big
                    return -1;
                }
                else
                {
                    return a - b;
                }
            }
        }

        /*
         * Compute
         * q = (int)( this / S )
         * this = 10 * ( this mod S )
         * Return q.
         * This is the iteration step of digit development for output.
         * We assume that S has been normalized, as above, and that
         * "this" has been lshift'ed accordingly.
         * Also assume, of course, that the result, q, can be expressed
         * as an integer, 0 <= q < 10.
         */
        public int quoRemIteration(OldFDBigIntForTest S)
        {
            // ensure that this and S have the same number of
            // digits. If S is properly normalized and q < 10 then
            // this must be so.
            if (nWords != S.nWords)
            {
                throw new ArgumentException("disparate values");
            }
            // estimate q the obvious way. We will usually be
            // right. If not, then we're only off by a little and
            // will re-add.
            int n = nWords - 1;
            long q = ((long)data[n] & 0xffffffffL) / (long)S.data[n];
            long diff = 0L;
            for (int i = 0; i <= n; i++)
            {
                diff += ((long)data[i] & 0xffffffffL) - q * ((long)S.data[i] & 0xffffffffL);
                data[i] = (int)diff;
                diff >>= 32; // N.B. SIGNED shift.
            }
            if (diff != 0L)
            {
                // damn, damn, damn. q is too big.
                // add S back in until this turns +. This should
                // not be very many times!
                long sum = 0L;
                while (sum == 0L)
                {
                    sum = 0L;
                    for (int i = 0; i <= n; i++)
                    {
                        sum += ((long)data[i] & 0xffffffffL) + ((long)S.data[i] & 0xffffffffL);
                        data[i] = (int)sum;
                        //sum >>= 32; // Signed or unsigned, answer is 0 or 1
                        sum = sum.TripleShift(32);
                    }
                    /*
                     * Originally the following line read
                     * "if ( sum !=0 && sum != -1 )"
                     * but that would be wrong, because of the
                     * treatment of the two values as entirely unsigned,
                     * it would be impossible for a carry-out to be interpreted
                     * as -1 -- it would have to be a single-bit carry-out, or
                     * +1.
                     */
                    Debug.Assert(sum == 0 || sum == 1, sum.ToString()); // carry out of division correction
                    q -= 1;
                }
            }
            // finally, we can multiply this by 10.
            // it cannot overflow, right, as the high-order word has
            // at least 4 high-order zeros!
            long p = 0L;
            for (int i = 0; i <= n; i++)
            {
                p += 10 * ((long)data[i] & 0xffffffffL);
                data[i] = (int)p;
                p >>= 32; // SIGNED shift.
            }
            Debug.Assert(p == 0L, p.ToString()); // Carry out of *10
            return (int)q;
        }

        public long longValue()
        {
            // if this can be represented as a long, return the value
            Debug.Assert(this.nWords > 0, this.nWords.ToString()); // longValue confused

            if (this.nWords == 1)
                return ((long)data[0] & 0xffffffffL);

            Debug.Assert(dataInRangeIsZero(2, this.nWords, this)); // value too big
            Debug.Assert(data[1] >= 0);  // value too big
            return ((long)(data[1]) << 32) | ((long)data[0] & 0xffffffffL);
        }

        public String toString()
        {
            StringBuffer r = new StringBuffer(30);
            r.Append('[');
            int i = Math.Min(nWords - 1, data.Length - 1);
            if (nWords > data.Length)
            {
                r.Append("(" + data.Length + "<" + nWords + "!)");
            }
            for (; i > 0; i--)
            {
                r.Append(data[i].ToHexString());
                r.Append(' ');
            }
            r.Append(data[0].ToHexString());
            r.Append(']');
            return r.ToString();
        }
    }
}
