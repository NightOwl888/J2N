using System;
using System.Globalization;

namespace J2N.Util
{
    /// <summary>
    /// Very simple wrapper for <see cref="int"/> to make it into a reference type.
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class Integer : IComparable<Integer>
    {
        public Integer(int value)
        {
            this.Value = value;
        }

        public int Value { get; set; }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Integer integer)
            {
                return Value.Equals(integer.Value);
            }
            return Value.Equals(obj);
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public int CompareTo(Integer other)
        {
            if (other == null)
                return 1;
            return this.Value.CompareTo(other.Value);
        }

        public static implicit operator int(Integer integer)
        {
            return integer.Value;
        }

        public static implicit operator Integer(int integer)
        {
            return new Integer(integer);
        }
    }
}
