using J2N.Numerics;
using System;
using System.ComponentModel;
using System.Text;

namespace J2N.Collections
{
    using SR = J2N.Resources.Strings;

    /// <summary>
    /// The <see cref="BitSet"/> class implements a bit field. Each element in a
    /// <see cref="BitSet"/> can be on(1) or off(0). A <see cref="BitSet"/> is created with a
    /// given size and grows if this size is exceeded. Growth is always rounded to a
    /// 64 bit boundary.
    /// <para/>
    /// Usage Note: Where possible, it is recommended to use <see cref="System.Collections.BitArray"/>.
    /// However, there are some members that don't exist on <see cref="System.Collections.BitArray"/>
    /// which make the use of <see cref="BitSet"/> sometimes necessary.
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class BitSet // TODO: Add methods to copy to/from and compare with BitArray, implement ICollection
#if FEATURE_CLONEABLE
        : ICloneable
#endif
    {
        private const int Offset = 6;

        private const int ElmSize = 1 << Offset;

        private const int RightBits = ElmSize - 1;

        private static readonly long[] TwoNArray = new long[] { 0x1L, 0x2L, 0x4L,
            0x8L, 0x10L, 0x20L, 0x40L, 0x80L, 0x100L, 0x200L, 0x400L, 0x800L,
            0x1000L, 0x2000L, 0x4000L, 0x8000L, 0x10000L, 0x20000L, 0x40000L,
            0x80000L, 0x100000L, 0x200000L, 0x400000L, 0x800000L, 0x1000000L,
            0x2000000L, 0x4000000L, 0x8000000L, 0x10000000L, 0x20000000L,
            0x40000000L, 0x80000000L, 0x100000000L, 0x200000000L, 0x400000000L,
            0x800000000L, 0x1000000000L, 0x2000000000L, 0x4000000000L,
            0x8000000000L, 0x10000000000L, 0x20000000000L, 0x40000000000L,
            0x80000000000L, 0x100000000000L, 0x200000000000L, 0x400000000000L,
            0x800000000000L, 0x1000000000000L, 0x2000000000000L,
            0x4000000000000L, 0x8000000000000L, 0x10000000000000L,
            0x20000000000000L, 0x40000000000000L, 0x80000000000000L,
            0x100000000000000L, 0x200000000000000L, 0x400000000000000L,
            0x800000000000000L, 0x1000000000000000L, 0x2000000000000000L,
            0x4000000000000000L, unchecked((long)0x8000000000000000L) };

        internal long[] bits; // internal for testing serialization

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private bool needClear; // non-serializable

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private int actualArrayLength; // non-serializable

#if FEATURE_SERIALIZABLE
        [NonSerialized]
#endif
        private bool isLengthActual; // non-serializable

        /// <summary>
        /// Create a new <see cref="BitSet"/> with size equal to 64 bits.
        /// </summary>
        /// <seealso cref="Clear(int)"/>
        /// <seealso cref="Set(int)"/>
        /// <seealso cref="Clear()"/>
        /// <seealso cref="Clear(int, int)"/>
        /// <seealso cref="Set(int, bool)"/>
        /// <seealso cref="Set(int, int)"/>
        /// <seealso cref="Set(int, int, bool)"/>
        public BitSet()
        {
            bits = new long[1];
            actualArrayLength = 0;
            isLengthActual = true;
        }

        /// <summary>
        /// Create a new <see cref="BitSet"/> with size equal to nbits. If nbits is not a
        /// multiple of 64, then create a <see cref="BitSet"/> with size nbits rounded to
        /// the next closest multiple of 64.
        /// </summary>
        /// <param name="nbits">The size of the bit set.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="nbits"/> is negative.</exception>
        /// <seealso cref="Clear(int)"/>
        /// <seealso cref="Set(int)"/>
        /// <seealso cref="Clear()"/>
        /// <seealso cref="Clear(int, int)"/>
        /// <seealso cref="Set(int, bool)"/>
        /// <seealso cref="Set(int, int)"/>
        /// <seealso cref="Set(int, int, bool)"/>
        public BitSet(int nbits)
        {
            if (nbits < 0)
                throw new ArgumentOutOfRangeException(nameof(nbits), SR.ArgumentOutOfRange_NeedNonNegNum);

            bits = new long[(nbits >> Offset) + ((nbits & RightBits) > 0 ? 1 : 0)];
            actualArrayLength = 0;
            isLengthActual = true;
        }

        /// <summary>
        /// Private constructor called from <see cref="Get(int, int)"/> method.
        /// </summary>
        /// <param name="bits">The size of the bit set.</param>
        /// <param name="needClear"></param>
        /// <param name="actualArrayLength"></param>
        /// <param name="isLengthActual"></param>
        private BitSet(long[] bits, bool needClear, int actualArrayLength,
                bool isLengthActual)
        {
            this.bits = bits;
            this.needClear = needClear;
            this.actualArrayLength = actualArrayLength;
            this.isLengthActual = isLengthActual;
        }

        /// <summary>
        /// Creates a copy of this <see cref="BitSet"/>.
        /// </summary>
        /// <returns>A copy of this <see cref="BitSet"/>.</returns>
        public virtual object Clone()
        {
            BitSet clone = (BitSet)base.MemberwiseClone();
            clone.bits = (long[])bits.Clone();
            return clone;
        }

        /// <summary>
        /// Compares the argument to this <see cref="BitSet"/> and returns whether they are
        /// equal. The object must be an instance of <see cref="BitSet"/> with the same
        /// bits set.
        /// </summary>
        /// <param name="obj">The <see cref="BitSet"/> object to compare.</param>
        /// <returns>A <see cref="bool"/> indicating whether or not this <see cref="BitSet"/> and
        /// <paramref name="obj"/> are equal.</returns>
        /// <seealso cref="GetHashCode()"/>
        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj is BitSet)
            {
                long[] bsBits = ((BitSet)obj).bits;
                int length1 = this.actualArrayLength, length2 = ((BitSet)obj).actualArrayLength;
                if (this.isLengthActual && ((BitSet)obj).isLengthActual
                        && length1 != length2)
                {
                    return false;
                }
                // If one of the BitSets is larger than the other, check to see if
                // any of its extra bits are set. If so return false.
                if (length1 <= length2)
                {
                    for (int i = 0; i < length1; i++)
                    {
                        if (bits[i] != bsBits[i])
                        {
                            return false;
                        }
                    }
                    for (int i = length1; i < length2; i++)
                    {
                        if (bsBits[i] != 0)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < length2; i++)
                    {
                        if (bits[i] != bsBits[i])
                        {
                            return false;
                        }
                    }
                    for (int i = length2; i < length1; i++)
                    {
                        if (bits[i] != 0)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Increase the size of the internal array to accommodate <paramref name="length"/> bits.
        /// The new array max index will be a multiple of 64.
        /// </summary>
        /// <param name="length">The index the new array needs to be able to access.</param>
        private void GrowLength(int length)
        {
            long[] tempBits = new long[Math.Max(length, bits.Length * 2)];
            System.Array.Copy(bits, 0, tempBits, 0, this.actualArrayLength);
            bits = tempBits;
        }

        /// <summary>
        /// Computes the hash code for this <see cref="BitSet"/>. If two <see cref="BitSet"/>s are equal
        /// the have to return the same result for <see cref="GetHashCode()"/>.
        /// </summary>
        /// <returns>The <see cref="int"/> representing the hash code for this bit
        /// set.</returns>
        /// <seealso cref="Equals(object)"/>
        public override int GetHashCode()
        {
            long x = 1234;
            for (int i = 0, length = actualArrayLength; i < length; i++)
            {
                x ^= bits[i] * (i + 1);
            }
            return (int)((x >> 32) ^ x);
        }

        /// <summary>
        /// Retrieves the bit at index <paramref name="position"/>. Grows the <see cref="BitSet"/> if
        /// <paramref name="position"/> &gt; size.
        /// </summary>
        /// <param name="position">The index of the bit to be retrieved.</param>
        /// <returns><c>true</c> if the bit at <paramref name="position"/> is set,
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="position"/> is negative.</exception>
        /// <seealso cref="Clear(int)"/>
        /// <seealso cref="Set(int)"/>
        /// <seealso cref="Clear()"/>
        /// <seealso cref="Clear(int, int)"/>
        /// <seealso cref="Set(int, bool)"/>
        /// <seealso cref="Set(int, int)"/>
        /// <seealso cref="Set(int, int, bool)"/>
        public virtual bool Get(int position)
        {
            // Negative index specified
            if (position < 0)
                throw new ArgumentOutOfRangeException(nameof(position), SR.ArgumentOutOfRange_NeedNonNegNum); //$NON-NLS-1$

            int arrayPos = position >> Offset;
            if (arrayPos < actualArrayLength)
            {
                return (bits[arrayPos] & TwoNArray[position & RightBits]) != 0;
            }
            return false;
        }

        /// <summary>
        /// Retrieves the bits starting from <paramref name="position1"/> to <paramref name="position2"/> and returns
        /// back a new bitset made of these bits. Grows the <see cref="BitSet"/> if
        /// <paramref name="position2"/> &gt; size.
        /// </summary>
        /// <param name="position1">Beginning position.</param>
        /// <param name="position2">Ending position.</param>
        /// <returns>New bitset of the range specified.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="position1"/> or <paramref name="position2"/> is negative.</exception>
        /// <exception cref="ArgumentException"><paramref name="position1"/> is greater than <paramref name="position2"/>.</exception>
        /// <seealso cref="Get(int)"/>
        public virtual BitSet Get(int position1, int position2)
        {
            if (position1 < 0)
                throw new ArgumentOutOfRangeException(nameof(position1), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (position2 < 0)
                throw new ArgumentOutOfRangeException(nameof(position2), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (position2 < position1)
                throw new ArgumentException(nameof(position1), J2N.SR.Format(SR.Argument_MinMaxValue, nameof(position1), nameof(position2)));

            int last = actualArrayLength << Offset;
            if (position1 >= last || position1 == position2)
            {
                return new BitSet(0);
            }
            if (position2 > last)
            {
                position2 = last;
            }

            int idx1 = position1 >> Offset;
            int idx2 = (position2 - 1) >> Offset;
            long factor1 = (~0L) << (position1 & RightBits);
            long factor2 = (~0L).TripleShift(ElmSize - (position2 & RightBits));

            if (idx1 == idx2)
            {
                long result = (bits[idx1] & (factor1 & factor2)).TripleShift(position1 % ElmSize);
                if (result == 0)
                {
                    return new BitSet(0);
                }
                return new BitSet(new long[] { result }, needClear, 1, true);
            }
            long[] newbits = new long[idx2 - idx1 + 1];
            // first fill in the first and last indexes in the new bitset
            newbits[0] = bits[idx1] & factor1;
            newbits[newbits.Length - 1] = bits[idx2] & factor2;

            // fill in the in between elements of the new bitset
            for (int i = 1; i < idx2 - idx1; i++)
            {
                newbits[i] = bits[idx1 + i];
            }

            // shift all the elements in the new bitset to the right by pos1
            // % ELM_SIZE
            int numBitsToShift = position1 & RightBits;
            int actualLen = newbits.Length;
            if (numBitsToShift != 0)
            {
                for (int i = 0; i < newbits.Length; i++)
                {
                    // shift the current element to the right regardless of
                    // sign
                    newbits[i] = newbits[i].TripleShift(numBitsToShift);

                    // apply the last x bits of newbits[i+1] to the current
                    // element
                    if (i != newbits.Length - 1)
                    {
                        newbits[i] |= newbits[i + 1] << (ElmSize - (numBitsToShift));
                    }
                    if (newbits[i] != 0)
                    {
                        actualLen = i + 1;
                    }
                }
            }
            return new BitSet(newbits, needClear, actualLen,
                    newbits[actualLen - 1] != 0);
        }

        /// <summary>
        /// Sets the bit at index <paramref name="position"/> to 1. Grows the <see cref="BitSet"/> if
        /// <paramref name="position"/> &gt; size.
        /// </summary>
        /// <param name="position">The index of the bit to set.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="position"/> is negative.</exception>
        /// <seealso cref="Clear(int)"/>
        /// <seealso cref="Clear()"/>
        /// <seealso cref="Clear(int, int)"/>
        public virtual void Set(int position)
        {
            if (position < 0)
                throw new ArgumentOutOfRangeException(nameof(position), SR.ArgumentOutOfRange_NeedNonNegNum); //$NON-NLS-1$

            int len = (position >> Offset) + 1;
            if (len > bits.Length)
            {
                GrowLength(len);
            }
            bits[len - 1] |= TwoNArray[position & RightBits];
            if (len > actualArrayLength)
            {
                actualArrayLength = len;
                isLengthActual = true;
            }
            NeedClear();
        }

        /// <summary>
        /// Sets the bit at index <paramref name="position"/> to <paramref name="value"/>. Grows the
        /// <see cref="BitSet"/> if <paramref name="position"/> &gt; size.
        /// </summary>
        /// <param name="position">The index of the bit to set.</param>
        /// <param name="value">Value to set the bit.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="position"/> is negative.</exception>
        /// <seealso cref="Set(int)"/>
        public virtual void Set(int position, bool value)
        {
            if (value)
            {
                Set(position);
            }
            else
            {
                Clear(position);
            }
        }

        /// <summary>
        /// Sets the bits starting from <paramref name="position1"/> to <paramref name="position2"/>. Grows the
        /// <see cref="BitSet"/> if <paramref name="position2"/> &gt; size.
        /// </summary>
        /// <param name="position1">Beginning position.</param>
        /// <param name="position2">Ending position.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="position1"/> or <paramref name="position2"/> is negative.</exception>
        /// <exception cref="ArgumentException"><paramref name="position1"/> is greater than <paramref name="position2"/>.</exception>
        /// <seealso cref="Set(int)"/>
        public virtual void Set(int position1, int position2)
        {
            if (position1 < 0)
                throw new ArgumentOutOfRangeException(nameof(position1), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (position2 < 0)
                throw new ArgumentOutOfRangeException(nameof(position2), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (position2 < position1)
                throw new ArgumentException(nameof(position1), J2N.SR.Format(SR.Argument_MinMaxValue, nameof(position1), nameof(position2)));

            if (position1 == position2)
            {
                return;
            }
            int len2 = ((position2 - 1) >> Offset) + 1;
            if (len2 > bits.Length)
            {
                GrowLength(len2);
            }

            int idx1 = position1 >> Offset;
            int idx2 = (position2 - 1) >> Offset;
            long factor1 = (~0L) << (position1 & RightBits);
            long factor2 = (~0L).TripleShift(ElmSize - (position2 & RightBits));

            if (idx1 == idx2)
            {
                bits[idx1] |= (factor1 & factor2);
            }
            else
            {
                bits[idx1] |= factor1;
                bits[idx2] |= factor2;
                for (int i = idx1 + 1; i < idx2; i++)
                {
                    bits[i] |= (~0L);
                }
            }
            if (idx2 + 1 > actualArrayLength)
            {
                actualArrayLength = idx2 + 1;
                isLengthActual = true;
            }
            NeedClear();
        }

        private void NeedClear()
        {
            this.needClear = true;
        }

        /// <summary>
        /// Sets the bits starting from <paramref name="position1"/> to <paramref name="position2"/> to the given
        /// <paramref name="value"/>. Grows the <see cref="BitSet"/> if <paramref name="position2"/> &gt; size.
        /// </summary>
        /// <param name="position1">Beginning position.</param>
        /// <param name="position2">Ending position.</param>
        /// <param name="value">Value to set these bits.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="position1"/> or <paramref name="position2"/> is negative, or if
        /// <paramref name="position2"/> is smaller than <paramref name="position1"/>.</exception>
        /// <seealso cref="Set(int, int)"/>
        public virtual void Set(int position1, int position2, bool value)
        {
            if (value)
            {
                Set(position1, position2);
            }
            else
            {
                Clear(position1, position2);
            }
        }

        /// <summary>
        /// Clears all the bits in this <see cref="BitSet"/>.
        /// </summary>
        /// <seealso cref="Clear(int)"/>
        /// <seealso cref="Clear(int, int)"/>
        public virtual void Clear()
        {
            if (needClear)
            {
                for (int i = 0; i < bits.Length; i++)
                {
                    bits[i] = 0L;
                }
                actualArrayLength = 0;
                isLengthActual = true;
                needClear = false;
            }
        }

        /// <summary>
        /// Clears the bit at index <paramref name="position"/>. Grows the <see cref="BitSet"/> if
        /// <paramref name="position"/> &gt; size.
        /// </summary>
        /// <param name="position">The index of the bit to clear.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="position"/> is negative.</exception>
        /// <seealso cref="Clear(int, int)"/>
        public virtual void Clear(int position)
        {
            if (position < 0)
                // Negative index specified
                throw new ArgumentOutOfRangeException(nameof(position), SR.ArgumentOutOfRange_NeedNonNegNum); //$NON-NLS-1$

            if (!needClear)
            {
                return;
            }
            int arrayPos = position >> Offset;
            if (arrayPos < actualArrayLength)
            {
                bits[arrayPos] &= ~(TwoNArray[position & RightBits]);
                if (bits[actualArrayLength - 1] == 0)
                {
                    isLengthActual = false;
                }
            }
        }

        /// <summary>
        /// Clears the bits starting from <paramref name="position1"/> to <paramref name="position2"/>. Grows the
        /// <see cref="BitSet"/> if <paramref name="position2"/> &gt; size;
        /// </summary>
        /// <param name="position1">Beginning position.</param>
        /// <param name="position2">Ending position.</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="position1"/> or <paramref name="position2"/> is negative.</exception>
        /// <exception cref="ArgumentException"><paramref name="position1"/> is greater than <paramref name="position2"/>.</exception>
        /// <seealso cref="Clear(int)"/>
        public virtual void Clear(int position1, int position2)
        {
            if (position1 < 0)
                throw new ArgumentOutOfRangeException(nameof(position1), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (position2 < 0)
                throw new ArgumentOutOfRangeException(nameof(position2), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (position2 < position1)
                throw new ArgumentException(nameof(position1), J2N.SR.Format(SR.Argument_MinMaxValue, nameof(position1), nameof(position2)));

            if (!needClear)
            {
                return;
            }
            int last = (actualArrayLength << Offset);
            if (position1 >= last || position1 == position2)
            {
                return;
            }
            if (position2 > last)
            {
                position2 = last;
            }

            int idx1 = position1 >> Offset;
            int idx2 = (position2 - 1) >> Offset;
            long factor1 = (~0L) << (position1 & RightBits);
            long factor2 = (~0L).TripleShift(ElmSize - (position2 & RightBits));

            if (idx1 == idx2)
            {
                bits[idx1] &= ~(factor1 & factor2);
            }
            else
            {
                bits[idx1] &= ~factor1;
                bits[idx2] &= ~factor2;
                for (int i = idx1 + 1; i < idx2; i++)
                {
                    bits[i] = 0L;
                }
            }
            if ((actualArrayLength > 0) && (bits[actualArrayLength - 1] == 0))
            {
                isLengthActual = false;
            }
        }

        /// <summary>
        /// Flips the bit at index <paramref name="position"/>. Grows the <see cref="BitSet"/> if
        /// <paramref name="position"/> &gt; size.
        /// </summary>
        /// <param name="position">The index of the bit to flip.</param>
        /// <exception cref="IndexOutOfRangeException">If <paramref name="position"/> is negative.</exception>
        /// <seealso cref="Flip(int, int)"/>
        public virtual void Flip(int position)
        {
            if (position < 0)
                // Negative index specified
                throw new ArgumentOutOfRangeException(nameof(position), SR.ArgumentOutOfRange_NeedNonNegNum); //$NON-NLS-1$

            int len = (position >> Offset) + 1;
            if (len > bits.Length)
            {
                GrowLength(len);
            }
            bits[len - 1] ^= TwoNArray[position & RightBits];
            if (len > actualArrayLength)
            {
                actualArrayLength = len;
            }
            isLengthActual = !((actualArrayLength > 0) && (bits[actualArrayLength - 1] == 0));
            NeedClear();
        }

        /// <summary>
        /// Flips the bits starting from <paramref name="position1"/> to <paramref name="position2"/>. Grows the
        ///  <see cref="BitSet"/> if <paramref name="position2"/> &gt; size.
        /// </summary>
        /// <param name="position1">Beginning position.</param>
        /// <param name="position2">Ending position.</param>
        /// <exception cref="IndexOutOfRangeException">If <paramref name="position1"/> or <paramref name="position2"/> is negative.</exception>
        /// <exception cref="ArgumentException"><paramref name="position1"/> is greater than <paramref name="position2"/>.</exception>
        /// <seealso cref="Flip(int)"/>
        public virtual void Flip(int position1, int position2)
        {
            if (position1 < 0)
                throw new ArgumentOutOfRangeException(nameof(position1), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (position2 < 0)
                throw new ArgumentOutOfRangeException(nameof(position2), SR.ArgumentOutOfRange_NeedNonNegNum);
            if (position2 < position1)
                throw new ArgumentException(nameof(position1), J2N.SR.Format(SR.Argument_MinMaxValue, nameof(position1), nameof(position2)));

            if (position1 == position2)
            {
                return;
            }
            int len2 = ((position2 - 1) >> Offset) + 1;
            if (len2 > bits.Length)
            {
                GrowLength(len2);
            }

            int idx1 = position1 >> Offset;
            int idx2 = (position2 - 1) >> Offset;
            long factor1 = (~0L) << (position1 & RightBits);
            long factor2 = (~0L).TripleShift(ElmSize - (position2 & RightBits));

            if (idx1 == idx2)
            {
                bits[idx1] ^= (factor1 & factor2);
            }
            else
            {
                bits[idx1] ^= factor1;
                bits[idx2] ^= factor2;
                for (int i = idx1 + 1; i < idx2; i++)
                {
                    bits[i] ^= (~0L);
                }
            }
            if (len2 > actualArrayLength)
            {
                actualArrayLength = len2;
            }
            isLengthActual = !((actualArrayLength > 0) && (bits[actualArrayLength - 1] == 0));
            NeedClear();
        }

        /// <summary>
        /// Checks if these two <see cref="BitSet"/>s have at least one bit set to true in the same
        /// position.
        /// </summary>
        /// <param name="bitSet"><see cref="BitSet"/> used to calculate the intersection.</param>
        /// <returns><c>true</c> if bs intersects with this <see cref="BitSet"/>,
        /// <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="bitSet"/> is <c>null</c>.</exception>
        public virtual bool Intersects(BitSet bitSet) // TODO: API - Make a member of ISet<T>?
        {
            if (bitSet == null)
                throw new ArgumentNullException(nameof(bitSet));

            long[] bsBits = bitSet.bits;
            int length1 = actualArrayLength, length2 = bitSet.actualArrayLength;

            if (length1 <= length2)
            {
                for (int i = 0; i < length1; i++)
                {
                    if ((bits[i] & bsBits[i]) != 0L)
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < length2; i++)
                {
                    if ((bits[i] & bsBits[i]) != 0L)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Performs the logical AND of this <see cref="BitSet"/> with another
        /// <see cref="BitSet"/>. The values of this <see cref="BitSet"/> are changed accordingly.
        /// </summary>
        /// <param name="bitSet"><see cref="BitSet"/> to AND with.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="bitSet"/> is <c>null</c>.</exception>
        /// <seealso cref="Or(BitSet)"/>
        /// <seealso cref="Xor(BitSet)"/>
        public virtual void And(BitSet bitSet) // TODO: API - Make a member of ISet<T>?
        {
            if (bitSet == null)
                throw new ArgumentNullException(nameof(bitSet));

            long[] bsBits = bitSet.bits;
            if (!needClear)
            {
                return;
            }
            int length1 = actualArrayLength, length2 = bitSet.actualArrayLength;
            if (length1 <= length2)
            {
                for (int i = 0; i < length1; i++)
                {
                    bits[i] &= bsBits[i];
                }
            }
            else
            {
                for (int i = 0; i < length2; i++)
                {
                    bits[i] &= bsBits[i];
                }
                for (int i = length2; i < length1; i++)
                {
                    bits[i] = 0;
                }
                actualArrayLength = length2;
            }
            isLengthActual = !((actualArrayLength > 0) && (bits[actualArrayLength - 1] == 0));
        }

        /// <summary>
        /// Clears all bits in the receiver which are also set in the <paramref name="bitSet"/> parameter.
        /// The values of this <see cref="BitSet"/> are changed accordingly.
        /// </summary>
        /// <param name="bitSet"><see cref="BitSet"/> to ANDNOT with.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="bitSet"/> is <c>null</c>.</exception>
        public virtual void AndNot(BitSet bitSet) // TODO: API - Make a member of ISet<T>?
        {
            if (bitSet == null)
                throw new ArgumentNullException(nameof(bitSet));

            long[] bsBits = bitSet.bits;
            if (!needClear)
            {
                return;
            }
            int range = actualArrayLength < bitSet.actualArrayLength ? actualArrayLength
                    : bitSet.actualArrayLength;
            for (int i = 0; i < range; i++)
            {
                bits[i] &= ~bsBits[i];
            }

            if (actualArrayLength < range)
            {
                actualArrayLength = range;
            }
            isLengthActual = !((actualArrayLength > 0) && (bits[actualArrayLength - 1] == 0));
        }

        /// <summary>
        /// Performs the logical OR of this <see cref="BitSet"/> with another <see cref="BitSet"/>.
        /// The values of this <see cref="BitSet"/> are changed accordingly.
        /// </summary>
        /// <param name="bitSet"><see cref="BitSet"/> to OR with.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="bitSet"/> is <c>null</c>.</exception>
        /// <seealso cref="Xor(BitSet)"/>
        /// <seealso cref="And(BitSet)"/>
        public virtual void Or(BitSet bitSet) // TODO: API - Make a member of ISet<T>?
        {
            if (bitSet == null)
                throw new ArgumentNullException(nameof(bitSet));

            int bsActualLen = bitSet.ActualArrayLength;
            if (bsActualLen > bits.Length)
            {
                long[] tempBits = new long[bsActualLen];
                System.Array.Copy(bitSet.bits, 0, tempBits, 0, bitSet.actualArrayLength);
                for (int i = 0; i < actualArrayLength; i++)
                {
                    tempBits[i] |= bits[i];
                }
                bits = tempBits;
                actualArrayLength = bsActualLen;
                isLengthActual = true;
            }
            else
            {
                long[] bsBits = bitSet.bits;
                for (int i = 0; i < bsActualLen; i++)
                {
                    bits[i] |= bsBits[i];
                }
                if (bsActualLen > actualArrayLength)
                {
                    actualArrayLength = bsActualLen;
                    isLengthActual = true;
                }
            }
            NeedClear();
        }

        /// <summary>
        /// Performs the logical XOR of this <see cref="BitSet"/> with another <see cref="BitSet"/>.
        /// The values of this <see cref="BitSet"/> are changed accordingly.
        /// </summary>
        /// <param name="bitSet"><see cref="BitSet"/> to XOR with.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="bitSet"/> is <c>null</c>.</exception>
        /// <seealso cref="Or(BitSet)"/>
        /// <seealso cref="And(BitSet)"/>
        public virtual void Xor(BitSet bitSet) // TODO: API - Make a member of ISet<T>?
        {
            if (bitSet == null)
                throw new ArgumentNullException(nameof(bitSet));

            int bsActualLen = bitSet.ActualArrayLength;
            if (bsActualLen > bits.Length)
            {
                long[] tempBits = new long[bsActualLen];
                System.Array.Copy(bitSet.bits, 0, tempBits, 0, bitSet.actualArrayLength);
                for (int i = 0; i < actualArrayLength; i++)
                {
                    tempBits[i] ^= bits[i];
                }
                bits = tempBits;
                actualArrayLength = bsActualLen;
                isLengthActual = !((actualArrayLength > 0) && (bits[actualArrayLength - 1] == 0));
            }
            else
            {
                long[] bsBits = bitSet.bits;
                for (int i = 0; i < bsActualLen; i++)
                {
                    bits[i] ^= bsBits[i];
                }
                if (bsActualLen > actualArrayLength)
                {
                    actualArrayLength = bsActualLen;
                    isLengthActual = true;
                }
            }
            NeedClear();
        }

        /// <summary>
        /// Returns the current capacity in bits (1 greater than the index of the last bit).</summary>
        /// <seealso cref="Length"/>
        public virtual int Capacity => bits.Length << Offset;

        /// <summary>
        /// Deprecated. Gets the number of bits this <see cref="BitSet"/> has.
        /// <para/>
        /// Use <see cref="Capacity"/> instead, as the name Count is not very clear what the property is intended for. This property is for compatibility purposes with the JDK bitset.
        /// </summary>
        /// <seealso cref="Length"/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual int Count => bits.Length << Offset;

        /// <summary>
        /// Returns the number of bits up to and including the highest bit set.
        /// </summary>
        /// <returns>The length of the <see cref="BitSet"/>.</returns>
        public virtual int Length
        {
            get
            {
                int idx = actualArrayLength - 1;
                while (idx >= 0 && bits[idx] == 0)
                {
                    --idx;
                }
                actualArrayLength = idx + 1;
                if (idx == -1)
                {
                    return 0;
                }
                int i = ElmSize - 1;
                long val = bits[idx];
                while ((val & (TwoNArray[i])) == 0 && i > 0)
                {
                    i--;
                }
                return (idx << Offset) + i + 1;
            }
        }

        private int ActualArrayLength
        {
            get
            {
                if (isLengthActual)
                {
                    return actualArrayLength;
                }
                int idx = actualArrayLength - 1;
                while (idx >= 0 && bits[idx] == 0)
                {
                    --idx;
                }
                actualArrayLength = idx + 1;
                isLengthActual = true;
                return actualArrayLength;
            }
        }

        /// <summary>
        /// Returns a string containing a concise, human-readable description of the
        /// receiver.
        /// </summary>
        /// <returns>A comma delimited list of the indices of all bits that are set.</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(bits.Length / 2);
            int bitCount = 0;
            sb.Append('{');
            bool comma = false;
            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] == 0)
                {
                    bitCount += ElmSize;
                    continue;
                }
                for (int j = 0; j < ElmSize; j++)
                {
                    if (((bits[i] & (TwoNArray[j])) != 0))
                    {
                        if (comma)
                        {
                            sb.Append(", "); //$NON-NLS-1$
                        }
                        sb.Append(bitCount);
                        comma = true;
                    }
                    bitCount++;
                }
            }
            sb.Append('}');
            return sb.ToString();
        }

        /// <summary>
        /// Returns the position of the first bit that is <c>true</c> on or after <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The starting position (inclusive).</param>
        /// <returns>-1 if there is no bits that are set to <c>true</c> on or after <paramref name="position"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="position"/> is less than zero.</exception>
        public virtual int NextSetBit(int position)
        {
            if (position < 0)
                throw new ArgumentOutOfRangeException(nameof(position), SR.ArgumentOutOfRange_NeedNonNegNum); //$NON-NLS-1$

            if (position >= actualArrayLength << Offset)
            {
                return -1;
            }

            int idx = position >> Offset;
            // first check in the same bit set element
            if (bits[idx] != 0L)
            {
                for (int j = position & RightBits; j < ElmSize; j++)
                {
                    if (((bits[idx] & (TwoNArray[j])) != 0))
                    {
                        return (idx << Offset) + j;
                    }
                }

            }
            idx++;
            while (idx < actualArrayLength && bits[idx] == 0L)
            {
                idx++;
            }
            if (idx == actualArrayLength)
            {
                return -1;
            }

            // we know for sure there is a bit set to true in this element
            // since the bitset value is not 0L
            for (int j = 0; j < ElmSize; j++)
            {
                if (((bits[idx] & (TwoNArray[j])) != 0))
                {
                    return (idx << Offset) + j;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the position of the first bit that is <c>false</c> on or after <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The starting position (inclusive).</param>
        /// <returns>the position of the next bit set to <c>false</c>, even if it is further
        /// than this <see cref="BitSet"/>'s size.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="position"/> is less than zero.</exception>
        public virtual int NextClearBit(int position)
        {
            if (position < 0)
                throw new ArgumentOutOfRangeException(nameof(position), SR.ArgumentOutOfRange_NeedNonNegNum); //$NON-NLS-1$

            int length = actualArrayLength;
            int bssize = length << Offset;
            if (position >= bssize)
            {
                return position;
            }

            int idx = position >> Offset;
            // first check in the same bit set element
            if (bits[idx] != (~0L))
            {
                for (int j = position % ElmSize; j < ElmSize; j++)
                {
                    if (((bits[idx] & (TwoNArray[j])) == 0))
                    {
                        return idx * ElmSize + j;
                    }
                }
            }
            idx++;
            while (idx < length && bits[idx] == (~0L))
            {
                idx++;
            }
            if (idx == length)
            {
                return bssize;
            }

            // we know for sure there is a bit set to true in this element
            // since the bitset value is not 0L
            for (int j = 0; j < ElmSize; j++)
            {
                if (((bits[idx] & (TwoNArray[j])) == 0))
                {
                    return (idx << Offset) + j;
                }
            }

            return bssize;
        }

        /// <summary>
        /// Returns true if all the bits in this <see cref="BitSet"/> are set to false.
        /// </summary>
        /// <returns><c>true</c> if the <see cref="BitSet"/> is empty, <c>false</c> otherwise.</returns>
        public virtual bool IsEmpty
        {
            get
            {
                if (!needClear)
                {
                    return true;
                }
                int length = bits.Length;
                for (int idx = 0; idx < length; idx++)
                {
                    if (bits[idx] != 0L)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Returns the number of bits that are <c>true</c> in this <see cref="BitSet"/>.
        /// </summary>
        /// <returns>The number of bits that are <c>true</c> in the set.</returns>
        public virtual int Cardinality
        {
            get
            {
                if (!needClear)
                {
                    return 0;
                }
                int count = 0;
                int length = bits.Length;
                // FIXME: need to test performance, if still not satisfied, change it to
                // 256-bits table based
                for (int idx = 0; idx < length; idx++)
                {
                    count += Pop(bits[idx] & 0xffffffffL);
                    count += Pop(bits[idx].TripleShift(32));
                }
                return count;
            }
        }

        private int Pop(long x)
        {
            x = x - (x.TripleShift(1) & 0x55555555);
            x = (x & 0x33333333) + ((x.TripleShift(2)) & 0x33333333);
            x = (x + (x.TripleShift(4))) & 0x0f0f0f0f;
            x = x + (x.TripleShift(8));
            x = x + (x.TripleShift(16));
            return (int)x & 0x0000003f;
        }

#if FEATURE_SERIALIZABLE
        [System.Runtime.Serialization.OnDeserialized]
        internal void OnDeserializedMethod(System.Runtime.Serialization.StreamingContext context)
        {
            this.isLengthActual = false;
            this.actualArrayLength = bits.Length;
            this.needClear = this.ActualArrayLength != 0;
        }
#endif
    }
}
