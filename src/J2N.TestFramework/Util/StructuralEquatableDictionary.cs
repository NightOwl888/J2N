using J2N.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace J2N.Util
{
    public class StructuralEquatableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IStructuralEquatable
    {
        private readonly DictionaryEqualityComparer<TKey, TValue> equalityComparer;

        public StructuralEquatableDictionary(DictionaryEqualityComparer<TKey, TValue> equalityComparer)
        {
            this.equalityComparer = equalityComparer ?? throw new ArgumentNullException(nameof(equalityComparer));
        }

        public bool Equals(object other, IEqualityComparer comparer)
        {
            if (other is IDictionary<TKey, TValue> otherDictionary)
                return equalityComparer.Equals(this, otherDictionary);
            return false;
        }

        public int GetHashCode(IEqualityComparer comparer)
        {
            return equalityComparer.GetHashCode(this);
        }

        //public override bool Equals(object obj)
        //{
        //    return Equals(obj, equalityComparer);
        //}

        //public override int GetHashCode()
        //{
        //    return GetHashCode(equalityComparer);
        //}
    }
}
